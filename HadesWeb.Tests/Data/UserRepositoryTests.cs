using Microsoft.EntityFrameworkCore;
using HadesWeb.Data;
using HadesWeb.Models;
using NUnit.Framework;

namespace HadesWeb.Tests.Data;

public class UserRepositoryTests
{
    private AppDbContext _context;
    private UserRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new UserRepository(_context);

        SeedTestData();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    private void SeedTestData()
    {
        var users = new List<User>
        {
            new User { Email = "test1@hades.htb", PasswordHash = "$2a$12$hash1", Role = "Web Users" },
            new User { Email = "test2@hades.htb", PasswordHash = "$2a$12$hash2", Role = "Administrators" },
            new User { Email = "test3@hades.htb", PasswordHash = "$2a$12$hash3", Role = "Support Team" }
        };

        _context.Users.AddRange(users);
        _context.SaveChanges();
    }

    [Test]
    public void GetByEmail_ExistingEmail_ReturnsUser()
    {
        var user = _repository.GetByEmail("test1@hades.htb");

        Assert.That(user, Is.Not.Null);
        Assert.That(user.Email, Is.EqualTo("test1@hades.htb"));
        Assert.That(user.Role, Is.EqualTo("Web Users"));
    }

    [Test]
    public void GetByEmail_NonExistingEmail_ReturnsNull()
    {
        var user = _repository.GetByEmail("nonexistent@hades.htb");

        Assert.That(user, Is.Null);
    }

    [Test]
    public void GetByEmail_CaseSensitive_ReturnsNull()
    {
        var user = _repository.GetByEmail("TEST1@hades.htb");

        Assert.That(user, Is.Null);
    }

    [Test]
    public void GetAll_ReturnsAllUsers()
    {
        var users = _repository.GetAll().ToList();

        Assert.That(users.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetAll_ReturnsCorrectUserData()
    {
        var users = _repository.GetAll().ToList();

        Assert.That(users.Any(u => u.Email == "test2@hades.htb" && u.Role == "Administrators"), Is.True);
        Assert.That(users.Any(u => u.Email == "test3@hades.htb" && u.Role == "Support Team"), Is.True);
    }
}