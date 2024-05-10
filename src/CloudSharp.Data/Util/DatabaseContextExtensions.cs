using FluentResults;

namespace CloudSharp.Data.Util;

public static class DatabaseContextExtensions
{
    public static async Task<Result<int>> SaveChangesAsyncWithResult(this DatabaseContext databaseContext)
    {
        return await Result.Try(
            () => databaseContext.SaveChangesAsync(),
            ex => new ExceptionalError(ex));
    }
}