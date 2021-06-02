import asyncio
import time
from uuid import UUID
from typing import Optional, Dict, Any

from facet import ServiceMixin
from fastapi.encoders import jsonable_encoder
from loguru import logger
from pydantic import BaseModel, ValidationError

from medovuha.injector import inject, register
from medovuha.state import GameState, GameStage


class UdpProtocol(asyncio.DatagramProtocol):

    def __init__(self, queue: asyncio.Queue):
        self._packets = queue

    def connection_made(self, transport):
        pass

    def connection_lost(self, transport):
        pass

    def datagram_received(self, data, address):
        self._packets.put_nowait((data, address))


class UdpSocket:

    def __init__(self, *,
                 local_address=None,
                 remote_address=None,
                 receive_queue_max_size=0,
                 **create_datagram_endpoint_kwargs):
        self.local_address = local_address
        self.remote_address = remote_address
        self.receive_queue_max_size = receive_queue_max_size
        self.create_datagram_endpoint_kwargs = create_datagram_endpoint_kwargs
        self.receive_queue = None
        self.protocol = None
        self.transport = None
        self.initialized = False

    async def initialize(self):
        if self.initialized:
            raise RuntimeError("socket already initialized")
        self.receive_queue = asyncio.Queue(maxsize=self.receive_queue_max_size)
        self.protocol = UdpProtocol(self.receive_queue)
        loop = asyncio.get_running_loop()
        self.transport, _ = await loop.create_datagram_endpoint(
            lambda: self.protocol,
            local_addr=self.local_address,
            remote_addr=self.remote_address,
            **self.create_datagram_endpoint_kwargs,
        )
        self.initialized = True

    async def close(self):
        if not self.initialized:
            raise RuntimeError("socket not initialized")
        self.transport.close()

    async def __aenter__(self):
        await self.initialize()
        return self

    async def __aexit__(self, *exc_info):
        await self.close()

    async def send(self, data, address=None):
        self.transport.sendto(data, address)

    async def receive(self):
        return await self.receive_queue.get()

    def __aiter__(self):
        return self

    async def __anext__(self):
        return await self.receive()


class JsonRpcRequest(BaseModel):
    method: str
    parameters: dict
    id: Optional[str] = None


class BaseParams(BaseModel):
    game_id: UUID
    player_id: UUID


class UdpServer(ServiceMixin):

    def __init__(self, advertise_host, advertise_port, host, port, tickrate):
        self.advertise_host = advertise_host
        self.advertise_port = advertise_port
        self.host = host
        self.port = port
        self.tick_interval = 1 / tickrate
        self.last_tick_start = None
        self.socket = None
        self.games: Dict[UUID, GameState] = {}
        self.players_address: Dict[UUID, Any] = {}

    async def start(self):
        self.socket = UdpSocket(local_address=(self.host, self.port))
        await self.socket.initialize()
        self.add_task(self.listen())
        self.add_task(self.send_state())

    async def stop(self):
        await self.socket.close()

    async def listen(self):
        async for data, address in self.socket:
            # logger.debug("Received from {}: {}", address, data) TODO: change log level
            try:
                request = JsonRpcRequest.parse_raw(data)
                base_params = BaseParams.parse_obj(request.parameters)
            except ValidationError:
                logger.exception("Bad request")
                continue
            if base_params.game_id not in self.games:
                logger.info("Game with id {} do not exist", base_params.game_id)
                continue  # TODO: response with "no such game"
            self.players_address[base_params.player_id] = address
            self.games[base_params.game_id].dispatch(request.method, request.parameters)

            # TODO: Change it later
            if (len(self.games[base_params.game_id].players) == 2 and 
                    self.games[base_params.game_id].stage == GameStage.waiting):
                self.games[base_params.game_id].stage = GameStage.preparing
                self.add_task(self.start_game(base_params.game_id))

            # TODO: add response maybe
    
    async def start_game(self, game_id):
        # Delay for creating enemys
        await asyncio.sleep(1) # TODO: improve it later
        self.games[game_id].stage = GameStage.running

    async def send_state(self):
        while True:
            now = time.perf_counter()
            if self.last_tick_start is not None:
                need_to_sleep = max(0, self.last_tick_start + self.tick_interval - now)
                await asyncio.sleep(need_to_sleep)
                now += need_to_sleep
            self.last_tick_start = now
            for state in self.games.values():
                rpc_request = JsonRpcRequest(method="update_state", parameters=jsonable_encoder(state))
                data = rpc_request.json().encode("utf-8")
                for player in state.players.values():
                    address = self.players_address.get(player.player_id)
                    if not address:
                        continue
                    await self.socket.send(data, address)

    def create_game(self) -> GameState:
        state = GameState.from_address(self.advertise_host, self.advertise_port)
        self.games[state.info.game_id] = state
        return state


@register(name="udp_server", singleton=True)
@inject
def udp_server_from_config(config):
    return UdpServer(
        advertise_host=config.udp_advertise_host,
        advertise_port=config.udp_advertise_port,
        host=config.udp_host,
        port=config.udp_port,
        tickrate=config.udp_tickrate,
    )
