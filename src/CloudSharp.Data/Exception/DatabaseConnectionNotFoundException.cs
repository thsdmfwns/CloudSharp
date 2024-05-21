using System.Diagnostics;

namespace CloudSharp.Data.Exception;

public class DatabaseConnectionNotFoundException(string message, string stackTrace) : System.Exception(message)
{
    public override string StackTrace { get; } = stackTrace;
}