using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace KittyHelpYouOut
{
    public class KittyUDPClient
    {
        private UdpClient UDPMessageReceiver;
        private IPEndPoint IPED;
        public string IP;
        public string Port;
        private bool IsListening=false;
        public bool OnOff => IsListening;
        private bool IsInited = false;
        private bool Enable = true;
        // public delegate void OnReceiveHandle(string raw,out T result);
        private Action<string> OnReceiveCallback;

        public KittyUDPClient(string ip, string port)
        {
            IP = ip;
            Port = port;
        }
        private KittyUDPClient()
        {
        }

        public void Init(Action<string> callback)
        {
            OnReceiveCallback = callback;
            IsInited = true;
            IPED = new IPEndPoint(IPAddress.Any, int.Parse(Port));
            UDPMessageReceiver = new UdpClient(IPED);
            Loom.RunAsync(ReceiveMessage);
        }
        
        public void StartListen()
        {
            if (IsInited)
            {
                IsListening = true;
            }
            else
            {
                Debug.LogError("KittyUDP not Inited!");
            }
        }
        public void StopListen()
        {
            if (IsListening)
            {
                IsListening = false;
            }
        }
        public void DiscardClient()
        {
            IsListening = false;
            Enable = false;
            UDPMessageReceiver.Dispose();
        }
        private void ReceiveMessage()
        {
            while (Enable)
            {
                if (IsListening)
                {
                    byte[] receivedBytes = UDPMessageReceiver.Receive(ref IPED);
                    if (receivedBytes.Length>0)//如果接受信号长度不为0，进入信号内容判断
                    {
                        var newString= Encoding.UTF8.GetString(receivedBytes);
                        Loom.QueueOnMainThread(e =>
                        {
                            if(IsListening)OnReceiveCallback?.Invoke((string)e);
                        },newString);
                    }
                }
            }
        }
    }
}