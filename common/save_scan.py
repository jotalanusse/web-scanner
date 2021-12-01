import json

# Save a scan to a file
def save_scan(scan, filename):
    print(f"Saving scan from host [{scan['host']}]")
    
    scan_json = json.dumps(scan) # Convert the scan to a JSON string
    
    with open(filename, "w") as file: # Open the file for writing
        file.write(scan_json) # Write the JSON string to the file