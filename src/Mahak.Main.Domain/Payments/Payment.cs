using System.Collections.Generic;
using Mahak.Main.Transactions;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mahak.Main.Payments;

public class Payment : AuditedAggregateRoot<long>
{
    public required long TrackingNumber { get; set; }
    public required decimal Amount { get; set; }
    public required string Token { get; set; }
    public string? TransactionCode { get; set; }
    public required string GatewayName { get; set; }
    public string? GatewayAccountName { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPaid { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}