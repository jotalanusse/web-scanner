import json
import math
import numpy as np
import matplotlib.pyplot as plt
from os import listdir
from os.path import isfile, join

CONFIG = {
    "output_directory": "./output", # Directory containing the output files from the scan
    "min_ports": 1, # Minimum number of occurrences of a port to be considered
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

# Get the popularity of each port
ports = {}
total_ports = 0
for scan in scans:
    for port in scan["ports"]:
        if port not in ports:
            ports[port] = 0

        ports[port] += 1
        total_ports += 1
        
# Filter ports
filtered_ports = {}
total_filtered_ports = 0
for port in ports:
    if ports[port] >= CONFIG["min_ports"]:
        filtered_ports[port] = ports[port]
        total_filtered_ports += ports[port]

# Sort ports by value popularity  
sorted_ports = sorted(filtered_ports.items(), key=lambda kv: kv[1])

# Print the results
for port in sorted_ports:
    print(f"Port: [{port[0]}], total [{port[1]}], popularity [{round((port[1] / total_filtered_ports) * 100, 3)}%]")

print(f"Total scans: {len(scans)}, with {total_filtered_ports} open ports")
