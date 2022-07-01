using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;

List<int> ports = new List<int> { 9, 21, 22, 25, 23, 43, 80, 81, 110, 123, 135, 137, 143, 145, 401, 443, 465, 554, 587, 631, 989, 990, 993, 995, 1433, 1434, 1883, 3000, 3306, 3389, 5900, 8291, 9050, 25565, 42069 };
// List<int> ports = new List<int> { 25565 };

WebScanner webScanner = new WebScanner(ports, 300, 1.2, "./scans.txt", 1000);

for (int i = 0; i < 1; i += 1)
{
    List<IPAddress> hosts = webScanner.GenerateRandomIPAddresses(1000000);
    webScanner.ScanHosts(hosts);
}


public class WebScanner {

    // Scan settings
    List<int> ports;
    int scanTimeout;
    double portTimeoutRatio;

    string outputFilePath;

    // Threading settings
    int minThreads;

    private static ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

    public WebScanner(List<int> ports, int scanTimeout, double portTimeoutRatio, string outputFilePath, int minThreads)
    {
        this.ports = ports;
        this.scanTimeout = scanTimeout;
        this.portTimeoutRatio = portTimeoutRatio;
        this.outputFilePath = outputFilePath;
        this.minThreads = minThreads;

        // Use the maximum available threads
        int worker;
        int ioCompletion;
        ThreadPool.GetMaxThreads(out worker, out ioCompletion);
        ThreadPool.SetMinThreads(worker, ioCompletion);
    }

    // Create a list of random IP addresses
    public List<IPAddress> GenerateRandomIPAddresses(int amount)
    {
        List<IPAddress> addresses = new List<IPAddress>();

        for (int i = 0; i < amount; i += 1)
        {
            addresses.Add(GenerateRandomIPAddress());
        }

        return addresses;
    }

    // Create a single random IP address
    public IPAddress GenerateRandomIPAddress()
    {
        byte[] data = new byte[4]; // Create a new array for our adress

        Random random = new Random();
        random.NextBytes(data); // Populate the array with random data

        // No class A addresses under my watch
        if (data[0] == 127)
        {
            data[0] = 69;
        }

        IPAddress ip = new IPAddress(data); // Create an IPAddress from the random bytes

        return ip;
    } 

    // Ping a host and return its reply
    PingReply PingHost(IPAddress host, int timeout)
    {
        // Console.WriteLine($"Pinging host [{host}]");

        Ping ping = new Ping();
        PingReply reply = ping.Send(host, timeout);
        ping.Dispose();

        return reply;
    }

    // Check if the given port is open in a host
    bool IsPortOpen(IPAddress host, int port, int timeout)
    {
        // Console.WriteLine($"Scanning port [{port}] on host [{host}]");

        using (TcpClient client = new TcpClient())
        {
            IAsyncResult result = client.BeginConnect(host, port, null, null); // Use begin connect to allow the use of timeouts

            bool connected = result.AsyncWaitHandle.WaitOne(timeout); // We wait for the host until the time runs out

            if (!connected)
            {
                // Console.WriteLine($"Port [{port}] not open on host [{host}]");

                return false;
            }

            try
            {
                client.EndConnect(result);

                Console.WriteLine($"Port [{port}] is open on host [{host}]");

                return true;
            }
            catch
            {
                Console.WriteLine($"Error while scanning [{port}] on host [{host}]");

                return false;
            }
            finally
            {
                client.Dispose();
            }
        }
    }

    // Scan a host for open ports and return the list of purts that are open
    List<int> ScanOpenPorts(IPAddress host, List<int> ports, int timeout)
    {
        // Console.WriteLine($"Scanning open ports for host [{host}]");

        ConcurrentBag<int> openPorts = new ConcurrentBag<int>();

        ParallelLoopResult result = Parallel.ForEach(ports, port =>
        {
            bool isPortOpen = IsPortOpen(host, port, timeout);

            if (isPortOpen)
            {
                openPorts.Add(port);
            }
        });

        return openPorts.ToList();
    }

