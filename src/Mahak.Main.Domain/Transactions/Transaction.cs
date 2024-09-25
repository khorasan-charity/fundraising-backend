using Volo.Abp.Domain.Entities.Auditing;

namespace Mahak.Main.Transactions;

public class Transaction : AuditedAggregateRoot<long>
{
    public long PaymentId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public bool IsSucceed { get; set; }
    public string? Message { get; set; }
    public string? AdditionalData { get; set; }

    public Payments.Payment? Payment { get; set; }
}