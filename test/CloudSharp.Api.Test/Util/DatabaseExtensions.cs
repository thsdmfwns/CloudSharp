using System.Data.Common;
using CloudSharp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Respawn;

namespace CloudSharp.Api.Test.Util;

public static class DatabaseExtensions
{
    public static async ValueTask<Respawner> GetRespawner(this DbConnection dbConnection)
    {
        var respawnerOptions =  new RespawnerOptions
        {
            SchemasToInclude =
            [
                "cloud_sharp"
            ],
            TablesToIgnore =
            [
                "__EFMigrationsHistory"
            ],
            DbAdapter = DbAdapter.MySql
        };
        await dbConnection.OpenAsync();
        return await Respawner.CreateAsync(dbConnection, respawnerOptions);
    }

    public static async Task MigrateAsync(this DatabaseContext databaseContext)
    {
        if((await databaseContext.Database.GetPendingMigrationsAsync()).Any()){
            await databaseContext.Database.MigrateAsync();
        }
    }
}