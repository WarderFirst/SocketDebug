using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using NetworkClasses;
using ProtoBuf;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections;
using System.Threading;
using SocketDebug.Server.Messsage;

//namespace NetworkClasses {
//    [System.Serializable]
//    [ProtoContract]
//    class TankData {
//        [ProtoMember(1)]
//        public float x;
//        [ProtoMember(2)]
//        public float y;
//        [ProtoMember(3)]
//        public float z;
//        [ProtoMember(4)]
//        public string str;
//        [ProtoMember(5)]
//        public int? a = null;

//        public TankData() {

//        }
//        //public  void ConvertPosition(Vector3 pos) {
//        //    x = pos.x;
//        //    y = pos.y;
//        //    z = pos.z;
//        //}

//    }

//}

namespace SocketDebug
{
   public class ClientObject : MonoBehaviour, IClient
    {


        int headerSize = 9;
        int _totalSize;

        ServerObject _server;
        [SerializeField] private TcpClient _client;
        public TcpClient TcpClient { get { return _client; } }
        NetworkStream _stream;
        //protected internal TankData _tankData;
        BinaryFormatter _bf;
        MemoryStream _ms;
        //Message1 _message;

        //public delegate void OnMessageRecived(Message message);
        public event OnMessageRecived onMessageRecived;
        private void MessageRecived(Message message)
        {
            if (onMessageRecived != null)
            {
                onMessageRecived.Invoke(message);
            }
        }

        protected internal string _id;
        string _playerName;


        [SerializeField] private bool _isAvailable;
        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }

