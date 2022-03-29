namespace Mango.messageBus
{
    public interface IBaseMessage
    {
        Task PublishMessage(BaseMessage message, string topicName);

    }
}