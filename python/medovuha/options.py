from click import INT, FLOAT
from cock import Option, build_options_from_dict

web_options = build_options_from_dict({
    "web": {
        "host": Option(default="0.0.0.0"),
        "port": Option(default=80, type=INT),
    },
})

udp_options = build_options_from_dict({
    "udp": {
        "advertise_host": Option(default="127.0.0.1"),
        "advertise_port": Option(default=666, type=INT),
        "host": Option(default="0.0.0.0"),
        "port": Option(default=666, type=INT),
        "tickrate": Option(default=32.0, type=FLOAT),
    },
})

options = [
    *web_options,
    *udp_options,
]
