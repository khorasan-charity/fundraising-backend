using Volo.Abp.Domain.Entities;

namespace Mahak.Main.Attributes;

public class Attribute : AggregateRoot<int>
{
    public string Title { get; set; }
    public string? ValueType { get; set; }
    public string? ValueTypeTitle { get; set; }
    public string? Description { get; set; }
}