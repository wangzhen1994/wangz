using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static SerialPort sp;

        static List<byte> buffer = new List<byte>();
        static void Main(string[] args)
        {
            sp = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);
            sp.Handshake = Handshake.None;

            sp.DataReceived += sp_DataReceived;
            if (!sp.IsOpen)
            {
                sp.Open();
            }

            Console.ReadKey();
        }

        static void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //byte[] ReDatas = new byte[sp.BytesToRead];
                //sp.Read(ReDatas, 0, ReDatas.Length);//读取数据
                //string s = Encoding.Default.GetString(ReDatas);
                //double va = GetWeight(s);
                //if(va!=0)
                //    Console.WriteLine(va);

                int n = sp.BytesToRead;
                byte[] buf = new byte[n];
                sp.Read(buf, 0, n);

                //1.缓存数据
                buffer.AddRange(buf);

                //2.1 查找数据头
                if (buffer[0] == 0x01) //传输数据有帧头，用于判断
                {
                    int len = buffer[2];
                    if (buffer.Count < len + 4) //数据区尚未接收完整
                    {

                    }
                    //得到完整的数据，复制到ReceiveBytes中进行校验
                    //buffer.CopyTo(0, ReceiveBytes, 0, len + 4);
                    byte jiaoyan; //开始校验
                    //jiaoyan = this.JY(ReceiveBytes);
                    //if (jiaoyan != ReceiveBytes[len+3]) //校验失败，最后一个字节是校验位
                    //{
                    //buffer.RemoveRange(0, len + 4);

                    //continue;
                    //}
                    buffer.RemoveRange(0, len + 4);
                    /////执行其他代码，对数据进行处理。
                }
                else //帧头不正确时，记得清除
                {
                    buffer.RemoveAt(0);
                }
            }

            catch
            { }
        }

        static double GetWeight(string s)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s) || !s.Contains("=") || s.Equals("="))
                {
                    return 0;
                }
                int index = s.IndexOfAny(new char[] { '=' });
                string value = s.Substring(index, 8).Replace("=", "");
                char[] value1 = value.Reverse<char>().ToArray();
                string value2 = new string(value1);
                double wei = Convert.ToDouble(value2);
                if (wei == 0)
                { }
                return wei;
            }
            catch
            { return 0; }
        }
    }
}
