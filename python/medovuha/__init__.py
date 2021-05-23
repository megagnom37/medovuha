from importlib import resources

from medovuha.injector import register


@register(singleton=True)
def version():
    return resources.read_text("medovuha", "version.txt").strip()
