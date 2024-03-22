using System.ComponentModel.DataAnnotations;
using CloudSharp.Api.Error;

namespace CloudSharp.Api.Util;

public static class StringExtensions
{
    public static bool IsFileName(this string filename)
        => filename.IndexOfAny(Path.GetInvalidFileNameChars()) == -1 && !string.IsNullOrWhiteSpace(filename);

    public static bool IsFolderName(this string folderName)
        => folderName.IndexOfAny(Path.GetInvalidPathChars()) == -1 && !string.IsNullOrWhiteSpace(folderName);
    
    public static bool IsEmail(this string email)
    {
        var emailValidate = new EmailAddressAttribute();
        return emailValidate.IsValid(email);
    }
}