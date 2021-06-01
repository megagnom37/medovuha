import pytest
from httpx import AsyncClient
from cock import Config

from medovuha.service import MedovuhaService
from medovuha.web import web_server_from_config
from medovuha.udp import udp_server_from_config


@pytest.fixture
async def config(unused_tcp_port):
    return Config(
        web_host="localhost",
        web_port=unused_tcp_port,
        udp_advertise_host="localhost",
        udp_advertise_port=60_000,  # TODO: make as factory with unused port
        udp_host="localhost",
        udp_port=60_000,  # TODO: make as factory with unused port
        udp_tickrate=32.0,
    )


@pytest.fixture
async def web(config):
    return web_server_from_config(config)


@pytest.fixture
async def udp(config):
    return udp_server_from_config(config)


@pytest.fixture
async def service(web, udp):
    async with MedovuhaService(web, udp) as s:
        yield s


@pytest.fixture
async def web_client(config, service):
    host = config["web_host"]
    port = config["web_port"]
    async with AsyncClient(base_url=f"http://{host}:{port}") as client:
        yield client
