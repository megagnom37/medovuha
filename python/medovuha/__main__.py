import asyncio

from cock import build_entrypoint
from loguru import logger

from medovuha.injector import inject, register
from medovuha.service import MedovuhaService
from medovuha.options import options


@inject
def main(config, version):
    register(lambda: config, singleton=True, name="config")
    logger.info("version: {}, config: {}", version, config)
    asyncio.run(MedovuhaService().run())


entrypoint = build_entrypoint(main, options, auto_envvar_prefix="MEDOVUHA", show_default=True)
entrypoint(prog_name="medovuha")
