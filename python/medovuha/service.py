from typing import List

from facet import ServiceMixin
from loguru import logger

from medovuha.web import WebServer
from medovuha.udp import UdpServer
from medovuha.injector import inject
from medovuha.state import GameState, GameInfo, GameStage


class MedovuhaService(ServiceMixin):

    @inject
    def __init__(self, web_server: WebServer, udp_server: UdpServer):
        self.web_server = web_server
        self.udp_server = udp_server
        self.games: List[GameState] = []

    @property
    def dependencies(self):
        return [
            self.udp_server,
            self.web_server,
        ]

    def configure_router(self):
        self.web_server.add_get("/get_game", self.get_game, response_model=GameInfo)

    async def start(self):
        self.configure_router()

    async def get_game(self) -> GameInfo:
        for game in self.games:
            if game.stage == GameStage.waiting:
                logger.info("get_game: returning already created game {}", game.info)
                return game.info

        game = await self.create_game()
        self.games.append(game)
        logger.info("get_game: returning new game {}", game.info)
        return game.info

    async def create_game(self) -> GameState:
        game = self.udp_server.create_game()
        logger.info("create_game: game {} created", game.info)
        return game
