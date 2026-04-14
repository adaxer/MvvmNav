namespace ADaxer.MvvmNav.Sample.Common.Interfaces;

public interface IFileService
{
    Task<string> GetFileAsync(string folderName, string fileName, CancellationToken cancellationToken=default);
}
