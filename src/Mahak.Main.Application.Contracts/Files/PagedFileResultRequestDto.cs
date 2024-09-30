using Volo.Abp.Application.Dtos;

namespace Mahak.Main.Files;

public class PagedFileResultRequestDto : PagedResultRequestDto
{
    public string? Filter { get; set; }
    public string? Extension { get; set; }
}