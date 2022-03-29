using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System.Text;

namespace Mango.messageBus
{
    public class AzureServiceBus : IBaseMessage
    {
        //Inprovisionado
        private string connectionString = "Endpoint=sb://mangdorestaurant.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=G6IDn+LvlHqQXeh2i+cfjyAbEE6Jjh+EIDhYrupMy30=";


        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            ISenderClient senderClient = new TopicClient(connectionString, topicName);

            var Jsonmessage = JsonConvert.SerializeObject(message);
            var finalMessage = new Message(Encoding.UTF8.GetBytes(Jsonmessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };


            await senderClient.SendAsync(finalMessage);


            await senderClient.CloseAsync();
        }
    }
}
