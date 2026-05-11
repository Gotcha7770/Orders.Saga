namespace PaymentService.Models;

public class User
{
    public Guid Id { get; init; }
    
    public bool CanPay { get; init; }
}