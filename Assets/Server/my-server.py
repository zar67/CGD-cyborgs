import socket
import sys

import GameData
import time

def decodeMessage(connection : socket, _data, _address) -> bool :
    if(_data == b"AddClient"):
        GameData.AddClient(_address[0], _address[1], str(len(GameData.allClients)))
    return True

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the port
server_address = ('localhost', 10000)
print('starting up on %s port %s' % server_address)
sock.bind(server_address)

sock.listen(1)

while True:
    # Wait for a connection
    print('waiting for a connection')
    connection, client_address = sock.accept()
    try:
        print('connection from', client_address)

        # Receive the data in small chunks and retransmit it
        while True:
            data = connection.recv(16)
            if(decodeMessage(connection, data, client_address)):
                connection.sendall(b"Success")
            #print('received "%s"' % data)
            #if data:
            #    print('sending data back to the client')
            #    connection.sendall(data)
            #else:
            ##    print('no more data from', client_address)
            break
            #time.sleep(0.1)
            
    finally:
        # Clean up the connection
        connection.close()

