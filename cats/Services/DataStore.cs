namespace cats.Models;

public static class DataStore
{
    public static List<Cat> Cats { get; set; } = new List<Cat>();
    public static List<Position> Positions { get; set; } = new List<Position>();
    public static List<User> Users { get; set; } = new List<User>();
    public static List<Role> Roles { get; set; } = new List<Role>();
}