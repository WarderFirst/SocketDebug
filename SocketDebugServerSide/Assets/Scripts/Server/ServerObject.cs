using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using UnityEngine;
using System.Collections;

namespace NetworkClasses
{
    public class ServerObject : MonoBehaviour
    {
        protected internal List<ClientObject> _clients = new List<ClientObject>();
        [SerializeField] string _ip = "192.168.87.170";
        int _port = 8850;
        TcpListener _server;

        public delegate void OnClientConnected(IClient client);
        public event OnClientConnected onClientConnected;
        private void ClientConnected(TcpClient client)
        {
            GameObject newClient = new GameObject();
            newClient.AddComponent<ClientObject>();
            newClient.GetComponent<ClientObject>().Init(client, this);
            Debug.Log("Client created");

            if (onClientConnected != null)
            {
                onClientConnected.Invoke(newClient.GetComponent<ClientObject>());
            }
        }

        Thread ChildThread = null;
        EventWaitHandle ChildThreadWait = new EventWaitHandle(true, EventResetMode.ManualReset);
        EventWaitHandle MainThreadWait = new EventWaitHandle(true, EventResetMode.ManualReset);

        TcpClient tcpClient = null;
        private void ChildThreadLoop()
        {
            ChildThreadWait.Reset();
            ChildThreadWait.WaitOne();

            _server = new TcpListener(IPAddress.Parse(_ip), _port);
            _server.Start();
            Debug.Log("Start Listening");

            while (true)
            {
                ChildThreadWait.Reset();

                // Do Update
                tcpClient = _server.AcceptTcpClient();
                DiagnosticClass._timer.Start();
                Debug.Log("StartTimer value=" + DiagnosticClass._timer.ElapsedMilliseconds);
                Debug.Log("ClientConnect");

                WaitHandle.SignalAndWait(MainThreadWait, ChildThreadWait);
            }
        }

        void Awake()
        {
            _ip = GetLocalIP();
            ChildThread = new Thread(ChildThreadLoop);
            ChildThread.Start();
        }


        void Update()
        {
            // Copy Results out of the thread
            if (tcpClient != null)
            {
                ClientConnected(tcpClient);
                tcpClient = null;
            }
            // Copy pending changes into the thread

            ChildThreadWait.Set();
        }
        public static string GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "0.0.0.0";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
