using System;
using System.IO.Ports;
using System.Runtime.InteropServices;

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
            port.DataReceived += SerialPort_DataReceived;
            port.ErrorReceived += SerialPort_ErrorReceived;
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

        public void Send(byte[] data)
        {
            SerialHeader header;
            header.Marker1 = 14;
            header.Marker2 = 55;
            header.Length = data.Length;
            int size = Marshal.SizeOf(header);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(header, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            port.Write(arr, 0, arr.Length);
            port.Write(data, 0, data.Length);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!port.IsOpen)
                return;
            string data = "";
            while(this.port.BytesToRead > 0)
                data += this.port.ReadExisting();
            Console.WriteLine(data);
        }
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
        }

    }
}
