using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketDebug.Server.Messsage
{
    public interface IMessage
    {

        MessageType MessageType { get; }
        IClient Client { get; }
        string MessageText { get; }
        string MessageTime { get; }
        int MessageSize { get; }
    }

    /// <summary> Describe different types of messages </summary>
    [System.Serializable]
    public enum MessageType
    {
        System,
        Error,
        Assert,
        Warning,
        Log,
        Exception
    }
}
