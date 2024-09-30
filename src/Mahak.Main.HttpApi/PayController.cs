using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Mahak.Main.Controllers;
using Mahak.Main.Models.Pay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Storage.Abstractions;
using Volo.Abp;

namespace Mahak.Main;

[Area("payment")]
[RemoteService(Name = "Payment")]
[Route("/api/app/payment")]
public class PayController(IOnlinePayment onlinePayment, IStorageManager storageManager)
    : MainController
{
    [HttpGet]
    public string Test()
    {
        return Request.Scheme + "://" + Request.Host.Value + Request.PathBase.Value;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromQuery] string gateway, [FromQuery] decimal amount,
        [FromQuery] string returnUrl)
    {
        gateway = "ParbadVirtual";

        var trackingNumber = 0;
        do
        {
            trackingNumber = RandomNumberGenerator.GetInt32(10000000, int.MaxValue);
        } while (await storageManager.DoesPaymentExistAsync(trackingNumber));

        var callbackUrl = Request.Scheme + "://" + Request.Host + Request.Path + "/verify?returnUrl="
                          + WebUtility.UrlEncode(returnUrl);
        
        var result = await onlinePayment.RequestAsync(x => x
            .SetGateway(gateway)
            .SetAmount(amount)
            .SetTrackingNumber(trackingNumber)
            .SetCallbackUrl(callbackUrl));

        if (result.IsSucceed)
        {
            return result.GatewayTransporter.TransportToGateway();
        }

        throw new UserFriendlyException("Cannot create payment record");
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromForm] VerifyPaymentInputDto input, [FromQuery] string paymentToken,
        [FromQuery] string? returnUrl)
    {
        var payment = await storageManager.GetPaymentByTokenAsync(paymentToken);
        var result = await onlinePayment.VerifyAsync(payment.TrackingNumber);

        returnUrl ??= string.Empty;
        returnUrl = returnUrl.Replace("#", "_SHARP_");
        returnUrl = QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string?>
        {
            { "paymentId", payment.Id.ToString() },
            { "paymentToken", payment.Token },
            { "trackingNumber", payment.TrackingNumber.ToString() },
            { "result", result.IsSucceed.ToString() }
        });
        returnUrl = returnUrl.Replace("_SHARP_", "#");
        return Redirect(returnUrl);
    }
}