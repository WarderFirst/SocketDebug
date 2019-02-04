using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using SocketDebug.Server.Messsage;

namespace SocketDebug
{
    public interface IClient
    {
        TcpClient TcpClient { get; }
        bool IsAvailable { get; set; }

        void Init(TcpClient client, ServerObject server);

        event OnMessageRecived onMessageRecived;
    }

    public delegate void OnMessageRecived(Message message);
}
