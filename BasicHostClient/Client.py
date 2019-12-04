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
# print network parameters
operating_network = ["OI", "OP", "CH"]
print("Operating network parameters:")
for cmd in operating_network:
	print("{}: {}".format(cmd, xbee.atcmd(cmd)))
# print network devices
while 1:
    result = xbee.discover()
    nodes = list(result)
    if nodes:
        for node in nodes:
            name = node["node_id"]
            strength = node["rssi"]
            print("'{}: {}'".format(name, strength))
            if strength>=-14:
                xbee.atcmd("D4",4);
            else:
                xbee.atcmd("D4",5);
            time.sleep(0.1)