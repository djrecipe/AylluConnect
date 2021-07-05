# imports
import xbee, time, sys
from machine import I2C
from struct import *


# bluetooth relay callback
def ble_callback(relay_frame):
    if relay_frame is not None:
        raw_data = relay_frame["message"]
        start_byte, data_length, addr0, addr1, addr2, addr3, addr4, addr5, addr6, addr7 = unpack('<BHBBBBBBBB', raw_data)
        if (start_byte != 0x7e):
            return
        sys.stdout.buffer.write(str(start_byte)+"\r\n")
        sys.stdout.buffer.write(str(data_length)+"\r\n")
        dest_addr = bytes((addr0, addr1, addr2, addr3, addr4, addr5, addr6, addr7))
        sys.stdout.buffer.write("created bytes\r\n")
        xmit_data_bytes = bytes("testing 123", 'utf-8')
        try:
            xbee.transmit(dest_addr, xmit_data_bytes)
            # TODO 06/21/21: create a response frame
            xbee.relay.send(xbee.relay.BLUETOOTH, "success")
        except Exception as e:
            # TODO 06/21/21: create a response frame
            xbee.relay.send(xbee.relay.BLUETOOTH, "error")


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