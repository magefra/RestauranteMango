namespace Mango.Services.PaymentAPI.Models
{
    public class UpdatePaymentResultMessage
    {
        public int OrderId { get; set; }
        public bool Status { get; set; }
    }
}
