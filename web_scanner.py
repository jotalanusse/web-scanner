from common import scan_hosts
from config import CONFIG

def main():
  print('#####################################')
  print('#########  WEB SCANNER v1.00  #######')
  print('#########  By @jotalanusse    #######')
  print('#####################################')

  print('Main process started!')
  
  hosts = []
  # scan_hosts(hosts)
  
  # for i in range(100000000):
  #     ip = socket.inet_ntoa(struct.pack('>I', random.randint(1, 0xffffffff)))
      
  #     if len(hosts) == CONFIG["host_scan_batch"]:
  #         scan_hosts(hosts)
  #         hosts = []
            
  #     if ip.split(".")[0] != "127": # Fuck them 127 IPs             
  #         hosts.append(ip)
      
  for i in range(256):
      for o in range(256):
          for p in range(256):
              ip = "148.%d.%d.%d" % (i, o, p)
              
              if len(hosts) == CONFIG["host_scan_batch"]:
                  scan_hosts(hosts)
                  hosts = []
              
              hosts.append(ip)
    
  scan_hosts(hosts)

if __name__ == '__main__':
  main() # Let's go