using BufferPool;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AGVS
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }
        const int CLOSE_SIZE = 10;
        private void MainTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Rectangle myTabRect = this.MainTabControl.GetTabRect(e.Index);
                //先添加TabPage属性   
                e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);
                //再画一个矩形框
                using (Pen p = new Pen(Color.White))
                {
                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                    myTabRect.Width = CLOSE_SIZE;
                    myTabRect.Height = CLOSE_SIZE;
                    e.Graphics.DrawRectangle(p, myTabRect);
                }

                //填充矩形框
                Color recColor = e.State == DrawItemState.Selected ? Color.White : Color.White;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTabRect);
                }

                //画关闭符号
                using (Pen objpen = new Pen(Color.Black))
                {
                    ////=============================================
                    //自己画X
                    ////"\"线
                    //Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                    //Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                    //e.Graphics.DrawLine(objpen, p1, p2);
                    ////"/"线
                    //Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                    //Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                    //e.Graphics.DrawLine(objpen, p3, p4);

                    ////=============================================
                    //使用图片
                    Bitmap bt = new Bitmap(Environment.CurrentDirectory + @"\close.png");
                    Point p5 = new Point(myTabRect.X, 4);
                    e.Graphics.DrawImage(bt, p5);
                    //e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, objpen.Brush, p5);
                }
                e.Graphics.Dispose();
            }
            catch
            { }
        }

        private void MainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    int x = e.X, y = e.Y;
                    //计算关闭区域   
                    Rectangle myTabRect = this.MainTabControl.GetTabRect(this.MainTabControl.SelectedIndex);

                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                    myTabRect.Width = CLOSE_SIZE;
                    myTabRect.Height = CLOSE_SIZE;

                    //如果鼠标在区域内就关闭选项卡   
                    bool isClose = x > myTabRect.X && x < myTabRect.Right && y > myTabRect.Y && y < myTabRect.Bottom;
                    if (isClose == true)
                    {
                        DialogResult dr = MessageBox.Show("关闭标签页？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dr == System.Windows.Forms.DialogResult.Yes)
                        {
                            (this.MainTabControl.SelectedTab.Controls[0] as Form).Close();
                            GC.Collect();
                            this.MainTabControl.TabPages.Remove(this.MainTabControl.SelectedTab);
                        }
                    }
                }
            }
            catch
            { }
        }

        private void Show(Form f, string text)
        {
            try
            {
                foreach (TabPage t in this.MainTabControl.TabPages)
                {
                    if (t.Name.Equals("tp_" + f.Name))
                    {
                        this.MainTabControl.SelectTab(t);
                        return;
                    }
                }
                TabPage tp = new TabPage(text);
                tp.Name = "tp_" + f.Name;

                f.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                f.Dock = DockStyle.Fill;
                f.TopLevel = false;
                tp.Controls.Add(f);
                this.MainTabControl.TabPages.Add(tp);
                f.Show();
                this.MainTabControl.SelectTab(tp);
            }
            catch
            { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.toolTimeNow.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.toolState.Text = ServerBuffer.Instance.State ? "已运行" : "未运行";
                this.toolCount.Text = ServerBuffer.Instance.Count.ToString();
            }
            catch
            { }
        }

        private void 监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show(new FrmShow(), "监控");
        }

        private void 配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show(new FrmConfig(), "配置");
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.toolIP.Text = ServerBuffer.Instance.Ip;
            this.toolPort.Text = ServerBuffer.Instance.Port.ToString();
        }
    }
}
