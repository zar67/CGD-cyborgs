import socket

allClients = []
turnIndex = 0

class Client:
    ip = ""
    port = ""
    id = ""
    connection = ""
    def __init__(self, _ip : str, _port : str, _id : str, _connection : socket) -> None:
        self.ip = _ip
        self.port = _port
        self.id = _id
        self.connection = _connection
    
def AddClient(_ip : str, _port: str, _id : str, _connection : socket):
    client = Client(_ip, _port, _id, _connection)
    allClients.append(client)

def UpdateTurn():
    global turnIndex
    if(turnIndex < len(allClients)-1):
        turnIndex = turnIndex + 1
    else:
        turnIndex = 0