namespace PaymentService.Models;

public class User
{
    public Guid Id { get; set; }
    
    public bool CanPay { get; set; }
}