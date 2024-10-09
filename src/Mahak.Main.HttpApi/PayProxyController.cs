using System.Threading.Tasks;
using Mahak.Main.Controllers;
using Mahak.Main.Donations;
using Mahak.Main.Models.PayProxy;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Data;

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

        // var request = HttpContext.Request;
        // if (request.Path.Value?.StartsWith("/apiendpoint", StringComparison.OrdinalIgnoreCase) == true)
        // {
        //     var json = JsonSerializer.SerializeObject(request.Path.Value
        //         .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
        //         .Skip(1));
        //     var response = context.HttpContext.Response;
        //
        //     response.Headers[HeaderNames.Location] = $"/custom";
        //     response.StatusCode = 307;
        //     context.Result = RuleResult.EndResponse;
        //     using (var bodyWriter = new StreamWriter(response.Body))
        //     {
        //         bodyWriter.Write(json);
        //         bodyWriter.Flush();
        //     }
        // }

        var html = $"""
                    <html>
                      <body>
                        <form action="https://khorasan-charity.org/payment/donate.php" method="post">
                          <input type="hidden" name="amount" value="{input.Amount}" />
                          <input type="hidden" name="refNum" value="{donationDto.Hash}" />
                          <input type="hidden" name="returnUrl" value="https://donate.khorasan-charity.org/api/app/payment/proxy/result" />
                        </form>
                        <script type="text/javascript">
                          document.forms[0].submit();
                        </script>
                      </body>
                    </html>
                    """;

        return Content(html, "text/html");
    }

    [HttpPost("result")]
    public async Task<IActionResult> ResultAsync([FromForm] PayProxyResultInputDto input)
    {
        var donationDto = input.State == "OK"
            ? await donationAppService.ConfirmAsync(new ConfirmDonationInputDto
            {
                Hash = input.ResNum,
                TransactionCode = input.TraceNo!.Value.ToString(),
                Token = input.Token!,
                ExtraProperties = new ExtraPropertyDictionary
                {
                    ["TerminalId"] = input.TerminalId,
                    ["RefNum"] = input.RefNum,
                    ["State"] = input.State,
                    ["TraceNo"] = input.TraceNo,
                    ["Amount"] = input.Amount,
                    ["AffectiveAmount"] = input.AffectiveAmount,
                    ["Wage"] = input.Wage,
                    ["Rrn"] = input.Rrn,
                    ["SecurePan"] = input.SecurePan,
                    ["Status"] = input.Status,
                    ["HashedCardNumber"] = input.HashedCardNumber
                }
            })
            : await donationAppService.RejectAsync(new RejectDonationInputDto
            {
                Hash = input.ResNum,
                ExtraProperties = new ExtraPropertyDictionary
                {
                    ["TerminalId"] = input.TerminalId,
                    ["ResNum"] = input.ResNum,
                    ["State"] = input.State,
                    ["TraceNo"] = input.TraceNo,
                    ["Amount"] = input.Amount,
                    ["Wage"] = input.Wage,
                    ["Rrn"] = input.Rrn,
                    ["SecurePan"] = input.SecurePan,
                    ["Status"] = input.Status,
                    ["HashedCardNumber"] = input.HashedCardNumber
                }
            });

        return Redirect(
            $"https://donate.khorasan-charity.org/#/payment/result?donationId={donationDto.Hash}");
    }
}