from uuid import UUID

import pytest

from medovuha.state import GameInfo


@pytest.mark.asyncio
async def test_get_game(web_client, config, service):
    assert len(service.games) == 0

    response = await web_client.get("get_game")
    response.raise_for_status()

    info1 = GameInfo.parse_obj(response.json())
    assert isinstance(info1.game_id, UUID)
    assert info1.host == config.udp_host
    assert info1.port == config.udp_port
    assert len(service.games) == 1

    response = await web_client.get("get_game")
    response.raise_for_status()

    info2 = GameInfo.parse_obj(response.json())
    assert isinstance(info2.game_id, UUID)
    assert info2.host == config.udp_host
    assert info2.port == config.udp_port
    assert len(service.games) == 1
