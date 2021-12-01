# COMMON_PORTS = [25565] # Minecraft mode go brrr
# COMMON_PORTS = [8291] # Mikrotic mode go brrr

COMMON_PORTS = [21, 22, 25, 23, 80, 81, 110, 123, 135, 137, 143, 145, 443, 465, 554, 587, 631, 993, 995, 1433, 1434, 3306, 3389, 5900, 8291, 9050, 25565]
CONFIG = {
    "host_scan_threads": 100, # Total threads to scan hosts
    "port_scan_threads": 5, # Total threads to scan ports
    "host_scan_batch": 15000, # Batch size to scan hosts
    "screenshot_delay": 10, # Delay in seconds before taking a screenshot
    "output_file": "results.json", # Name of the output file
    "ports_to_scan": COMMON_PORTS, # List of ports to scan
    # "ports_to_scan": list(range(1, 65536)), # List of ports to scan
    "host_progress_message_batch": 1000, # Batch size to print host scanning progress
    "port_progress_message_batch": 1000, # Batch size to print port scanning progress
    "ping_timeout": 1, # Timeout for the ping command
    "port_scan_timeout": 1, # Timeout for the port scan command
    "output_directory": "output", # Directory to store the scan results
}