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
using NetworkClasses;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections;

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

namespace ServerUnity {

   

    class ClientObject :MonoBehaviour, IClient {

     
        int headerSize = 9;
        int _totalSize;

        ServerObject _server;
        TcpClient _client;
        NetworkStream _stream;
        protected internal TankData _tankData;
        BinaryFormatter _bf;
        MemoryStream _ms;
        Message _message;

        public delegate void OnMessageRecived(IClient client, Message message);
        public event OnMessageRecived onMessageRecived;
        private void MessageRecived(IClient client, Message message)
        {
            if(onMessageRecived != null)
            {
                onMessageRecived.Invoke(client, message);
            }
        }

        protected internal string _id;
        string _playerName;


        bool _isAvailable;
        public bool IsAvailable  {
            get {
                return _isAvailable;
            }

            set {
                _isAvailable = value;
            }
        }

        public ClientObject(TcpClient client,ServerObject server) {
            _client = client;
            _stream = client.GetStream();
            _tankData = new TankData();
            _id = Guid.NewGuid().ToString();
            _server = server;

        }

        protected internal void Process() {
            //while (true) {
        
            //    ReadData();
            //    yield return new WaitForSeconds(1f);
            //}
        }

        void ReadData() {
            try {
               // Console.WriteLine("Start Read data timer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
                //   _bf = new BinaryFormatter();
                _ms = new MemoryStream();
                byte[] headerBuffer = new byte[headerSize];
             //   Console.WriteLine("Before First read Stream value=" + DiagnosticClass._timer.ElapsedMilliseconds);
                _stream.Read(headerBuffer, 0, headerBuffer.Length);
         
                string header = Encoding.Default.GetString(headerBuffer);
                Console.WriteLine(header);
                int bufferLenght = int.Parse(header);
              //  Console.WriteLine(bufferLenght);
                byte[] dataBuffer = new byte[bufferLenght];
              //  Console.WriteLine("Before second read Stream value=" + DiagnosticClass._timer.ElapsedMilliseconds);
                _stream.Read(dataBuffer,0,dataBuffer.Length);

                // _totalSize = headerSize + dataBuffer.Length;
                //foreach (byte b in dataBuffer) {
                //    Console.WriteLine(b);
                //}
             //   Console.WriteLine("Befor Write Stream value=" + DiagnosticClass._timer.ElapsedMilliseconds);
                _ms.Write(dataBuffer,0,dataBuffer.Length);
                _ms.Position = 0;
                //  Object obj = _bf.Deserialize(_ms);// as TankData;
                //  Message msg= Serializer.Deserialize<Message>(_ms);
                Console.WriteLine(_ms.Length);
                //  Console.WriteLine("Before Serialize Message timer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Binder = new CustomizedBinder();
                //   bf.Deserialize(_ms);
                var dData = new DebugData();
                dData = bf.Deserialize(_ms) as DebugData;
                Console.WriteLine(dData.logString);
                Console.WriteLine(dData.type);
              //  _message = Serializer.Deserialize<Message>(_ms);
              //  Console.WriteLine(_message._methodParameters._types[1]);
              //  Console.WriteLine("After Serialize Message timer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
            //    ManageMessage(_message);
                // _tankData = _bf.Deserialize(_ms) as TankData;
                // byte[] dataBuffer = new byte[128];
                //  _stream.Read(dataBuffer,0,128);
                _ms.Close();
               // Console.WriteLine(msg._messageType);
                //Console.WriteLine("x={0}, y={1}, z={2}, str={3}",_tankData.x,_tankData.y,_tankData.z,_tankData.a);
                //Console.WriteLine("MessageType={0}, ID={1}, MethodName={2}, TankPosX={3}", _message._messageType,
                //                                                            _message._Id, 
                //                                                            _message._methodName,
                //                                                            _message._methodParameters._tankParams.x);
              //  SendData();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }

            

        }

        void ManageMessage(Message msg) {
            if (msg._messageType==MessageType.Game) {
                GameMessage(msg);
            }

        }
        void GameMessage(Message msg) {
            if (msg._Id!=null) {
              //  _id = msg._Id;
            }
          var method=this.GetType().GetMethod(msg._methodName);
            Console.WriteLine(msg._methodName);
          //  Console.WriteLine(msg._methodParameters._types[0]);
          //  Console.WriteLine(msg._methodParameters.GetParams()[1]);
           // Console.WriteLine();
            method.Invoke(this,msg._methodParameters.GetParams());

        }

   

        protected internal void SendData(Message msg) {
            Console.WriteLine(msg._messageType+"Send DataMessage");
            Console.WriteLine("SendData");
           // Console.WriteLine(msg._methodParameters._types[0]);
            MemoryStream ms = new MemoryStream();
            //   msg._methodParameters._types[0] = typeof(Parameters);
          //  msg._methodParameters = new Parameters(1f,2f,3f);
          //  Console.WriteLine(msg._methodParameters._types[0]);
          //  Console.WriteLine(msg._methodParameters.GetParams());
            Serializer.Serialize(ms,msg);
            ms.Position = 0;

          // Console.WriteLine(Serializer.Deserialize<Message>(ms)._methodParameters._types[0]);

            byte[] databuffer = new byte[ms.Length];
            
            int bytes = ms.Read(databuffer,0,databuffer.Length);
            ms.Close();
            byte[] header = GetHeader(databuffer.Length);
            byte[] totalBuffer = new byte[header.Length + databuffer.Length];
            

            Buffer.BlockCopy(header,0,totalBuffer,0,header.Length);
            Buffer.BlockCopy(databuffer,0,totalBuffer,header.Length,databuffer.Length);

            //foreach (byte b in totalBuffer) {

            //    Console.WriteLine(b);
            //}
            Console.WriteLine("End Message timer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
            DiagnosticClass._timer.Reset();
            DiagnosticClass._timer.Start();
            _stream.Write(totalBuffer,0,totalBuffer.Length);
            
             
        }
        private byte[] GetHeader(int length) {//Header create method (Header lenght-const=9)
            string header = length.ToString();
            if (header.Length < 9) {
                string zeros = null;
                for (int i = 0; i < (9 - header.Length); i++) {
                    zeros += "0";
                }
                header = zeros + header;
            }

            byte[] byteheader = Encoding.Default.GetBytes(header);


            return byteheader;
        }

        public void Autorization(string playerName) {//Check for public for reflect call
            _tankData._id = _id;
            _tankData._name = playerName;
            _playerName = playerName;
            Message msg = new Message(MessageType.System, _id, "ConnectNewPlayer", new Parameters(_tankData._name,
                                                        _tankData._level,
                                                        _tankData.x,
                                                        _tankData.y,
                                                        _tankData.z));
            _server.BroadCastAutorisation(msg, _id,playerName);

        }

        public void ConstructWorld() {//build vorld for client after virification
          //  Console.WriteLine("ConstructWorld");
            foreach (ClientObject client in _server._clients) {//select objects for build world on client-side
                if (client!=null && client!=this) {
                    SendData(new Message(MessageType.System,
                                         client._tankData._id,
                                         "ConnectNewPlayer",
                                         new Parameters(client._tankData._name,
                                                        client._tankData._level,
                                                        client._tankData.x,
                                                        client._tankData.y,
                                                        client._tankData.z)));
                }
            }
            IsAvailable = true;
            Console.WriteLine(IsAvailable);
        }
        public void OnOffAvailable(bool value) {
            IsAvailable = value;
        }
        public void Move(float x,float y,float z) {
         //   Console.WriteLine("x= {0} y= {1} z= {2}",x,y,z);
            _tankData.x = x;
            _tankData.y = y;
            _tankData.z = z;
            _message._messageType = MessageType.Game;
            _message._Id = _id;
            _message._methodName = "Move";
            _message._methodParameters._floatParams[0] = x;
            _message._methodParameters._floatParams[1] = y;
            _message._methodParameters._floatParams[2] = z;

            _server.BroadCastPosition(_message,_id);
            //  Console.WriteLine(tankData.GetType());

        }

        #region //////////////Test region//////////
        public void FloatTest(float x,float y,float z) {//Check for public for reflect call
           // Console.WriteLine("jshdkgjhskdg");
            Console.WriteLine("{0} {1} {2}",x,y,z);

        }

        public void IntTest(int a,int b) {
            Console.WriteLine(" Integer {0} {1}",a,b);
        }
        public void StringTest(string a, string b,string c) {//Check for public for reflect call
                                                          // Console.WriteLine("jshdkgjhskdg");
            Console.WriteLine("{0} {1} {2}", a, b, c);

        }

        public void StringFloatTest(string a,string b,float x,float y) {
            Console.WriteLine("{0} {1} {2} {3}", a, b, x,y);

        }

        public void IntFloatStringTest(int a, float b,string c) {
            Console.WriteLine("IntFloatStringTest {0} {1} {2}", a, b, c);

        }

        public void IntFloatStringBoolTest(int a, float b, string c,bool d) {
            Console.WriteLine("IntFloatStringBoolTest {0} {1} {2} {3}", a, b, c,d);

        }

        #endregion
    }

    sealed class CustomizedBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName) {
            Type returntype = null;
            string sharedAssemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            assemblyName = Assembly.GetExecutingAssembly().FullName;
            typeName = typeName.Replace(sharedAssemblyName, assemblyName);
            returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));

            return returntype;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName) {
            base.BindToName(serializedType, out assemblyName, out typeName);
            assemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        }
    }
}
