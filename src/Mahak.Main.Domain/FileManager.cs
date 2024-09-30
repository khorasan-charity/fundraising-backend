using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mahak.Main.Settings;
using Volo.Abp;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Settings;
using File = Mahak.Main.Files.File;

namespace Mahak.Main;

public class FileManager(IRepository<File, Guid> fileRepository, ISettingProvider settingProvider) : DomainService
{
    public async Task<File> CreateAsync(IRemoteStreamContent file)
    {
        // TODO: should I dispose this stream or it get disposed automatically?
        var stream = file.GetStream();
        var id = GuidGenerator.Create();
        var extension = System.IO.Path.GetExtension(file.FileName!);
        var entity = new File(id)
        {
            Name = file.FileName ?? "noname",
            Extension = extension,
            ContentType = file.ContentType,
            Size = stream.Length,
        };

        entity = await fileRepository.InsertAsync(entity, true);
        await WriteStreamAsync(id, stream);

        return entity;
    }

    public async Task<File> UpdateAsync(Guid id, IRemoteStreamContent fileStreamContent)
    {
        var file = await fileRepository.GetAsync(id);

        // TODO: should I dispose this stream or it get disposed automatically?
        var stream = fileStreamContent.GetStream();
        file.Name = fileStreamContent.FileName!;
        file.ContentType = fileStreamContent.ContentType;
        file.Extension = System.IO.Path.GetExtension(fileStreamContent.FileName!);
        file.Size = stream.Length;

        await WriteStreamAsync(id, stream);
        file = await fileRepository.UpdateAsync(file, true);

        return file;
    }

    public async Task RemoveAsync(IEnumerable<Guid> ids)
    {
        var files = await fileRepository.GetListAsync(x => ids.Contains(x.Id));

        await RemoveAsync(files);
    }

    private async Task RemoveAsync(List<File> files)
    {
        await fileRepository.DeleteManyAsync(files);

        foreach (var file in files)
        {
            System.IO.File.Delete(await GetFilePathAsync(file.Id));
        }
    }

    private async Task WriteStreamAsync(Guid id, System.IO.Stream stream)
    {
        await using var fileStream = System.IO.File.OpenWrite(await GetFilePathAsync(id));
        await stream.CopyToAsync(fileStream);

        fileStream.Close();
    }

    public async Task<string> GetFilePathAsync(Guid id)
    {
        var fileStoragePath = await settingProvider.GetOrNullAsync(MainSettings.FileStoragePath)
            ?? throw new UserFriendlyException("FileStoragePath setting is not set");
        
        return System.IO.Path.Combine(fileStoragePath, id.ToString());
    }

    public async Task<IRemoteStreamContent> GetRemoteStreamContentAsync(Guid id)
    {
        var fileInfo = await fileRepository.GetAsync(id);
        var stream = System.IO.File.OpenRead(await GetFilePathAsync(id));
        return new RemoteStreamContent(stream, fileInfo.Name, fileInfo.ContentType);
    }

    public async Task CopyFileToDirectoryAsync(Guid id, string directoryPath, bool overwrite)
    {
        var fileInfo = await fileRepository.GetAsync(id);
        System.IO.File.Copy(await GetFilePathAsync(id), System.IO.Path.Combine(directoryPath, id + fileInfo.Extension), overwrite);
    }
}