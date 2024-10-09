using Volo.Abp.Application.Dtos;

namespace Mahak.Main.Payments;

public class PaymentDto : EntityDto<long>
{
    public long TrackingNumber { get; set; }
    public decimal Amount { get; set; }
    public string? Token { get; set; }
    public string? TransactionCode { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPaid { get; set; }
}