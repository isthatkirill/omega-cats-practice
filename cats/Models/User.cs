namespace cats.Models;

public class User
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
}