using Bogus;

namespace CloudSharp.Api.Test.Util;

public static class FakerExtensions
{
    public static string MakeFakeFile(this Faker faker, string DirectoryPath, string? fileDir, string? ext = null, bool fullPath = false)
    {
        fileDir ??= string.Empty;
        var fileName = faker.System.CommonFileName(ext);
        var fileContent = faker.Lorem.Sentences();
        var filePath = Path.Combine(DirectoryPath, fileDir, fileName);
        Directory.CreateDirectory(Path.Combine(DirectoryPath, fileDir));
        File.WriteAllText(filePath, fileContent);
        var path = filePath.Remove(0, DirectoryPath.Length);
        return fullPath ? filePath : path.TrimStart('/');
    }
}