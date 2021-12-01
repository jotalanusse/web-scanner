import socket
from config import CONFIG

# Scan if a specific port is open in the given host
def is_port_open(host, port):
    print(f"Scanning port [{port}] for host [{host}]...")
    
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM) # Create a new socket
    sock.settimeout(CONFIG["port_scan_timeout"]) # Set a limit timeout
    response = sock.connect_ex((host, port)) # Connect to the port
    sock.close()  # Close the socket onc the scan is done
    
    if response == 0: # Return true or false depending on the response
        print(f"Port [{port}] is open on host [{host}]")
        
        return port
    else:
        return None