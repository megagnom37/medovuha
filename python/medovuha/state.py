from enum import Enum
from typing import List
from uuid import UUID

from pydantic import BaseModel


class Position:
    x: float
    y: float
    z: float


class PlayerInfo(BaseModel):
    player_id: UUID
    position: Position


class GameState(Enum):
    waiting = "waiting"
    running = "running"
    stopped = "stopped"


class GameInfo(BaseModel):
    game_id: UUID
    state: GameState
    players: List[PlayerInfo]
    host: str
    port: str
