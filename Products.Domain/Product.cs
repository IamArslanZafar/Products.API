namespace Products.Domain;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Colour { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
