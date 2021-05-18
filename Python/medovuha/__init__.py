from importlib import resources

VERSION = resources.read_text("medovuha", "version.txt").strip()
