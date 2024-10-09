using System;
using Volo.Abp.Data;

namespace Mahak.Main.Donations;

public class RejectDonationInputDto
{
    public Guid Hash { get; set; }
    public ExtraPropertyDictionary? ExtraProperties { get; set; }
}