namespace cats.Models;

public class Position
{
    public int Id { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateModified { get; set; }
    public decimal Price { get; set; }
    public int CatId { get; set; }
    public Cat Cat { get; set; }
}