# imports
import xbee, time
import micropython
from sys import stdin, stdout
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
while 1:
    text = ""
    ch = ''
    while(True):
        ch = stdin.buffer.read(1)
        if(ch == b'\r' or ch == b'\n'):
            break
        stdout.buffer.write(ch)
        text += ch.decode("utf-8")
    print("")
    print("Sending: '{}'".format(text))
    xbee.transmit(xbee.ADDR_BROADCAST, text)