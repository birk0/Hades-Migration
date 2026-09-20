using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HadesWeb.Data;
using HadesWeb.Models;
using HadesWeb.Services;
using System.Text.Json;

namespace HadesWeb.Data;

public static class DbSeeder
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();

        if (!context.Users.Any())
        {
            var jsonPath = Path.Combine(
                scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>().ContentRootPath,
                "App_Data", "users.json");

            if (File.Exists(jsonPath))
            {
                using var reader = new StreamReader(File.OpenRead(jsonPath));
                foreach (var u in JsonSerializer.Deserialize<List<UsersList>>(reader.ReadToEnd()))
                {
                    context.Users.Add(new User
                    {
                        Email = u.Email,
                        PasswordHash = u.Password,
                        Role = u.Role
                    });
                }

                context.SaveChanges();
            }
        }
    }
}