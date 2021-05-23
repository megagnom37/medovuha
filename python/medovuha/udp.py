import asyncio
import json
from uuid import uuid4

from medovuha.injector import inject, register

from facet import ServiceMixin


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


class UdpServer(ServiceMixin):

    def __init__(self, host, port):
        self.host = host
        self.port = port
        self.address_by_game_id = {}
        self.socket = None

    async def start(self):
        self.socket = UdpSocket(local_address=(self.host, self.port))
        await self.socket.initialize()
        self.add_task(self.listen())

    async def stop(self):
        await self.socket.close()

    async def listen(self):
        async for data, address in self.socket:
            # TODO: use pydantic to declare packet format
            parsed_data = json.loads(data)
            game_id = parsed_data["game_id"]
            if game_id not in self.address_by_game_id:  # unknown game
                continue  # TODO: response with "no such game"
            for address in self.address_by_game_id[game_id]:
                # TODO: repack data, so no sensitive information will reach another user
                await self.socket.send(data, address)  # this is ok, since `send` implementation have not `awaits`

    def create_game(self):
        return str(uuid4())

    def remove_game(self, game_id: str):
        self.address_by_game_id.pop(game_id)


@register(name="udp_server", singleton=True)
@inject
def create_udp_server(config):
    return UdpServer(
        host=config["udp_host"],
        port=config["udp_port"],
    )
