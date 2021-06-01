from click import INT, FLOAT
from cock import Option, build_options_from_dict

web_options = build_options_from_dict({
    "web": {
        "host": Option(default="127.0.0.1"),
        "port": Option(default=8080, type=INT),
    },
})

udp_options = build_options_from_dict({
    "udp": {
        "host": Option(default="127.0.0.1"),
        "port": Option(default=9090, type=INT),
        "tickrate": Option(default=32.0, type=FLOAT),
    },
})

options = [
    *web_options,
    *udp_options,
]
