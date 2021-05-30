from enum import Enum
from typing import Dict
from uuid import UUID, uuid4
from functools import lru_cache
from inspect import signature

from pydantic import BaseModel, Field
from loguru import logger


class Position(BaseModel):
    x: float
    y: float
    z: float


class PlayerInfo(BaseModel):
    player_id: UUID
    player_name: str
    position: Position


class GameStage(Enum):
    waiting = "waiting"
    running = "running"
    stopped = "stopped"


class GameInfo(BaseModel):
    game_id: UUID = Field(default_factory=uuid4)
    host: str
    port: int


class GameState(BaseModel):
    stage: GameStage
    info: GameInfo
    players: Dict[UUID, PlayerInfo] = Field(default_factory=dict)

    METHODS = {
        "set_position",
    }

    @classmethod
    def from_address(cls, host: str, port: int):
        return cls(
            stage=GameStage.waiting,
            info=GameInfo(host=host, port=port),
        )

    @classmethod
    @lru_cache
    def _resolve_model(cls, method_name) -> BaseModel:
        method = getattr(cls, method_name)
        sig = signature(method)
        Model = sig.parameters["params"].annotation
        return Model

    def dispatch(self, method: str, params: dict):
        if method not in self.METHODS:
            logger.info("Method {!r} not available", method)
            return
        Model = self._resolve_model(method)
        parsed_params = Model.parse_obj(params)
        return getattr(self, method)(parsed_params)

    def set_position(self, params: PlayerInfo):
        self.players[params.player_id].position = params.position
