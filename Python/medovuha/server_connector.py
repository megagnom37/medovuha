import socket
import subprocess
from contextlib import closing

from aiohttp import web

from medovuha.server import Server


routes = web.RouteTableDef()

class ServerConnector:
    def __init__(self):
        self._app = web.Application()
        self._server = None

        self.host = '127.0.0.1'
        self.port = 9999

    def start(self):
        self._app.add_routes([web.post('/server_connector/connect', self.connect)])
        web.run_app(self._app)

    @staticmethod
    def _find_free_port():
        with closing(socket.socket(socket.AF_INET, socket.SOCK_STREAM)) as s:
            s.bind(('', 0))
            s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            return s.getsockname()[1]

    async def connect(self, request):
        # TODO: Get some player metadata for server
        # data = await request.json()

        if not self._server:
            # host = '127.0.0.1'
            # port = self._find_free_port()
            # TODO: Set host and port for server
            subprocess.Popen(["python", "./server.py"])
            self._server = True


        response_data = {
            'host': self.host,
            'port': self.port
        }

        return web.json_response(response_data)

if __name__ == '__main__':
    s = ServerConnector()
    s.start()