using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace Mahak.Main.Files;

[Authorize(Roles = "admin")]
public class FileAppService(FileManager fileManager, IReadOnlyRepository<File, Guid> readOnlyFileRepository)
    : MainAppService, IFileAppService
{
    public async Task<PagedResultDto<FileDto>> GetListAsync(PagedFileResultRequestDto input)
    {
        var q = await readOnlyFileRepository.GetQueryableAsync();
        q = q.WhereIf(!input.Filter.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Filter!))
            .WhereIf(!input.Extension.IsNullOrWhiteSpace(), x => x.Extension == input.Extension);

        var totalCount = await AsyncExecuter.CountAsync(q);
        var items = await AsyncExecuter.ToListAsync(q.OrderByDescending(x => x.CreationTime)
            .PageBy(input));
        var itemDtos = ObjectMapper.Map<List<File>, List<FileDto>>(items);
        return new PagedResultDto<FileDto>(totalCount, itemDtos);
    }

    [AllowAnonymous]
    public async Task<List<FileDto>> GetByIdsAsync(Guid[] ids)
    {
        var files = await readOnlyFileRepository.GetListAsync(x => ids.Contains(x.Id));

        return ObjectMapper.Map<List<File>, List<FileDto>>(files);
    }

    [AllowAnonymous]
    public async Task<IRemoteStreamContent> GetDownloadAsync(string filenameId)
    {
        var found = Guid.TryParse(filenameId.Split('.')[0].Trim(), out var fileInfoId);
        if (!found)
        {
            throw new UserFriendlyException("file.id.invalid");
        }

        return await fileManager.GetRemoteStreamContentAsync(fileInfoId);
    }

    public async Task<FileDto> CreateAsync(IRemoteStreamContent file)
    {
        var entity = await fileManager.CreateAsync(file);

        return ObjectMapper.Map<File, FileDto>(entity);
    }

    public async Task<FileDto> UpdateAsync(Guid id, IRemoteStreamContent file)
    {
        var entity = await fileManager.UpdateAsync(id, file);

        return ObjectMapper.Map<File, FileDto>(entity);
    }

    public async Task RemoveAsync(IEnumerable<Guid> ids)
    {
        await fileManager.RemoveAsync(ids);
    }
}