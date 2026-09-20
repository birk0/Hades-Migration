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

    [Test]
    public void Create_NewUser_AddsToDatabase()
    {
        var user = _repository.Create("newuser@hades.htb", "$2a$12$hash4", "Web Users");

        Assert.That(user, Is.Not.Null);
        Assert.That(user.Email, Is.EqualTo("newuser@hades.htb"));
        Assert.That(user.Role, Is.EqualTo("Web Users"));
        Assert.That(user.Id, Is.GreaterThan(0));
    }

    [Test]
    public void Create_DuplicateEmail_ThrowsException()
    {
        _repository.Create("duplicate@hades.htb", "$2a$12$hash", "Web Users");

        Assert.Throws<DbUpdateException>(() => _repository.Create("duplicate@hades.htb", "$2a$12$hash2", "Administrators"));
    }

    [Test]
    public void Create_ThenGetByEmail_ReturnsCreatedUser()
    {
        var created = _repository.Create("findme@hades.htb", "$2a$12$hash", "Support Team");
        var found = _repository.GetByEmail("findme@hades.htb");

        Assert.That(found, Is.Not.Null);
        Assert.That(found.Id, Is.EqualTo(created.Id));
        Assert.That(found.Email, Is.EqualTo(created.Email));
        Assert.That(found.Role, Is.EqualTo(created.Role));
    }
}