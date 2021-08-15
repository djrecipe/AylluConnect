using AbaciConnect.Relay;
using AbaciConnect.Relay.Emission;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace AylluConnectUI
{
    public class TransmitPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; 
        public ICommand CommandTransmit { get; private set; }
        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            return;
        }
        public TransmitPageModel()
        {
            this.CommandTransmit = new RelayCommand(this.ExecuteCommandTransmit, this.CanExecuteCommandTransmit);
        }
        private bool CanExecuteCommandTransmit()
        {
            return true;
        }
        private void ExecuteCommandTransmit()
        {

            int expected_byte_count = 20000;
            byte[] raw_bytes = new byte[expected_byte_count];
            Random rnd = new Random();
            for (int i = 0; i < expected_byte_count; i++)
            {
                //raw_bytes[i] = (byte)rnd.Next();
                raw_bytes[i] = (byte)'x';
            }
            using (RelayController ctrl_send = RelayController.ConnectSerial("COM4"))
            {

                ctrl_send.Clear();
                ulong long_address = 0x0013A20041B764AD;
                // receive "Discover" emission
                try
                {
                    ctrl_send.Transmit(long_address, raw_bytes);
                }
                catch
                {
                    // ignored
                    Console.WriteLine("Transmission failed");
                }
            }
        }
    }
}
