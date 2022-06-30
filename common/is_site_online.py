import requests

# Check if a webiste is online
def is_site_online(host):
    resp = requests.get(host)

    # TODO: Maybe add 403 and other status codes to the list
    if resp.ok:
        print(f"Site [{host}] is online")
        
        return True
    else:
        return False