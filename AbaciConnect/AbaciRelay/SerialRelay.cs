using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;

namespace AbaciConnect.Relay
{
    public class SerialRelay : IRelay
    {
        private readonly SerialPort port = null;
        public SerialRelay(string name)
        {
            port = new SerialPort(name);
            port.BaudRate = 115200;
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.Encoding = Encoding.Unicode;
            //port.DataReceived += SerialPort_DataReceived;
            //port.ErrorReceived += SerialPort_ErrorReceived;
            port.Open();
            port.DiscardInBuffer();
            if(!port.IsOpen)
                throw new Exception("Failed to connect to serial port " + name);
        }


        public void Dispose()
        {
            if (this.port.IsOpen)
                this.port.Close();
        }
        private byte[] GetHeader(DataTypes type, long length)
        {
            SerialHeader header;
            header.Marker1 = 14;
            header.Marker2 = 55;
            header.Type = DataTypes.Bytes;
            header.Length = length;
            int size = Marshal.SizeOf(header);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(header, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        public string GetText()
        {
            return this.port.ReadExisting();
        }
        public void SendBytes(byte[] data)
        {
            byte[] header_bytes = GetHeader(DataTypes.Bytes, data.Length);
            // send header bytes
            port.Write(header_bytes, 0, header_bytes.Length);
            // send data
            port.Write(data, 0, data.Length);
        }
        public void SendString(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] header_bytes = GetHeader(DataTypes.String, data.Length);
            // send header bytes
            port.Write(header_bytes, 0, header_bytes.Length);
            // send data
            port.Write(data, 0, data.Length);
        }
        public void SendFile(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            byte[] header_bytes = GetHeader(DataTypes.File, data.Length);
            // send header bytes
            port.Write(header_bytes, 0, header_bytes.Length);
            // send data
            port.Write(data, 0, data.Length);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //if (!port.IsOpen)
            //    return;
            //string data = "";
            //while(this.port.BytesToRead > 0)
            //    data += this.port.ReadExisting();
            //Console.WriteLine(data);
        }
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
        }

    }
}
