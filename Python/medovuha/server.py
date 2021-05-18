import socket
import threading
import time
import pickle


host = '127.0.0.1'
port = 9999

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind((host, port))
sock.settimeout(4.0)

cli_addr = set()
cli_data = dict()

def handle_data(data, from_address):
    player_id, x, y, z = data.decode().split(':')
    print(f'\tPlayer: {player_id} ({x}, {y}, {z})')
    cli_addr.add(from_address)
    cli_data[from_address] = f'({player_id}:{x}:{y}:{z})'

def send_data_to_clients():
    while True:
        resp = ';'.join(cli_data.values()).encode()
        for client in cli_addr:
            sock.sendto(resp, client)
        time.sleep(2)

def wait_for_client():
    print('Start Server...')
    c_thread = threading.Thread(target=send_data_to_clients,
                                args=tuple())
    c_thread.daemon = True
    c_thread.start()

    try:
        while True:
            try:
                data, client_address = sock.recvfrom(1024)
                print(f'{client_address} recived')

                handle_data(data, client_address)
            except socket.timeout:
                pass
    except KeyboardInterrupt:
        sock.close()
        print('KeyboardInterrupt catched')
    print('End...')

if __name__ == '__main__':
    wait_for_client()