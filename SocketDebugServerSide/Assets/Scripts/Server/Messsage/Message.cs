using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace SocketDebug.Server.Messsage
{
    public class Message : IMessage
    {
        #region IMessage

        [SerializeField] private MessageType _messageType;
        public MessageType MessageType { get { return _messageType; } }

        [SerializeField] private IClient _client;
        public IClient Client { get { return _client; } }

        [SerializeField] private string _messageText;
        public string MessageText { get { return _messageText; } }

        [SerializeField] private string _messageTime;
        public string MessageTime { get { return _messageTime; } }

        [SerializeField] private int _messageSize;
        public int MessageSize { get { return _messageSize; } }

        #endregion //IMessage

        public Message(MessageType messageType, IClient client, string messageText, string messageTime)
        {
            _messageType = messageType;
            _client = client;
            _messageText = messageText;
            _messageTime = messageTime;
            _messageSize = messageText.Length;
        }
    }
}
