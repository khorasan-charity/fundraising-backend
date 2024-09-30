using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mahak.Main.Files;

public class File : AuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string ContentType { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }

    public File(Guid id) : base(id)
    {
    }
}