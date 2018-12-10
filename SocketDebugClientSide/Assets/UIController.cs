using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] private InputField _inputField;
    [SerializeField] private Button _logError;
    [SerializeField] private Button _logWarning;
    [SerializeField] private Button _logMessage;

    private void OnEnable()
    {
        _logError.onClick.AddListener(()=>LogError(_inputField.text));
        _logWarning.onClick.AddListener(() => LogWarning(_inputField.text));
        _logMessage.onClick.AddListener(() => LogMessage(_inputField.text));
    }
    private void OnDisable()
    {
        _logError.onClick.RemoveAllListeners();
        _logWarning.onClick.RemoveAllListeners();
        _logMessage.onClick.RemoveAllListeners();
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void LogError(string message)
    {
        Debug.LogError(message);
    }
    public void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
    public void LogMessage(string message)
    {
        Debug.Log(message);
    }
}
