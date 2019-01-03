using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkClasses.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private ServerObject _serverObject;

        [SerializeField] private Text _textMessages;
        [SerializeField] private Text _textIP;
        private void Awake()
        {
            _textIP.text += ServerObject.GetLocalIP();
        }

        private void OnEnable()
        {
            _serverObject.onClientConnected += (client) =>
            {
                client.onMessageRecived += (message) =>{ LogMessage(message.MessageType + " : " + message.MessageText + " -----" + message.MessageTime); };
                LogMessage(client.ToString());
            };
        }
        private void OnDisable()
        {
            _serverObject.onClientConnected -= (client) => { LogMessage(client.ToString()); };
        }

        public void LogMessage(string message)
        {
            _textMessages.text += message;
            _textMessages.text += "\n";
        }
    }
}
