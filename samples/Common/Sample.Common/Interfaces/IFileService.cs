namespace ADaxer.MvvmNav.Sample.Common.Interfaces;

public interface IFileService
{
    Task<string> GetFileAsync(string path);
}
