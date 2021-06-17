# imports
import xbee, time
import micropython
from sys import stdin, stdout
def rx_callback(packet):
    micropython.kbd_intr(-1)
    if(packet != None):
        
        stdout.buffer.write(packet["payload"])
    micropython.kbd_intr(3)
        #for i in range(0,3):
        #    print("{}".format(packet["payload"][i]))
        #stdout.buffer.write(data.encode("utf-8"));
        #print("{}".format(packet["payload"]))
# set name
xbee.atcmd("NI","XBee B");
# configure network
network_settings = {"CE": 0, "ID": 0x0, "EE": 0, "NT":0x20}
for command, value in network_settings.items():
	print("SET {}: {}".format(command, value))
	xbee.atcmd(command, value)
xbee.atcmd("AC") # apply changes
time.sleep(1)
# wait for success
print("Connecting to network, please wait...")
while xbee.atcmd("AI") != 0:
    time.sleep(0.1)
print("Connected to Network")
xbee.receive_callback(None)
# print network parameters
operating_network = ["OI", "OP", "CH"]
print("Operating network parameters:")
for cmd in operating_network:
	print("{}: {}".format(cmd, xbee.atcmd(cmd)))
xbee.receive_callback(rx_callback)
# loops
while True:
    time.sleep(0.01)
    #vals = xbee.receive()
    #rx_callback(vals)