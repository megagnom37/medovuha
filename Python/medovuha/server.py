import socket
import threading
import time
import pickle


class Server:
    def __init__(self, host, port):
        self._host = host
        self._port = port

        self._cli_addr = set()
        self._cli_data = dict()

        self._sock = None

    def initialize(self):
        self._sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self._sock.bind((self._host, self._port))
        self._sock.settimeout(4.0)

    def get_server_url(self):
        return self._host, self._port

    def handle_data(data, from_address):
        player_id, x, y, z = data.decode().split(':')
        # print(f'\tPlayer: {player_id} ({x}, {y}, {z})')
        self._cli_addr.add(from_address)
        self._cli_data[from_address] = f'{player_id}:{x}:{y}:{z}'

    def _send_data_to_clients(self):
        time.sleep(10.)
        while True:
            resp = ';'.join(self._cli_data.values()).encode()
            for client in self._cli_addr:
                self._sock.sendto(resp, client)
            # time.sleep(0.1)

    def wait_for_client(self):
        print('Start Server...')
        c_thread = threading.Thread(target=self._send_data_to_clients,
                                    args=tuple())
        c_thread.daemon = True
        c_thread.start()

        try:
            while True:
                try:
                    data, client_address = self._sock.recvfrom(1024)
                    # print(f'{client_address} recived')

                    handle_data(data, client_address)
                except socket.timeout:
                    pass
        except KeyboardInterrupt:
            self._sock.close()
            print('KeyboardInterrupt catched')
        print('End...')

if __name__ == '__main__':
    s = Server('127.0.0.1', 9999)
    s.initialize()
    s.wait_for_client()
