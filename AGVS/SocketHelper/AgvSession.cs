using System;
using System.Net.Sockets;

namespace SocketHelper
{
    public class AgvSession
    {
        private Socket socket;
        private Action<ushort, Exception> AgvError;
        private Action<ushort, byte[]> ReceiveData;
        private Action<string> MessageAction;
        private byte[] Buffer = new byte[500];

        /// <summary>
        /// 小车IP
        /// </summary>
        public string IP { get; private set; }

        /// <summary>
        /// 小车编号
        /// </summary>
        public ushort ID { get; private set; }

        /// <summary>
        /// 创建一个AgvSession的实例
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="id"></param>
        /// <param name="AgvError"></param>
        /// <param name="ReceiveData"></param>
        /// <param name="MessageAction"></param>
        public AgvSession(Socket socket, ushort id, Action<ushort, Exception> AgvError, Action<ushort, byte[]> ReceiveData, Action<string> MessageAction)
        {
            this.socket = socket;
            this.IP = socket.RemoteEndPoint.ToString().Split(':')[0];
            this.ID = id;
            this.AgvError = AgvError;
            this.ReceiveData = ReceiveData;
            this.MessageAction = MessageAction;

            this.socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), this.socket);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool Send(byte[] buffer)
        {
            try
            {
                int cou = this.socket.Send(buffer);
                return cou > 0;
            }
            catch (SocketException se)
            {
                AgvError.Invoke(this.ID, se);
                return false;
            }
            catch (Exception e)
            {
                MessageAction.Invoke("向" + IP + "发送数据异常：" + e.Message);
                return false;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var s = ar.AsyncState as Socket;
                int cou = s.EndReceive(ar);
                if (cou > 0)
                {
                    byte[] value = new byte[cou];
                    Array.Copy(Buffer, 0, value, 0, value.Length);
                    ReceiveData.BeginInvoke(this.ID, value, null, null);

                    s.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), s);
                }
                else
                {
                    AgvError.Invoke(this.ID, new SocketException());
                    return;
                }
            }
            catch (SocketException se)
            {
                AgvError.Invoke(this.ID, se);
            }
            catch (Exception e)
            {
                MessageAction.Invoke("小车" + IP + "接收数据异常：" + e.Message);
            }
        }

        ~AgvSession()
        {
            socket.Close();
        }
    }
}
