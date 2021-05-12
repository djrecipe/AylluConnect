using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AbaciConnect.Relay
{
    public class SerialRelay : IRelay
    {
        private readonly SerialPort port = null;
        private readonly BackgroundWorker workerWrite = new BackgroundWorker();
        private readonly Mutex mutexPort = new Mutex();
        private readonly Mutex mutexBytes = new Mutex();
        private int bytesWaitingToRead = 0;
        private int bytesCurrentlyRead = 0;
        private List<byte> bytesRead = new List<byte>();
        public SerialRelay(string name)
        {
            this.workerWrite.DoWork += this.workerWrite_DoWork;
            //
            port = new SerialPort(name);
            port.BaudRate = 115200;
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.Encoding = Encoding.Unicode;
            port.DataReceived += SerialPort_DataReceived;
            port.ErrorReceived += SerialPort_ErrorReceived;
            port.Open();
            port.DiscardInBuffer();
            if(!port.IsOpen)
                throw new Exception("Failed to connect to serial port " + name);
        }
        public void ClearBuffer()
        {
            this.mutexPort.WaitOne();
            this.port.DiscardInBuffer();
            this.port.DiscardOutBuffer();
            this.mutexPort.ReleaseMutex();
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
        public void SendBytes(byte[] data, DataTypes data_type = DataTypes.Bytes)
        {
            byte[] header_bytes = GetHeader(data_type, data.Length);
            Write(data, header_bytes);
        }
        public void SendString(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            SendBytes(data, DataTypes.String);
        }
        public void SendFile(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            SendBytes(data, DataTypes.File);
        }
        public List<byte> WaitForBytes(int count)
        {
            this.mutexBytes.WaitOne();
            this.bytesWaitingToRead = count;
            this.mutexBytes.ReleaseMutex();
            List<byte> local_bytes = new List<byte>();
            bool pending = true;
            while(pending)
            {
                this.mutexBytes.WaitOne();
                if(this.bytesCurrentlyRead >= this.bytesWaitingToRead)
                {
                    local_bytes = new List<byte>(this.bytesRead);
                    pending = false;
                    this.bytesWaitingToRead = 0;
                    this.bytesRead.Clear();
                }
                this.mutexBytes.ReleaseMutex();
                if(pending)
                    Thread.Sleep(100);
            }
            return local_bytes;
        }
        private void Write(byte[] data, byte[] header)
        {
            this.ClearBuffer();
            Tuple<byte[], byte[]> args = new Tuple<byte[], byte[]>(data, header);
            this.workerWrite.RunWorkerAsync(args);
            this.workerWrite.RunWorkerCompleted += workerWrite_RunWorkerCompleted;
        }


        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            List<byte> local_bytes = new List<byte>();
            this.mutexPort.WaitOne();
            while (this.port.BytesToRead > 0)
            {
                int current_byte = port.ReadByte();
                if(current_byte != -1)
                    local_bytes.Add((byte)current_byte);
            }
            this.mutexPort.ReleaseMutex();
            this.mutexBytes.WaitOne();
            if (this.bytesWaitingToRead > 0)
            {

                this.bytesRead.AddRange(local_bytes);
                this.bytesCurrentlyRead = this.bytesRead.Count;
                Debug.WriteLine($"{this.port.PortName} received {this.bytesCurrentlyRead} of {this.bytesWaitingToRead} bytes");
            }
            else
            {
                string text = Encoding.ASCII.GetString(local_bytes.ToArray());
                Debug.Write(text);
            }
            this.mutexBytes.ReleaseMutex();
            return;
        }
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
        }
        private void workerWrite_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<byte[],byte[]> args = e.Argument as Tuple<byte[],byte[]>;
            byte[] data = args.Item1;
            byte[] header = args.Item2;
            // send header bytes
            this.mutexPort.WaitOne();
            port.Write(header, 0, header.Length);
            for (int i = 0; i < data.Length; i+=84)
            {
                int current_length = Math.Min(84, data.Length - i);
                port.Write(data, i, current_length);
                // TODO 5/12/21: determine why this sleep is necessary
                Thread.Sleep(20);
            }
            this.mutexPort.ReleaseMutex();
            e.Result = data.Length;
        }
        private void workerWrite_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int length = (int)e.Result;
            Debug.WriteLine($"{this.port.PortName} sent {length} bytes");
        }

    }
}
