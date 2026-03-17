using System;
using System.Collections.Generic;
using System.Text;
using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace ADaxer.MvvmNav.Sample.Common;

public class FileService : IFileService
{
    public async Task<string> GetFileAsync(string path)
    {
        return await File.ReadAllTextAsync(path);
    }
}
