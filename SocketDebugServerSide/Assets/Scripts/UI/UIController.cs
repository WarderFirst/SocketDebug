using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ServerUnity.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private ServerObject _serverObject;

        [SerializeField] private Text _debugText;

        private void Awake()
        {
             
        }

        private void OnEnable()
        {
            _serverObject.onClientConnected += (client) => { LogMessage(client.ToString()); };
        }
        private void OnDisable()
        {
            _serverObject.onClientConnected -= (client) => { LogMessage(client.ToString()); };
        }

        public void LogMessage(string message)
        {
            _debugText.text += message;
            _debugText.text += "\n";
        }
    } 
}
