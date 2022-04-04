using Mango.messageBus;

namespace Mango.Services.PaymentAPI.Models
{
    public class UpdatePaymentResultMessage : BaseMessage
    {
        public int OrderId { get; set; }
        public bool Status { get; set; }
    }
}
