using Bogus;

namespace CloudSharp.Api.Test.Util;

public static class FakerExtensions
{
    public static string MakeFakeFile(this Faker faker, string directoryPath, string? fileDir, string? fileName = null, string? ext = null, bool fullPath = false)
    {
        fileDir ??= ".";
        fileName ??= faker.System.CommonFileName(ext);
        var fileContent = faker.Lorem.Sentences();
        var filePath = Path.Combine(directoryPath, fileDir, fileName);
        Directory.CreateDirectory(Path.Combine(directoryPath, fileDir));
        File.WriteAllText(filePath, fileContent);
        var path = filePath.Remove(0, directoryPath.Length);
        return fullPath ? filePath : path.TrimStart('/');
    }
}