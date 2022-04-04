using Azure.Messaging.ServiceBus;
using Mango.messageBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.OrderApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {

        private readonly string serviceBusConnectionString;
        private readonly string subscriptionCheckOut;
        private readonly string checkoutMessageTopic;
        private readonly string orderPaymentProcessTopic;
        private readonly string orderUpdatePaymentResultTopic;


        private readonly OrderRepository _orderRepository;

        private ServiceBusProcessor checOutPRocessor;
        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;


        private readonly IConfiguration _configuration;
        private readonly IBaseMessage _baseMessage;

        public AzureServiceBusConsumer(OrderRepository orderRepository,
                                       IConfiguration configuration,
                                       IBaseMessage baseMessage)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _baseMessage = baseMessage;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckOut");
            checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");



            var client = new ServiceBusClient(serviceBusConnectionString);

            checOutPRocessor = client.CreateProcessor(checkoutMessageTopic, subscriptionCheckOut);
            orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionCheckOut);
        }



        public async Task Start()
        {
            checOutPRocessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checOutPRocessor.ProcessErrorAsync += ErrorHandler;
            await checOutPRocessor.StartProcessingAsync();


            orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {

            await checOutPRocessor.StopProcessingAsync();
            await checOutPRocessor.DisposeAsync();

            await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
            await orderUpdatePaymentStatusProcessor.DisposeAsync();

        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                PaymentStatus = false,
                Phone = checkoutHeaderDto.Phone,
                PickupDateTime = checkoutHeaderDto.PickupDateTime
            };

            foreach (var detailsList in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = detailsList.ProductId,
                    ProductName = detailsList.Product.Name,
                    Price = detailsList.Product.Price,
                    Count = detailsList.Count
                };

                orderHeader.CartTotalItems += detailsList.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            };


            await _orderRepository.AddOrder(orderHeader);


            PaymentRequestMessage paymentRequestMessage = new PaymentRequestMessage()
            {
                Name = orderHeader.FirstName + " " + orderHeader.LastName,
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId = orderHeader.OrderHeaderId,
                OderTotal = orderHeader.OrderTotal
            };

            try
            {
                await _baseMessage.PublishMessage(paymentRequestMessage, orderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            try
            {
                await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
