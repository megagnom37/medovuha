import socket
import random
import pickle
import time

host = '127.0.0.1'
port = 9999

def main():
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    while True:
        value = int(random.random()*100)
        print(f'Sent value: {value}')

        data = pickle.dumps(value)
        sock.sendto(data, (host, port))
        received = sock.recv(1024)
        print(f'Recived: {pickle.loads(received)}')

        time.sleep(2)


if __name__ == '__main__':
    main()
