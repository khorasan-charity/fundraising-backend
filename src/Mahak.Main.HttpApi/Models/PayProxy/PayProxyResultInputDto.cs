using System;

namespace Mahak.Main.Models.PayProxy;

public class PayProxyResultInputDto
{
    public int TerminalId { get; set; }
    public string? RefNum { get; set; }
    public Guid ResNum { get; set; }
    public string State { get; set; }
    public long? TraceNo { get; set; }
    public decimal Amount { get; set; }
    public decimal? AffectiveAmount { get; set; }
    public object? Wage { get; set; }
    public object? Rrn { get; set; }
    public string? SecurePan { get; set; }
    public int Status { get; set; }
    public string? Token { get; set; }
    public string? HashedCardNumber { get; set; }
}