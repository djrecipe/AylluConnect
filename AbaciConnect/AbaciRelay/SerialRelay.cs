using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Linq;

namespace AbaciConnect.Relay
{
    internal class SerialRelay : IRelay
    {
        private readonly SerialPort port = null;
        private readonly BackgroundWorker workerWrite = new BackgroundWorker();
        private readonly Mutex mutexPort = new Mutex();
        private readonly Mutex mutexBytes = new Mutex();
        private List<byte> byteQueue = new List<byte>();
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
        public void SendBytes(byte[] data)
        {
            Write(data);
        }
        public List<byte> WaitForBytes(int count)
        {
            List<byte> local_bytes = new List<byte>();
            // wait for bytes
            while(true)
            {
                this.mutexBytes.WaitOne();
                // check if new bytes received
                if(this.byteQueue.Count > 0)
                {
                    // determine number of remaining bytes desired
                    int remaining = count - local_bytes.Count;
                    // determine number of bytes to take
                    int take_count = Math.Min(this.byteQueue.Count, remaining);
                    // copy the bytes
                    local_bytes.AddRange(this.byteQueue.Take(take_count));
                    // remove bytes from queue
                    this.byteQueue.RemoveRange(0, take_count);
                }
                this.mutexBytes.ReleaseMutex();
                // check if recieved desired byte count
                if (local_bytes.Count >= count)
                    break;
            }
            // return bytes
            return local_bytes;
        }
        private void Write(byte[] data)
        {
            while(this.workerWrite.IsBusy);
            this.ClearBuffer();
            this.workerWrite.RunWorkerAsync(data);
            this.workerWrite.RunWorkerCompleted += workerWrite_RunWorkerCompleted;
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            List<byte> local_bytes = new List<byte>();
            this.mutexPort.WaitOne();
            // read bytes
            while (this.port.BytesToRead > 0)
            {
                int current_byte = port.ReadByte();
                if(current_byte != -1)
                    local_bytes.Add((byte)current_byte);
            }
            this.mutexPort.ReleaseMutex();
            // add bytes to thread-safe queue
            this.mutexBytes.WaitOne();
            this.byteQueue.AddRange(local_bytes);
            this.mutexBytes.ReleaseMutex();
            return;
        }
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
        }
        private void workerWrite_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] args = e.Argument as byte[];
            // send header bytes
            this.mutexPort.WaitOne();
            port.Write(args, 0, args.Length);
            this.mutexPort.ReleaseMutex();
        }
        private void workerWrite_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

    }
}
