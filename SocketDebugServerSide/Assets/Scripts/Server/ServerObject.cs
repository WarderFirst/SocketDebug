using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkClasses;

using UnityEngine;
using System.Collections;

namespace ServerUnity
{
    class ServerObject : MonoBehaviour
    {
        protected internal List<ClientObject> _clients = new List<ClientObject>();
        //List<TankData> _tanks = new List<TankData>();
        [SerializeField] string _ip = "192.168.87.170";
        int _port = 8888;
        TcpListener _server;

        public delegate void OnClientConnected(IClient client);
        public event OnClientConnected onClientConnected;
        private void ClientConnected(IClient client)
        {
            if (onClientConnected != null)
            {
                onClientConnected.Invoke(client);
            }
        }

        private void Awake()
        {
            Thread tcpListenerThread = new Thread(new ThreadStart(Listen));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();


            //StartCoroutine(Listen());
            //Listen();
        }
        public void Listen()
        {
            _server = new TcpListener(IPAddress.Parse(_ip), _port);
            _server.Start();
            Debug.Log("Start Listening");
            // Console.ReadLine();
            //_server = new TcpListener(IPAddress.Parse(_ip), _port);

            try
            {
                _server.Start();
                while (true)
                {
                    TcpClient tcpClient = _server.AcceptTcpClient();
                    DiagnosticClass._timer.Start();
                    Debug.Log("StartTimer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
                    Debug.Log("ClientConnect");
                    ClientObject client = new ClientObject(tcpClient, this);
                    _clients.Add(client);
                    client.Process();
                    //Thread clientThread = new Thread(new ThreadStart(client.Process));
                    //clientThread.Start();
                    //yield return new WaitForSeconds(1f);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        //public IEnumerator Listen()
        //{
        //    while(true)
        //    {
        //        using (TcpClient tcpClient = _server.AcceptTcpClient())
        //        {
        //            DiagnosticClass._timer.Start();
        //            Debug.Log("StartTimer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
        //            Debug.Log("ClientConnect");
        //            ClientObject client = new ClientObject(tcpClient, this);
        //            _clients.Add(client);
        //            client.Process();

        //        }
        //        yield return new WaitForFixedUpdate();
        //    }
        //}
        //private void FixedUpdate()
        //        {
        //            //try
        //            //{

        //            //    TcpClient tcpClient = _server.AcceptTcpClient();
        //            //    DiagnosticClass._timer.Start();
        //            //    Debug.Log("StartTimer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
        //            //    Debug.Log("ClientConnect");
        //            //    ClientObject client = new ClientObject(tcpClient, this);
        //            //    _clients.Add(client);
        //            //    client.Process();
        //            //}
        //            //catch (Exception e)
        //            //{
        //            //    Debug.Log(e);
        //            //}
        //        }
        public void BroadCastAutorisation(Message msg, string id, string playerName)
        {
            Debug.Log("Broadcast Autorisation");
            foreach (ClientObject client in _clients)
            {
                if (client._id == id)
                {
                    // msg._messageType = MessageType.System;
                    msg._methodName = "StartGame";
                    msg._methodParameters = new Parameters(playerName, id);
                    client.SendData(msg);
                    // client.SendData(msg);
                }
                else
                {

                    if (client.IsAvailable)
                    {
                        msg._methodName = "ConnectNewPlayer";
                        client.SendData(msg);
                    }

                }
            }
        }

        public void BroadCastPosition(Message msg, string id)
        {

            foreach (ClientObject client in _clients)
            {
                if (client.IsAvailable)
                {
                    if (client._id == id)
                    {
                        msg._methodName = "Move";
                    }
                    else
                    {
                        msg._methodName = "EnemyMove";
                    }
                    client.SendData(msg);
                }

            }

        }
    }
}
