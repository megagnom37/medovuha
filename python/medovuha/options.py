from click import INT
from cock import Option, build_options_from_dict

web_options = build_options_from_dict({
    "web": {
        "host": Option(default="0.0.0.0"),
        "port": Option(default=80, type=INT),
    },
})

udp_options = build_options_from_dict({
    "udp": {
        "host": Option(default="0.0.0.0"),
        "port": Option(default=666, type=INT),
    },
})

options = [
    *web_options,
    *udp_options,
]
