using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Mahak.Main.Files;

public interface IFileAppService : IApplicationService
{
    Task<PagedResultDto<FileDto>> GetListAsync(PagedFileResultRequestDto input);
    Task<List<FileDto>> GetByIdsAsync(Guid[] ids);
    Task<IRemoteStreamContent> GetDownloadAsync(string filenameId);
    Task<FileDto> CreateAsync(IRemoteStreamContent file);
    Task<FileDto> UpdateAsync(Guid id, IRemoteStreamContent file);
    Task RemoveAsync(IEnumerable<Guid> ids);
}