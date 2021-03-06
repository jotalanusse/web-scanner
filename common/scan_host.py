import json
import ping3
from .scan_open_ports import scan_open_ports
from .save_scan import save_scan
from .is_site_online import is_site_online
from .create_directory import create_directory
# from .screenshot_website import screenshot_website
from os.path import join
from config import CONFIG

# Scan a host and return a dictionary with the information
def scan_host(id, host):
    # print(f"Scan [{id}]: Target host is [{host}]")
    
    if id % CONFIG["host_progress_message_batch"] == 0: # If the ID is a multiple of the batch size
        print(f"Total hosts scanned: {id}, current host [{host}]") # Print a progress message
    
    ping = ping3.ping(host, timeout=CONFIG["ping_timeout"]) # Ping the host and get the response time

    if ping != None: # If the host is online
        print(f"Host [{host}] is online")
        
        multiplied_timeout = ping * CONFIG["port_scan_timeout_multiplier"] # Calculate the timeout for the port scanning
        open_ports = scan_open_ports(host, multiplied_timeout) # Scan the host for open ports
        
        scan = {
            "host": host,
            "ping": ping,
            "ports": open_ports,
        }
        
        if len(scan["ports"]) > 0: # If the host has open ports
            scan_output_path = join(CONFIG["output_directory"], scan["host"]) # Create the output directory for the host
            create_directory(scan_output_path) # Create the directory for the scan
            save_scan(scan, join(scan_output_path, "scan.json")) # Save the scan to a file
            
            # site = "http://" + scan['host']
            # if is_site_online(site): # Only attempt to screenshot if the site is online
            #     screenshot_website(site, join(scan_output_path, scan["host"] + ".png")) # Save an image of the site
                
            