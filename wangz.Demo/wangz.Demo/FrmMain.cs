using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using wangz.TcpServer;

namespace wangz.Demo
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnTcpServer_Click(object sender, EventArgs e)
        {
            FrmTcpServer f = new FrmTcpServer();
            f.Show();
        }
    }
}