    // Scan a list of hosts and return the results
    public List<Scan> ScanHosts(List<IPAddress> hosts)
    {
        Console.WriteLine($"Scanning a total of [{hosts.Count}] hosts");

        ConcurrentBag<Scan> scans = new ConcurrentBag<Scan>();

        Stopwatch clock = Stopwatch.StartNew();
        clock.Start();

        int totalCompletedScans = 0;
        int batchCompletedScans = 0;

        ParallelLoopResult result = Parallel.ForEach(hosts, new ParallelOptions { MaxDegreeOfParallelism = 400 }, host =>
        {
            Scan scan = ScanHost(host);
            SaveScan(scan, outputFilePath);


            totalCompletedScans += 1;
            batchCompletedScans += 1;

            if (batchCompletedScans % 20000 == 0)
            {
                clock.Stop();
                double elapsedMilliSeconds = clock.ElapsedMilliseconds;
                double scansPerMilliSecond = batchCompletedScans / elapsedMilliSeconds;
                double scansPerSecond = Math.Round(scansPerMilliSecond * 1000, 2);
                double portScansPerSecond = Math.Round(scansPerMilliSecond * ports.Count * 1000, 2);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Total scanned hosts [{totalCompletedScans}] @ Scans [{scansPerSecond}/s], Ports [{portScansPerSecond}/s]");
                Console.ResetColor();

                Console.WriteLine($"Last scanned host was [{scan.host}]");

                batchCompletedScans = 0;
                clock.Restart();
                clock.Start();
            }

            // scans.Add(scan);
        });

        return scans.ToList();
    }

    // Sva a scan to the specified file
    void SaveScan(Scan scan, string outputFilePath)
    {

        if (scan.online && scan.ports.Count > 0)
        {
            // Convert the scan to a "SimpleScan" so it's easier to serialize
            SimpleScan simpleScan = new SimpleScan(scan);
            string scanJson = JsonConvert.SerializeObject(simpleScan);

            readerWriterLock.EnterWriteLock(); // Use a lock to avoid 2 threads trying to write at the same time
            try
            {
                using (FileStream fs = new FileStream(outputFilePath, FileMode.Append, FileAccess.Write))
                using (StreamWriter outputFile = new StreamWriter(fs))
                {
                    outputFile.WriteLine(scanJson);
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }

    // Scan a single host and return the scan result
    public Scan ScanHost(IPAddress host)
    {
        // Console.WriteLine($"Scanning host [{host}]...");

        Scan scan = new Scan(host); // Create an empty scan instance to save our details

        try
        {
            PingReply pingReply = PingHost(host, scanTimeout); // First ping the host to see if it's online

            // If the host is not online we simply return the empty scan
            if (pingReply.Status == IPStatus.Success)
            {
                // Console.WriteLine($"Host [{host}] is online with a ping of [{pingReply.RoundtripTime}] ms");

                scan.online = true;
                scan.ping = pingReply.RoundtripTime;

                int portScanTimeout = (int)(scan.ping * portTimeoutRatio); // We use the ping of the host as the base of the timeout
                scan.ports = ScanOpenPorts(host, ports, portScanTimeout); // TODO: Change the timeout for the host ping

                return scan;
            }
        }
        catch (PingException e)
        {
            // Console.WriteLine(e.Message);
        }

        // Console.WriteLine($"Host [{host}] is offline");

        return scan;
    }

    public class Scan
    {
        public IPAddress host;
        public bool online = false;
        public List<int> ports = new List<int>();
        public long ping = 0;

        public Scan(IPAddress host)
        {
            this.host = host;
        }
    }

    public class SimpleScan
    {
        public string host { get; set; }
        public List<int> ports { get; set; }
        public long ping { get; set; }

        public SimpleScan(Scan scan)
        {
            host = scan.host.ToString();
            ports = scan.ports;
            ping = scan.ping;
        }
    }
}
