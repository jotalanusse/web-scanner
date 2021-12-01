from .scan_host import scan_host
from .nesteable_pools import NestablePool
from config import CONFIG

# Scan a range of hosts
def scan_hosts(hosts):
    print(f"Processing a total of [{len(hosts)}] hosts")
        
    pool = NestablePool(CONFIG["host_scan_threads"]) # Start a new multiprocessing pool to scan hosts
    for index, host in enumerate(hosts): # For each host in the list
        pool.apply_async(scan_host, args=(index, host)) # Use multiprocessing to scan all the hosts

    pool.close() # Close the pool
    pool.join() # Start the pool