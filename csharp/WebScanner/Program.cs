using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

WebScanner webScanner = new WebScanner(new List<int> { 80, 443 }, 400);

List<IPAddress> hosts = webScanner.GenerateRandomIPAddresses(100);

foreach (IPAddress host in hosts)
{
    webScanner.ScanHost(host);
}


public class WebScanner {
    List<int> ports;
    int timeout;

    public WebScanner(List<int> ports, int timeout)
    {
        this.ports = ports;
        this.timeout = timeout;
    }

    public List<IPAddress> GenerateRandomIPAddresses(int amount)
    {
        List<IPAddress> addresses = new List<IPAddress>();

        for (int i = 0; i < amount; i += 1)
        {
            addresses.Add(GenerateRandomIPAddress());
        }

        return addresses;
    }

    public IPAddress GenerateRandomIPAddress()
    {
        byte[] data = new byte[4]; // Create a new array for our adress

        Random random = new Random();
        random.NextBytes(data); // Populate the array with random data

        IPAddress ip = new IPAddress(data); // Create an IPAddress from the random bytes

        return ip;
    } 

    PingReply PingHost(IPAddress host, int timeout)
    {
        Console.WriteLine($"Pinging host [{host}]");

        Ping ping = new Ping();
        PingReply reply = ping.Send(host, timeout);

        Console.WriteLine($"Host [{host}] status is [{reply.Status}]");

        return reply;
    }

    bool IsPortOpen(IPAddress host, int port, int timeout)
    {
        Console.WriteLine($"Scanning port [{port}] on host [{host}]");

        using (TcpClient tcpClient = new TcpClient())
        {
            IAsyncResult asyncResult = tcpClient.BeginConnect(host, port, null, null);
            using (asyncResult.AsyncWaitHandle)
            {
                //Wait 2 seconds for connection.
                if (asyncResult.AsyncWaitHandle.WaitOne(timeout, false))
                {
                    try
                    {
                        tcpClient.EndConnect(asyncResult);
                        return true;
                    }
                    catch
                    {
                        // EndConnect threw an exception.
                        // Most likely means the server refused the connection.
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }

    List<int> ScanOpenPorts(IPAddress host, List<int> ports, int timeout)
    {
        Console.WriteLine($"Scanning open ports for host [{host}]");

        List<int> openPorts = new List<int>();

        foreach (int port in ports)
        {
            bool isPortOpen = IsPortOpen(host, port, timeout);

            if (isPortOpen)
            {
                Console.WriteLine($"Port [{port}] is open on host [{host}]!");
                openPorts.Add(port);
            }
        }

        return ports;
    }

    public Scan ScanHost(IPAddress host)
    {
        Console.WriteLine($"Scanning host [{host}]...");

        Scan scan = new Scan(host); // Create an empty scan instance to save our details

        try
        {
            PingReply pingReply = PingHost(host, timeout); // First ping the host to see if it's online

            // If the host is not online we simply return the empty scan
            if (pingReply.Status == IPStatus.Success)
            {
                Console.WriteLine($"Host [{host}] is online with a ping of [{pingReply.RoundtripTime}]");

                scan.online = true;
                scan.ping = pingReply.RoundtripTime;
                scan.ports = ScanOpenPorts(host, ports, timeout);

                return scan;
            }
        }
        catch (PingException)
        {
            // Ignore
        }

        Console.WriteLine($"Host [{host}] is offline");
        return scan;
    }

    public class Scan
    {
        public IPAddress address;
        public bool online = false;
        public List<int> ports = new List<int>();
        public long ping = 0;

        public Scan(IPAddress address)
        {
            this.address = address;
        }
    }
}
