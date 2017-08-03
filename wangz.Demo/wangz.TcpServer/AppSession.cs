
using System;
using System.Net.Sockets;
using System.Net;

namespace wangz.TcpServer
{
    public class AppSession
    {
        #region 私有字段
        private Socket client;//套接字
        private Action<string, byte[]> ReceivedDataAction;//接到一个数据后发生
        private Action<string, Exception> ExceptionAction;//触发异常后发生

        private byte[] buffer = new byte[500];//接收数据缓存

        private string _ID;
        private string _RemoteIp;
        private int _RemotePort;
        private string _LocalIp;
        private int _LocalPort;
        #endregion

        #region 公共属性
        /// <summary>
        /// 会话ID
        /// </summary>
        public string ID { get { return _ID; } }

        /// <summary>
        /// 远程服务端IP
        /// </summary>
        public string RemoteIp { get { return _RemoteIp; } }

        /// <summary>
        /// 远程服务端端口
        /// </summary>
        public int RemotePort { get { return _RemotePort; } }

        /// <summary>
        /// 本地会话IP
        /// </summary>
        public string LocalIp { get { return _LocalIp; } }

        /// <summary>
        /// 本地会话端口
        /// </summary>
        public int LocalPort { get { return _LocalPort; } }
        #endregion

        public AppSession(Socket client, Action<string, byte[]> ReceivedDataAction, Action<string, Exception> ExceptionAction)
        {
            this.client = client;
            this.ReceivedDataAction = ReceivedDataAction;
            this.ExceptionAction = ExceptionAction;

            ScanIPE(this.client.RemoteEndPoint, out this._RemoteIp, out this._RemotePort);
            ScanIPE(this.client.LocalEndPoint, out this._LocalIp, out this._LocalPort);
            this._ID = this._LocalIp;

            this.client.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), this.client);
        }

        

        /// <summary>
        /// 关闭连接并释放一切资源
        /// </summary>
        public void Disconnect()
        {
            try
            {
                client.Close();
                client.Dispose();
            }
            catch
            { }
        }

        #region 回调方法
        /// <summary>
        /// 接收数据回调方法
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                int len = socket.EndReceive(ar);
                if (len > 0)
                {
                    byte[] value = new byte[len];
                    Array.Copy(buffer, 0, value, 0, len);
                    ReceivedDataAction.BeginInvoke(this.ID, value, new AsyncCallback(ReceivedDataCallback), ReceivedDataAction);
                }
                else
                {
                    ExceptionAction.BeginInvoke(this.ID, new Exception(), new AsyncCallback(ExceptionCallback), ExceptionAction);
                }

                socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), this.client);
            }
            catch (Exception e)
            {
                ExceptionAction.BeginInvoke(this.ID, e, new AsyncCallback(ExceptionCallback), ExceptionAction);
            }
        }

        /// <summary>
        /// 接收到数据后委托回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceivedDataCallback(IAsyncResult ar)
        {
            Action<string, byte[]> action = ar.AsyncState as Action<string, byte[]>;
            action.EndInvoke(ar);
        }

        /// <summary>
        /// 异常委托回调方法
        /// </summary>
        /// <param name="ar"></param>
        private void ExceptionCallback(IAsyncResult ar)
        {
            Action<string, Exception> action = ar.AsyncState as Action<string, Exception>;
            action.EndInvoke(ar);
        }
        #endregion

        #region 私有方法
        public void ScanIPE(EndPoint ep, out string ip, out int port)
        {
            string[] ipport = ep.ToString().Split(':');
            ip = ipport[0];
            port = Convert.ToInt32(ipport[1]);
        }

        #endregion
    }
}
