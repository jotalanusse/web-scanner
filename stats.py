import json
import math
import numpy as np
import matplotlib.pyplot as plt
from os import listdir
from os.path import isfile, join

CONFIG = {
    "output_directory": "./output",
}

# Get all the files in the output folder
files = [f for f in listdir(CONFIG["output_directory"]) if isfile(
    join(CONFIG["output_directory"], f))]

# Convert every file into a dictionary
scans = []
for file in files:
    with open(join(CONFIG["output_directory"], file)) as f:
        scan = json.load(f)
        scans.append(scan)

print("Total scans:", len(scans))

# Get the popularity of each port
ports = {}
total_ports = 0
for scan in scans:
    for port in scan["ports"]:
        if port not in ports:
            ports[port] = 0

        ports[port] += 1
        total_ports += 1

# Sort ports by value popularity  
sorted_ports = sorted(ports.items(), key=lambda kv: kv[1], reverse=False)

# Print the results
for port in sorted_ports:
    print(f"Port: [{port[0]}], total [{port[1]}], popularity [{(port[1] / total_ports) * 100}%]")
