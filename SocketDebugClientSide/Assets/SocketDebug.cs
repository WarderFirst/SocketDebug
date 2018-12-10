//using NetworkClasses;
//using ProtoBuf;
using System;
using System.Collections;
//using System.Diagnostics;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
//using TankBehaviors;
using UnityEngine;
using UnityEngine.UI;


namespace NetworkClasses
{
    sealed class CustomizedBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = null;
            string sharedAssemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            assemblyName = Assembly.GetExecutingAssembly().FullName;
            typeName = typeName.Replace(sharedAssemblyName, assemblyName);
            returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));

            return returntype;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            base.BindToName(serializedType, out assemblyName, out typeName);
            assemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        }
    }
    public class SocketDebug : MonoBehaviour
    {

        [SerializeField]
        private string _hostIP = "192.168.87.41";
        [SerializeField]
        private int port = 8888;


        private const int _headerSize = 9;

        private TcpClient _client;
        private NetworkStream _netStream;
        private MemoryStream ms;


        void OnEnable()
        {

        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void Awake()
        {
            ConnectToServer();
        }
        private void Start()
        {

        }

        public void ConnectToServer()
        {
            _client = new TcpClient();
            _client.Connect(this._hostIP, this.port);
            if (!_client.Connected)
            {
                return;
            }
            _netStream = _client.GetStream();
            Application.logMessageReceived += HandleLog;
        }

        void HandleLog(string logString, string stackTrace, UnityEngine.LogType type)
        {
            DebugData debugdata = new DebugData()
            {
                logString = logString,
                stackTrace = stackTrace,
                type = (LogType)(int)type
            };
            //    SendData(logString);
            SendData(debugdata);
        }


        public void SendData(DebugData msg)
        {
            if (this._netStream == null)
                return;
            ms = new MemoryStream();
            // Serializer.Serialize<Message>((Stream)this._ms, (M0)msg);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new CustomizedBinder();
            bf.Serialize(ms, msg);
            ms.Position = 0;
            byte[] buffer1 = new byte[ms.Length];
            ms.Read(buffer1, 0, buffer1.Length);
            byte[] header = this.GetHeader(buffer1.Length);
            ms.Close();
            byte[] buffer2 = new byte[buffer1.Length + header.Length];
            Buffer.BlockCopy((Array)header, 0, (Array)buffer2, 0, header.Length);
            Buffer.BlockCopy((Array)buffer1, 0, (Array)buffer2, header.Length, buffer1.Length);
            _netStream.Write(buffer2, 0, buffer2.Length);
        }

        private byte[] GetHeader(int length)
        {
            string s = length.ToString();
            if (s.Length < 9)
            {
                string str = (string)null;
                for (int index = 0; index < 9 - s.Length; ++index)
                    str += "0";
                s = str + s;
            }
            UnityEngine.Debug.Log(s);
            return Encoding.Default.GetBytes(s);
        }
    }

    [System.Serializable]
    public class DebugData
    {
        public string logString;
        public string stackTrace;
        public LogType type;
    }
    [System.Serializable]
    public enum LogType
    {
        //
        // Summary:
        //     LogType used for Errors.
        Error = 0,
        //
        // Summary:
        //     LogType used for Asserts. (These could also indicate an error inside Unity itself.)
        Assert = 1,
        //
        // Summary:
        //     LogType used for Warnings.
        Warning = 2,
        //
        // Summary:
        //     LogType used for regular log messages.
        Log = 3,
        //
        // Summary:
        //     LogType used for Exceptions.
        Exception = 4
    }

}