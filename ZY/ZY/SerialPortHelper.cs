using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY
{
    public class SerialPortHelper
    {
        private readonly static SerialPortHelper _Instance = new SerialPortHelper();

        public static SerialPortHelper Serialport { get { return _Instance; } }

        private SerialPort sp;

        private SerialPortHelper()
        {
        }

        /// <summary>
        /// 获取本机串口
        /// </summary>
        /// <returns></returns>
        public string[] GetSerialportArray()
        {
            return SerialPort.GetPortNames();
        }

        public void OpenSerial(string com)
        {
            try
            {
                sp = new SerialPort(com, 9600, Parity.None, 8, StopBits.None);
                sp.DataReceived += sp_DataReceived;
                if (!sp.IsOpen)
                {
                    sp.Open();
                }
            }
            catch
            { }
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] ReDatas = new byte[sp.BytesToRead];
                sp.Read(ReDatas, 0, ReDatas.Length);//读取数据

            }
            catch
            { }
        }

    }
}
