from facet import ServiceMixin

from pydantic import BaseModel

from medovuha.web import WebServer
from medovuha.udp import UdpServer
from medovuha.injector import inject


class GameInfo(BaseModel):
    game_id: str
    host: str
    port: str


class MedovuhaService(ServiceMixin):

    @inject
    def __init__(self, web_server: WebServer, udp_server: UdpServer):
        self.web_server = web_server
        self.udp_server = udp_server

    @property
    def dependencies(self):
        return [
            self.udp_server,
            self.web_server,
        ]

    def configure_router(self):
        self.web_server.add_get("/create_game", self.create_game, response_model=GameInfo)

    async def start(self):
        self.configure_router()

    async def create_game(self) -> GameInfo:
        return GameInfo(
            game_id=self.udp_server.create_game(),
            host=self.udp_server.host,
            port=self.udp_server.port,
        )
