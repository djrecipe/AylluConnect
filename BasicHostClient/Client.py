# imports
import xbee, time
# set name
xbee.atcmd("NI","XBee B");
# configure network
network_settings = {"CE": 0, "ID": 0xABCD, "EE": 0, "NT":0x20}
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
# loops
while True:
    time.sleep(0.1)
    vals = xbee.receive()
    if(vals != None):
        print("{}".format(vals["payload"]))