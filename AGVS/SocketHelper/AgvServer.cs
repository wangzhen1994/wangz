using Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SocketHelper
{
    public class AgvServer
    {
        private Socket socket;
        private Action<AGV> ReceiveAGV;
        private IProtocol protocol;
        private Action<string> Message;
        private Action<Socket> NewConnect;
        private readonly object obj = new object();
        private Dictionary<ushort, AgvSession> Sessions = new Dictionary<ushort, AgvSession>();

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { get; private set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// 服务器状态
        /// </summary>
        public bool ServerState { get; private set; }

        public int SessionCount { get { return Sessions.Count; } }

        public AgvServer(Action<AGV> ReceiveAGV, IProtocol protocol, Action<string> Message)
        {
            this.ReceiveAGV = ReceiveAGV;
            this.protocol = protocol;
            this.Message = Message;

            NewConnect = new Action<Socket>(NewConnectProcess);
        }

        public void Start(int port)
        {
            try
            {
                IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
                if (ips == null || ips.Length == 0)
                {
                    ServerIP = "127.0.0.1";
                }
                else
                {
                    foreach (IPAddress ip in ips)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ServerIP = ip.ToString();
                            break;
                        }
                    }
                }
                this.ServerPort = port;
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(this.ServerIP), this.ServerPort);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ipe);
                socket.Listen(1000);

                socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);
            }
            catch (Exception e)
            {
                if (Message != null)
                {
                    Message.BeginInvoke("运行服务异常：" + e.Message, null, null);
                }
            }
        }

        public void Start(string ip, int port)
        {
            this.ServerIP = ip;
            this.ServerPort = port;
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(this.ServerIP), this.ServerPort);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipe);
            socket.Listen(1000);

            socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var s = ar.AsyncState as Socket;
                Socket client = s.EndAccept(ar);
                NewConnect.BeginInvoke(client, null, null);
                ServerState = true;

                s.BeginAccept(new AsyncCallback(AcceptCallback), s);
            }
            catch (Exception e)
            {
                ServerState = false;
                if (Message != null)
                {
                    Message.BeginInvoke("运行服务异常：" + e.Message, null, null);
                }
            }
        }

        private void NewConnectProcess(Socket obj)
        {
            try
            {
                byte[] call = protocol.Call();
                int len = obj.Send(call);
                if (len > 0)
                {
                    byte[] buffer = new byte[100];
                    int cou = obj.Receive(buffer);
                    if (cou > 0)
                    {
                        byte[] value = new byte[cou];
                        Array.Copy(buffer, 0, value, 0, value.Length);
                        AGV agv = protocol.DeCall(value);
                        AgvSession session = new AgvSession(obj, agv.ID, new Action<ushort, Exception>(AgvErrorProcess), new Action<ushort, byte[]>(ReceiveData), new Action<string>(MessageProcess));
                        if (Sessions.ContainsKey(session.ID))
                        {
                            Sessions[session.ID] = session;
                        }
                        else
                        {
                            Sessions.Add(session.ID, session);
                        }
                        ReceiveAGV.BeginInvoke(agv, null, null);
                        return;
                    }
                }
                if (Message != null)
                {
                    Message.BeginInvoke("处理连接" + obj.RemoteEndPoint.ToString() + "异常", null, null);
                }
            }
            catch (Exception e)
            {
                if (Message != null)
                {
                    Message.BeginInvoke("处理连接" + obj.RemoteEndPoint.ToString() + "异常", null, null);
                }
            }
        }

        private void MessageProcess(string obj)
        {
            try
            {
                if (Message != null)
                {
                    Message.BeginInvoke("AgvSession异常：" + obj, null, null);
                }
            }
            catch
            { }
        }

        private void ReceiveData(ushort arg1, byte[] arg2)
        {
            try
            {
                AGV agv = protocol.Report(arg1, arg2);
                ReceiveAGV.BeginInvoke(agv, null, null);
            }
            catch (Exception e)
            {
                if (Message != null)
                {
                    Message.BeginInvoke("解析实时汇报异常：" + obj, null, null);
                }
            }
        }

        private void AgvErrorProcess(ushort arg1, Exception arg2)
        {
            try
            {
               
                if (Sessions.ContainsKey(arg1))
                {
                    Sessions.Remove(arg1);
                }

                if (Message != null)
                {
                    Message.BeginInvoke(arg1.ToString() + "号小车断开连接：" + arg2.Message, null, null);
                }
            }
            catch (Exception e)
            {
                if (Message != null)
                {
                    Message.BeginInvoke("移除连接时异常：" + e.Message, null, null);
                }
            }
        }

        ~AgvServer()
        {
            this.socket.Close();
        }
    }
}
