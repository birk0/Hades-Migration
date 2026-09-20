using HadesWeb.Models;

namespace HadesWeb.Data;

public interface IUserRepository
{
    User GetByEmail(string email);
    IEnumerable<User> GetAll();
    User Create(string email, string passwordHash, string role);
}