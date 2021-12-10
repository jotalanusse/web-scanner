import time
import functools
from .nesteable_pools import NestablePool
from .is_port_open import is_port_open
from config import CONFIG

# Scan a list of ports in a host to find out if they are open
def scan_open_ports(host, timeout):
    # print(f"Scanning ports for host [{host}]...")
    
    start_time = time.time() # Start a timer to measure performance
  
    open_ports = []
    pool = NestablePool(CONFIG["port_scan_threads"]) # Start a new multiprocessing pool to scan ports
    open_ports = pool.map(functools.partial(is_port_open, host, timeout), CONFIG["ports_to_scan"]) # Scan ports in parallel
    pool.close() # Close the pool
    pool.join() # Start the pool
    
    open_ports = list(filter(None, open_ports)) # Remove all null values from list
    
    end_time = time.time() # Stop the timer
    total_time = end_time - start_time # Calculate the total time it took to scan ports

    print(f"Finished scanning ports for host [{host}], total time [{round(total_time, 2)}] seconds")

    return open_ports