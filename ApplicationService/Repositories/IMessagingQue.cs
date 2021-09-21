namespace ApplicationService.Repositories
{
    public interface IMessagingQue
    {
        //void SendMessage(string exchange, string message);
        string SendMessageWithResponse(string exchange, string message);
    }
}