namespace StockService.Models;

public enum ProductState
{
    Free,
    Reserved
}

public class Product{
    public int Id { get; init; }
    public ProductState State { get; set; }
};