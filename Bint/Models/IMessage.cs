using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bint.Models
{
    public interface IMessage
    {

        void SendEmail(Message _message);
        string SendSMS(Message _message);
        void SendMessage(Message _message);

    }
}
