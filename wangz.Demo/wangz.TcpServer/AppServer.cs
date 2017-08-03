﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace wangz.TcpServer
{
    public class AppServer
    {
        #region 私有字段
        private Socket server;//通信套接字
        private EndPoint localEp;//本地终结点
        private IPEndPoint localIpe;//本地终结点

        private Action<Socket> NewSessionConnectedAction;//新的客户端连接后
        private Action<string, byte[]> ReceivedDataAction;//接到一条数据后
        private Action<string, Exception> SessionExceptionAction;//会话发生异常后
        private event Action<Exception> _ServerExceptionEvent;//服务器发生异常后

        private Dictionary<string, AppSession> _sessions = new Dictionary<string, AppSession>();//会话字典
        #endregion

        #region 公共属性
        /// <summary>
        /// 服务器发生异常后
        /// </summary>
        public event Action<Exception> ServerExceptionEvent
        {
            add { _ServerExceptionEvent += value; }
            remove
            {
                if (_ServerExceptionEvent != null)
                {
                    _ServerExceptionEvent -= value;
                }
            }
        }

        /// <summary>
        /// 本地IP
        /// </summary>
        public string LocalIp { get; private set; }

        /// <summary>
        /// 本地端口
        /// </summary>
        public int LocalPort { get; private set; }

        /// <summary>
        /// 会话字典
        /// </summary>
        public Dictionary<string, AppSession> Sessions
        {
            get { return _sessions; }
            private set { _sessions = value; }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 用指定的端口初始化wangz.TcpServer.AppServer的新实例
        /// 使用默认本地IPV4地址，若不存在，则使用“127.0.0.1”
        /// </summary>
        /// <param name="localPort">端口</param>
        public AppServer(int localPort)
        {
            this.LocalIp = GetLocalIPV4();//初始化IP
            this.LocalPort = localPort;//初始化端口

            this.localIpe = new IPEndPoint(IPAddress.Parse(this.LocalIp), this.LocalPort);//初始化终结点
            this.localEp = this.localIpe;//初始化终结点

            NewSessionConnectedAction = new Action<Socket>(NewSessionConnected);//初始化连接委托
            ReceivedDataAction = new Action<string, byte[]>(ReceivedData);//初始化接收数据委托
            SessionExceptionAction = new Action<string, Exception>(SessionException);//初始化会话异常委托
        }

        /// <summary>
        /// 使用指定的IP和端口初始化wangz.TcpServer.AppServer的新实例
        /// </summary>
        /// <param name="localIp">IP</param>
        /// <param name="localPort">端口</param>
        public AppServer(string localIp, int localPort)
        {
            this.LocalIp = localIp;//初始化IP
            this.LocalPort = localPort;//初始化端口

            this.localIpe = new IPEndPoint(IPAddress.Parse(this.LocalIp), this.LocalPort);//初始化终结点
            this.localEp = this.localIpe;//初始化终结点

            NewSessionConnectedAction = new Action<Socket>(NewSessionConnected);//初始化连接委托
            ReceivedDataAction = new Action<string, byte[]>(ReceivedData);//初始化接收数据委托
            SessionExceptionAction = new Action<string, Exception>(SessionException);//初始化会话异常委托
        }

        /// <summary>
        /// 使用指定的IP、端口和异常处理方法初始化wangz.TcpServer.AppServer的新实例
        /// </summary>
        /// <param name="localIp">IP</param>
        /// <param name="localPort">端口</param>
        /// <param name="ServerExceptionEvent">异常处理</param>
        public AppServer(string localIp, int localPort, Action<Exception> ServerExceptionEvent)
        {
            this.LocalIp = localIp;//初始化IP
            this.LocalPort = localPort;//初始化端口
            this.ServerExceptionEvent += ServerExceptionEvent;//初始化异常处理

            this.localIpe = new IPEndPoint(IPAddress.Parse(this.LocalIp), this.LocalPort);//初始化终结点
            this.localEp = this.localIpe;//初始化终结点

            NewSessionConnectedAction = new Action<Socket>(NewSessionConnected);//初始化连接委托
            ReceivedDataAction = new Action<string, byte[]>(ReceivedData);//初始化接收数据委托
            SessionExceptionAction = new Action<string, Exception>(SessionException);//初始化会话异常委托
        }
        #endregion

        #region 操作方法
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns>启动成功返回true</returns>
        public bool Start()
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//初始化套接字
                server.Bind(this.localIpe);//绑定终结点
                server.Listen(0);//开始监听
            }
            catch (Exception e)
            {
                if (_ServerExceptionEvent != null)
                {
                    _ServerExceptionEvent.BeginInvoke(e, new AsyncCallback(ServerExceptionCallback), _ServerExceptionEvent);
                }
                return false;
            }

            server.BeginAccept(new AsyncCallback(AcceptCallback), server);//开始异步接受连接
            return true;
        }
        #endregion

        #region 回调方法
        /// <summary>
        /// 接受连接回调方法
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                Socket client = socket.EndAccept(ar);//接受一个新的连接

                NewSessionConnectedAction.BeginInvoke(client, new AsyncCallback(NewSessionConnectedCallback), NewSessionConnectedAction);//调用新建连接委托

                socket.BeginAccept(new AsyncCallback(AcceptCallback), server);//开始异步接受连接
            }
            catch (Exception e)
            {
                if (_ServerExceptionEvent != null)
                {
                    _ServerExceptionEvent.BeginInvoke(e, new AsyncCallback(ServerExceptionCallback), _ServerExceptionEvent);
                }
            }
        }

        /// <summary>
        /// 新建连接委托回调方法
        /// </summary>
        /// <param name="ar"></param>
        private void NewSessionConnectedCallback(IAsyncResult ar)
        {
            Action<Socket> action = ar.AsyncState as Action<Socket>;
            action.EndInvoke(ar);
        }

        /// <summary>
        /// 服务器异常事件回调方法
        /// </summary>
        /// <param name="ar"></param>
        private void ServerExceptionCallback(IAsyncResult ar)
        {
            Action<Exception> action = ar.AsyncState as Action<Exception>;
            action.EndInvoke(ar);
        }
        #endregion

        #region 委托绑定的方法
        /// <summary>
        /// 会话异常后发生
        /// </summary>
        /// <param name="arg1">发生异常的会话ID</param>
        /// <param name="arg2">发生的异常</param>
        private void SessionException(string arg1, Exception arg2)
        {
            try
            {
                if (this._sessions.ContainsKey(arg1))
                {
                    this._sessions[arg1].Disconnect();//关闭连接
                    this._sessions.Remove(arg1);//从字典移除
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 接收到数据后发生
        /// </summary>
        /// <param name="arg1">接收数据的会话ID</param>
        /// <param name="arg2">接收到的数据</param>
        private void ReceivedData(string arg1, byte[] arg2)
        {
            try
            {

            }
            catch
            { }
        }

        /// <summary>
        /// 建立新的连接后发生
        /// </summary>
        /// <param name="obj">新的连接的套接字</param>
        private void NewSessionConnected(Socket obj)
        {
            try
            {

            }
            catch
            { }
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 获取本地IPV4地址
        /// </summary>
        /// <returns></returns>
        private string GetLocalIPV4()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            if (ips != null && ips.Length > 0)
            {
                foreach (IPAddress ip in ips)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            return "127.0.0.1";
        }

        #endregion
    }
}