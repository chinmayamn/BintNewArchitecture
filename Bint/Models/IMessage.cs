namespace Bint.Models
{
    public interface IMessage
    {

        void SendEmail(Message message);
        string SendSms(Message message);
        void SendMessage(Message message);

    }
}
