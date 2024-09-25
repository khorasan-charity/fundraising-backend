using System.Security.Cryptography;
using System.Threading.Tasks;
using Mahak.Main.Controllers;
using Microsoft.AspNetCore.Mvc;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Storage.Abstractions;
using Volo.Abp;

namespace Mahak.Main;

[Route("/api/app/payment")]
public class PayController(IOnlinePayment onlinePayment, IStorageManager storageManager)
    : MainController
{
    [HttpGet]
    public async Task<IActionResult> Pay()
    {
        var trackingNumber = 0;
        do
        {
            trackingNumber = RandomNumberGenerator.GetInt32(100000, int.MaxValue);
        } while (await storageManager.DoesPaymentExistAsync(trackingNumber));

        var result = await onlinePayment.RequestAsync(x => x
            .SetGateway("ParbadVirtual")
            .SetAmount(50000)
            .SetTrackingNumber(trackingNumber)
            .SetCallbackUrl($"http://localhost:5000/api/app/payment/verify"));

        if (result.IsSucceed)
        {
            return result.GatewayTransporter.TransportToGateway();
        }

        throw new UserFriendlyException("Cannot create payment record");
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromQuery] string paymentToken)
    {
        var payment = await storageManager.GetPaymentByTokenAsync(paymentToken);
        return Ok(await onlinePayment.VerifyAsync(payment.TrackingNumber));
    }
}