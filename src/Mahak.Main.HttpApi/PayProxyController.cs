using System.Threading.Tasks;
using Mahak.Main.Controllers;
using Mahak.Main.Donations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Mahak.Main;

[Area("payment")]
[RemoteService(Name = "Payment")]
[Route("/api/app/payment/proxy")]
public class PayProxyController(
    IDonationAppService donationAppService)
    : MainController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] CreateDonationInputDto input)
    {
        var donationDto = await donationAppService.CreateAsync(input);

        return Redirect(
            $"https://khorasan-charity.org/payment?donationId={donationDto.Id}&amount={donationDto.Amount}");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmAsync([FromQuery] ConfirmDonationInputDto input)
    {
        var donationDto = await donationAppService.ConfirmAsync(input);
        
        return Redirect(
            $"https://donate.khorasan-charity.org/payment/result?donationId={donationDto.Id}&amount={donationDto.Amount}&transactionCode={input.TransactionCode}&success=true");
    }
}