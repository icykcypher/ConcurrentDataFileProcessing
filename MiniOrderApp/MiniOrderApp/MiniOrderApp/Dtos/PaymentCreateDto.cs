namespace MiniOrderApp.Dtos;

public class PaymentCreateDto
{
        public int OrderId { get; set; }
        public float PaidAmount { get; set; }
}