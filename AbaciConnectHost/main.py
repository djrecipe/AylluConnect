# imports
import xbee, time, sys
from machine import I2C
from struct import *


# bluetooth relay callback
def ble_callback(relay_frame):
    if relay_frame is None:
        return
    raw_data = relay_frame["message"]
    raw_data_len = len(raw_data)
    sys.stdout.buffer.write("raw data len:"+str(raw_data_len)+"\r\n")
    if (raw_data_len < 11):
        return
    header = raw_data[0:11]
    start_byte, data_length, addr0, addr1, addr2, addr3, addr4, addr5, addr6, addr7 = unpack('>BHBBBBBBBB', header)
    sys.stdout.buffer.write("start byte:"+str(start_byte)+"\r\n")
    if (start_byte != 0x7e):
        sys.stdout.buffer.write("error: invalid start byte:"+str(start_byte)+"\r\n")
        return
    sys.stdout.buffer.write("data length:"+str(data_length)+"\r\n")
    if(raw_data_len<data_length+11):
        sys.stdout.buffer.write("error: invalid data length:"+str(data_length)+"+11<"+str(raw_data_len))
        return
    sys.stdout.buffer.write("addr0:"+hex(addr0)+"\r\n")
    sys.stdout.buffer.write("addr1:"+hex(addr1)+"\r\n")
    sys.stdout.buffer.write("addr2:"+hex(addr2)+"\r\n")
    sys.stdout.buffer.write("addr3:"+hex(addr3)+"\r\n")
    sys.stdout.buffer.write("addr4:"+hex(addr4)+"\r\n")
    sys.stdout.buffer.write("addr5:"+hex(addr5)+"\r\n")
    sys.stdout.buffer.write("addr6:"+hex(addr6)+"\r\n")
    sys.stdout.buffer.write("addr7:"+hex(addr7)+"\r\n")
    #dest_addr = bytes([0x00,0x13,0xA2,0x00,0x41,0xB7,0x64,0xAD])
    dest_addr = bytes((addr0, addr1, addr2, addr3, addr4, addr5, addr6, addr7))
    sys.stdout.buffer.write("dest addr:"+str(dest_addr)+"\r\n")
    xmit_data=raw_data[11:11+data_length]
    sys.stdout.buffer.write("xmit data:"+str(xmit_data)+"\r\n")
    try:
        xbee.transmit(dest_addr, xmit_data)
        xbee.relay.send(xbee.relay.BLUETOOTH, bytes([0x7e, 0x00, 0x07, 0x8B, 0, 0xFF, 0xFE, 0, 0x00, 0x00, 0x00]))
    except Exception as e:
        xbee.relay.send(xbee.relay.BLUETOOTH, bytes([0x7e, 0x00, 0x07, 0x8B, 0, 0xFF, 0xFE, 0, 0x01, 0x00, 0x00]))


# set name
xbee.atcmd("NI", "XBee A");
# configure network
network_settings = {"CE": 1, "ID": 0xCCCC, "EE": 0, "NJ": 0xFF, "NT": 0x20, "JV": 1, "JN": 1}
for command, value in network_settings.items():
    print("SET {}: {}".format(command, value))
    xbee.atcmd(command, value)
xbee.atcmd("AC")  # apply changes
time.sleep(1)
# wait for success
print("Establishing network, please wait...")
while xbee.atcmd("AI") != 0:
    time.sleep(0.1)
print("Network Established")
# print network parameters
operating_network = ["OI", "OP", "CH"]
print("Operating network parameters:")
for cmd in operating_network:
    print("{}: {}".format(cmd, xbee.atcmd(cmd)))

xbee.relay.callback(ble_callback)
# main loop
while (True):
    continue
    # do nothing