using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HadesWeb.Data;

namespace HadesWeb.Data;

public static class DbSeeder
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();
    }
}