# imports
import xbee, time
import micropython
from sys import stdin, stdout
from struct import *
# set name
xbee.atcmd("NI","XBee A");
# configure network
network_settings = {"CE": 1, "ID": 0xABCD, "EE": 0, "NJ": 0xFF, "NT": 0x20}
for command, value in network_settings.items():
	print("SET {}: {}".format(command, value))
	xbee.atcmd(command, value)
xbee.atcmd("AC") # apply changes
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
# main loop
micropython.kbd_intr(-1)
data_pending = False
data_length = 0
while(True):
    # read header
    if(data_pending):
        data = stdin.buffer.read(-1)
        xbee.transmit(xbee.ADDR_BROADCAST, "ok") # convert bytearray to bytes
        data_pending = False
        data_length = 0
    else:
        header_data = stdin.buffer.read(10) # blocks until receiving 10 bytes
        header_marker1, header_marker2, data_length = unpack('bbl', header_data)
        if(header_marker1==14 and header_marker2==55):
            data_pending=True
            xbee.transmit(xbee.ADDR_BROADCAST, "Waiting for " + str(data_length) + " bytes") # convert bytearray to bytes
        else:
            stdin.buffer.read(-1)
            xbee.transmit(xbee.ADDR_BROADCAST, "Invalid data received") # convert bytearray to bytes