# imports
from machine import I2C
import xbee, time
import sys
import time

# bluetooth relay callback
def ble_callback(relay_frame):
  if relay_frame is not None:
    data = relay_frame["message"]
    try:
        xbee.transmit(bytes([0x00,0x13,0xA2,0x00,0x41,0xB7,0x64,0xAD]), data)
        relay.send(relay.BLUETOOTH, "success")
    except Exception as e:
        # TODO 06/21/21: create a response frame
        relay.send(relay.BLUETOOTH, "error")
# set name
xbee.atcmd("NI","XBee A");
# configure network
network_settings = {"CE": 1, "ID": 0xCCCC, "EE": 0, "NJ": 0xFF, "NT": 0x20, "JV": 1, "JN": 1}
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
    
xbee.relay.callback(ble_callback)
# main loop
while(True):
    continue
    # do nothing