using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SocketDebug.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private ServerObject _serverObject;

        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _textPrefab;
        [SerializeField] private Text _inputText;
        [SerializeField] private Button _findLog;
        //[SerializeField] private Text _textMessages;
        [SerializeField] private Text _textIP;

        private List<Text> _textList = new List<Text>();
        private List<Text> _searchedTextElements = new List<Text>();

        private void Awake()
        {
            _textIP.text += ServerObject.GetLocalIP();
            _findLog.onClick.AddListener(Search);
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
            Text curText = Instantiate(_textPrefab, _content).GetComponent<Text>();

            curText.text += message;

            _textList.Add(curText);

            //_textMessages.text += message;
            //_textMessages.text += "\n";
        }

        public void Search()
        {
            _searchedTextElements.Clear();

            Debug.Log("Search");
            string searchText = _inputText.text;

            foreach(Text bufText in _textList)
            {
                if(bufText.text.Contains(searchText))
                {
                    Debug.Log("Contains :" + bufText.text);
                    _searchedTextElements.Add(bufText);
                }
            }

            ShowOnlyFindedText();
        }

        private void ShowOnlyFindedText()
        {
            List<Text> notConsistsElements = _textList.Where(p => !_searchedTextElements.Any(p2 => p2.text == p.text)).ToList();

            //empty finded list
            if (notConsistsElements.Count == 0)
            {
                //activate all
                foreach (var bufElement in _textList)
                {
                    bufElement.gameObject.SetActive(true);
                }
            }
            else
            {
                //hide all not mached text
                foreach (var bufElement in notConsistsElements)
                {
                    bufElement.gameObject.SetActive(false);
                }
            }
        }
    }
}
