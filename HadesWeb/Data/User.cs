using System.ComponentModel.DataAnnotations;

namespace HadesWeb.Data;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(256)]
    public string Email { get; set; }

    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; set; }

    [Required]
    [MaxLength(100)]
    public string Role { get; set; }
}