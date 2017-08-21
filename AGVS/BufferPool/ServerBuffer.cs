using Model;
using SocketHelper;
using System;

namespace BufferPool
{
    public class ServerBuffer
    {
        public readonly static ServerBuffer Instance = new ServerBuffer();

        private ServerBuffer()
        {
            Server = new AgvServer(new Action<AGV>(ReceiveAGV), null, new Action<string>(Message));
            Server.Start(ConfigBuffer.Port);
        }

        private AgvServer Server = null;

        public string Ip { get { return Server.ServerIP; } }

        public int Port { get { return Server.ServerPort; } }

        public bool State { get { return Server.ServerState; } }

        public int Count { get { return Server.SessionCount;     } }

        private void Message(string obj)
        {

        }

        private void ReceiveAGV(AGV obj)
        {
            AgvBuffer.Instance.Set(obj);
        }
    }
}
