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
data_pending = False
data_length = 0
while(True):
    # read header
    header_marker1=0
    header_marker2=0
    data_type=0
    if(data_pending):
        for x in range(0, data_length, 84):
            current_length = min(84, data_length - x)
            data = stdin.buffer.read(current_length)
            try:
                xbee.transmit(xbee.ADDR_BROADCAST, data) # convert bytearray to bytes
            except:
                xbee.transmit(xbee.ADDR_BROADCAST, "Error while transmitting data")
        data_pending = False
        micropython.kbd_intr(3)
        data_length = 0
    else:
        #xbee.transmit(xbee.ADDR_BROADCAST, "Waiting for header")                                                   # --- DEBUG
        header_data = stdin.buffer.read(11) # blocks until receiving 10 bytes
        #xbee.transmit(xbee.ADDR_BROADCAST, "Received header " + str(header_data))                                  # --- DEBUG
        header_marker1, header_marker2, data_type, data_length = unpack('<bbbQ', header_data)
        #xbee.transmit(xbee.ADDR_BROADCAST, "Header markers: " + str(header_marker1) + ", " + str(header_marker2))  # --- DEBUG
        #xbee.transmit(xbee.ADDR_BROADCAST, "Header data length: " + str(data_length))                              # --- DEBUG
        if(header_marker1 == 14 and header_marker2==55):
            data_pending=True
            micropython.kbd_intr(-1)
            #xbee.transmit(xbee.ADDR_BROADCAST, "Waiting for bytes") # convert bytearray to bytes                   # --- DEBUG
        else:
            stdin.buffer.read(-1)
            #xbee.transmit(xbee.ADDR_BROADCAST, "Invalid data received") # convert bytearray to bytes               # --- DEBUG