using CloudSharp.Data;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Api.Test.Util;

public static class DatabaseUtil
{
    public static DatabaseContext GetDatabaseContext()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        dbContextOptionsBuilder.UseMySQL(
            "server=localhost;port=11001;database=cloud_sharp;user=root;password=q1w2e3r4",
            b => b.MigrationsAssembly("CloudSharp.Migration"));
        return new DatabaseContext(dbContextOptionsBuilder.Options);
    }
}