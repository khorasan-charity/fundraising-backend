using System;
using Volo.Abp.Application.Dtos;

namespace Mahak.Main.Files;

public class FileDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string ContentType { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }
}