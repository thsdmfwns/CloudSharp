using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CloudSharp.Api.Error;

namespace CloudSharp.Api.Util;

public static partial class StringExtensions
{
    [GeneratedRegex(@"^(?=.{2,32}$)(?!(?:everyone|here)$)\.?[a-z0-9_]+(?:\.[a-z0-9_]+)*\.?$")]
    private static partial Regex MemberNameRegex();
    
    public static bool IsFileName(this string filename)
        =>  !string.IsNullOrWhiteSpace(filename)
            && filename.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;

    public static bool IsFolderName(this string folderName)
        =>  !string.IsNullOrWhiteSpace(folderName)
            && folderName.IndexOfAny(Path.GetInvalidPathChars()) == -1;
    
    public static bool IsEmail(this string email)
    {
        var emailValidate = new EmailAddressAttribute();
        return emailValidate.IsValid(email);
    }

    public static bool IsMemberName(this string name)
        => !string.IsNullOrWhiteSpace(name)
           && MemberNameRegex().IsMatch(name);
}
    