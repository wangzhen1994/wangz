using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace wangz.TcpServer
{
    public partial class FrmTcpServer : Form
    {
        public FrmTcpServer()
        {
            InitializeComponent();
        }

        AppServer server;
        MethodInvoker mi;

        private void FrmTcpServer_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            mi = new MethodInvoker(ReadSession);
            //mi.BeginInvoke(new AsyncCallback(ReadCallBack), mi);
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            MethodInvoker m = ar.AsyncState as MethodInvoker;
            m.EndInvoke(ar);

            Thread.Sleep(300);

            m.BeginInvoke(new AsyncCallback(ReadCallBack), mi);
        }

        private void ReadSession()
        {
            if (server != null && server.Sessions.Count > 0)
            {
                lock (server)
                {
                    try
                    {
                        this.listBox1.BeginUpdate();

                        string[] sessions = server.Sessions.Keys.ToArray();
                        System.Windows.Forms.ListBox.ObjectCollection items = this.listBox1.Items;

                        List<object> tempitem = new List<object>();
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (!sessions.Contains(items.ToString()))
                            {
                                tempitem.Add(items[i]);
                            }
                        }
                        foreach (object obj in tempitem)
                        {
                            this.listBox1.Items.Remove(obj);
                        }

                        List<string> temp = new List<string>();
                        for (int i = 0; i < sessions.Length; i++)
                        {
                            if (!items.Contains(temp))
                            {
                                temp.Add(sessions[i]);
                            }
                        }
                        this.listBox1.Items.AddRange(temp.ToArray());
                        this.listBox1.EndUpdate();
                    }
                    catch
                    { }
                }
            }
            else
            {
                this.listBox1.Items.Clear();
            }
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            int port = Convert.ToInt32(this.numericUpDown1.Value);
            server = new AppServer(port);
            server.MessageEvent += server_MessageEvent;
            server.AddConnectedEvent += server_AddConnectedEvent;
            server.RemoveConnectedEvent += server_RemoveConnectedEvent;
            server.Start();
            this.toolStripStatusLabel1.Text = server.LocalIp;
        }

        void server_RemoveConnectedEvent(string obj)
        {
            try
            {
                if (listBox1.Items.Contains(obj))
                {
                    listBox1.Items.Remove(obj);
                }
            }
            catch
            { }
        }

        void server_AddConnectedEvent(string obj)
        {
            try
            {
                listBox1.Items.Add(obj);
            }
            catch
            { }
        }

        void server_MessageEvent(string obj)
        {
            this.textBox1.Invoke((MethodInvoker)delegate
            {
                this.textBox1.AppendText(obj + Environment.NewLine);
            });
        }
    }
}
