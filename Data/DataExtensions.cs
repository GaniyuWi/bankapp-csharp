using System;
using Microsoft.EntityFrameworkCore;

namespace Bank_API_EF.Data;

public static class DataExtensions 
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BankContext>();
        dbContext.Database.Migrate();

    }
}
