using Microsoft.EntityFrameworkCore;
using HadesWeb.Models;

namespace HadesWeb.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public User GetByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users.ToList();
    }

    public User Create(string email, string passwordHash, string role)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            Role = role
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }
}