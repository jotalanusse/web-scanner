# COMMON_PORTS = [25565] # Minecraft mode go brrr
# COMMON_PORTS = [8291] # Mikrotic mode go brrr
# COMMON_PORTS = [23] # Telnet mode go brrr

# COMMON_PORTS = [9, 21, 22, 25, 23, 43, 80, 81, 110, 123, 135, 137, 143, 145, 401, 443, 465, 554, 587, 631, 989, 990, 993, 995, 1433, 1434, 1883, 3000, 3306, 3389, 5900, 8291, 9050, 25565, 42069]
CONFIG = {
    "host_scan_threads": 30, # Total threads to scan hosts
    "port_scan_threads": 120, # Total threads to scan ports
    "host_scan_batch_size": 10000000, # Batch size to scan hosts
    # "screenshot_delay": 2, # Delay in seconds before taking a screenshot
    # "ports_to_scan": COMMON_PORTS, # List of ports to scan
    "ports_to_scan": list(range(1, 65536)), # List of ports to scan
    "host_progress_message_batch": 1000, # Batch size to print host scanning progress
    "port_progress_message_batch": 1000, # Batch size to print port scanning progress
    "ping_timeout": 1.5, # Timeout for the ping command
    "port_scan_timeout_multiplier": 1.5, # The host ping derlay will be multiplied by this value to calculate the port scan timeout
    "output_directory": "output", # Directory to store the scan results
}