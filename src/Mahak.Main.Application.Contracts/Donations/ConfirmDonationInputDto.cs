using System;
using Volo.Abp.Data;

namespace Mahak.Main.Donations;

public class ConfirmDonationInputDto
{
    public Guid Hash { get; set; }
    public string TransactionCode { get; set; }
    public string Token { get; set; }
    public ExtraPropertyDictionary? ExtraProperties { get; set; } = new ExtraPropertyDictionary();
}