            set
            {
                _isAvailable = value;
            }
        }

        //public ClientObject(TcpClient client,ServerObject server) {
        //    _client = client;
        //    _stream = client.GetStream();
        //    _tankData = new TankData();
        //    _id = Guid.NewGuid().ToString();
        //    _server = server;

        //}

        //protected internal void Process() {
        //    //while (true) {

        //    //    ReadData();
        //    //    yield return new WaitForSeconds(1f);
        //    //}
        //}

        //void ReadData() {
        //    try {
        //       // Console.WriteLine("Start Read data timer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
        //        //   _bf = new BinaryFormatter();
        //        _ms = new MemoryStream();
        //        byte[] headerBuffer = new byte[headerSize];
        //     //   Console.WriteLine("Before First read Stream value=" + DiagnosticClass._timer.ElapsedMilliseconds);
        //        _stream.Read(headerBuffer, 0, headerBuffer.Length);

        //        string header = Encoding.Default.GetString(headerBuffer);
        //        Console.WriteLine(header);
        //        int bufferLenght = int.Parse(header);
        //      //  Console.WriteLine(bufferLenght);
        //        byte[] dataBuffer = new byte[bufferLenght];
        //      //  Console.WriteLine("Before second read Stream value=" + DiagnosticClass._timer.ElapsedMilliseconds);
        //        _stream.Read(dataBuffer,0,dataBuffer.Length);

        //        // _totalSize = headerSize + dataBuffer.Length;
        //        //foreach (byte b in dataBuffer) {
        //        //    Console.WriteLine(b);
        //        //}
        //     //   Console.WriteLine("Befor Write Stream value=" + DiagnosticClass._timer.ElapsedMilliseconds);
        //        _ms.Write(dataBuffer,0,dataBuffer.Length);
        //        _ms.Position = 0;
        //        //  Object obj = _bf.Deserialize(_ms);// as TankData;
        //        //  Message msg= Serializer.Deserialize<Message>(_ms);
        //        Console.WriteLine(_ms.Length);
        //        //  Console.WriteLine("Before Serialize Message timer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
        //        BinaryFormatter bf = new BinaryFormatter();
        //        bf.Binder = new CustomizedBinder();
        //        //   bf.Deserialize(_ms);
        //        var dData = new DebugData();
        //        dData = bf.Deserialize(_ms) as DebugData;
        //        Console.WriteLine(dData.logString);
        //        Console.WriteLine(dData.type);
        //      //  _message = Serializer.Deserialize<Message>(_ms);
        //      //  Console.WriteLine(_message._methodParameters._types[1]);
        //      //  Console.WriteLine("After Serialize Message timer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
        //    //    ManageMessage(_message);
        //        // _tankData = _bf.Deserialize(_ms) as TankData;
        //        // byte[] dataBuffer = new byte[128];
        //        //  _stream.Read(dataBuffer,0,128);
        //        _ms.Close();
        //       // Console.WriteLine(msg._messageType);
        //        //Console.WriteLine("x={0}, y={1}, z={2}, str={3}",_tankData.x,_tankData.y,_tankData.z,_tankData.a);
        //        //Console.WriteLine("MessageType={0}, ID={1}, MethodName={2}, TankPosX={3}", _message._messageType,
        //        //                                                            _message._Id, 
        //        //                                                            _message._methodName,
        //        //                                                            _message._methodParameters._tankParams.x);
        //      //  SendData();
        //    }
        //    catch (Exception e) {
        //        Console.WriteLine(e);
        //    }



        //}

        //void ManageMessage(Message msg) {
        //    if (msg._messageType==MessageType.Game) {
        //        GameMessage(msg);
        //    }

        //}
        //void GameMessage(Message msg) {
        //    if (msg._Id!=null) {
        //      //  _id = msg._Id;
        //    }
        //  var method=this.GetType().GetMethod(msg._methodName);
        //    Console.WriteLine(msg._methodName);
        //  //  Console.WriteLine(msg._methodParameters._types[0]);
        //  //  Console.WriteLine(msg._methodParameters.GetParams()[1]);
        //   // Console.WriteLine();
        //    method.Invoke(this,msg._methodParameters.GetParams());

        //}

        Thread ChildThread = null;
        EventWaitHandle ChildThreadWait = new EventWaitHandle(true, EventResetMode.ManualReset);
        EventWaitHandle MainThreadWait = new EventWaitHandle(true, EventResetMode.ManualReset);

        private Message newMessage = null;
        void ChildThreadLoop()
        {
            ChildThreadWait.Reset();
            ChildThreadWait.WaitOne();

            while (true)
            {
                ChildThreadWait.Reset();

                // Do Update
                _ms = new MemoryStream();
                byte[] headerBuffer = new byte[headerSize];

                _stream.Read(headerBuffer, 0, headerBuffer.Length);

                string header = Encoding.Default.GetString(headerBuffer);

                int bufferLenght = int.Parse(header);

                byte[] dataBuffer = new byte[bufferLenght];

                _stream.Read(dataBuffer, 0, dataBuffer.Length);

                _ms.Write(dataBuffer, 0, dataBuffer.Length);
                _ms.Position = 0;

                Console.WriteLine(_ms.Length);

                BinaryFormatter bf = new BinaryFormatter();
                bf.Binder = new CustomizedBinder();

                var dData = new DebugData();
                dData = bf.Deserialize(_ms) as DebugData;
                newMessage = new Message(dData.type,this, dData.logString, System.DateTime.Now.ToString());

                _ms.Close();

                WaitHandle.SignalAndWait(MainThreadWait, ChildThreadWait);
            }
        }

        void Awake()
        {
            ChildThread = new Thread(ChildThreadLoop);
            ChildThread.Start();
        }

        void Update()
        {

            if (newMessage != null)
            {

                MessageRecived(newMessage);
                //Debug.Log(message);
                newMessage = null;
            }

            ChildThreadWait.Set();
        }
        public void Init(TcpClient client, ServerObject server)
        {
            //    _client = client;
            //    _stream = client.GetStream();
            //    _tankData = new TankData();
            //    _id = Guid.NewGuid().ToString();
            //    _server = server;
            _client = client;
            _stream = client.GetStream();
            _server = server;
        }
    }

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
}
