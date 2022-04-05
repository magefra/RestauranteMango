using Mango.messageBus;

namespace Mango.Services.OrderApi.Messages
{
    public class PaymentRequestMessage : BaseMessage
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryMonthYear { get; set; }
        public double OderTotal { get; set; }

        public string Email { get; set; }
    }
}
