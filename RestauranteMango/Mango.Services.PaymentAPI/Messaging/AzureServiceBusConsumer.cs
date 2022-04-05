using Azure.Messaging.ServiceBus;
using Mango.messageBus;
using Mango.Services.PaymentAPI.Models;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {

        private readonly string serviceBusConnectionString;
        private readonly string subscriptionPayment;
        private readonly string orderPaymentProcessTopic;
        private readonly string orderUpdatePaymentResultTopic;

        private ServiceBusProcessor checOutPRocessor;
        private readonly IProcessPayment _processPayment;
        private readonly IConfiguration _configuration;
        private readonly IBaseMessage _baseMessage;

        public AzureServiceBusConsumer(IProcessPayment processPayment,
                                       IConfiguration configuration,
                                       IBaseMessage baseMessage)
        {
            _processPayment = processPayment;
            _configuration = configuration;
            _baseMessage = baseMessage;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionPayment = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
            orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);

            checOutPRocessor = client.CreateProcessor(orderPaymentProcessTopic, subscriptionPayment);
        }



        public async Task Start()
        {
            checOutPRocessor.ProcessMessageAsync += ProcessPayments;
            checOutPRocessor.ProcessErrorAsync += ErrorHandler;
            await checOutPRocessor.StartProcessingAsync();
        }

        public async Task Stop()
        {

            await checOutPRocessor.StopProcessingAsync();
            await checOutPRocessor.DisposeAsync();

        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task ProcessPayments(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);


            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email = paymentRequestMessage.Email
            };

            try
            {
                await _baseMessage.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception e)
            {
                throw;
            }


        }
    }
}
