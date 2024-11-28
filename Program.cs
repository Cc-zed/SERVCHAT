using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private const int PORT = 12345;
    private const int MAX_CLIENTS = 5;
    private static UdpClient udpServer;
    private static List<IPEndPoint> clients = new List<IPEndPoint>();

    static void Main()
    {
        Console.WriteLine("Starting UDP Chat Server...");
        udpServer = new UdpClient(PORT);
        Console.WriteLine($"Server is listening on port {PORT}");

        while (true)
        {
            try
            {
                // Receive data from clients
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpServer.Receive(ref clientEndpoint);

                string message = Encoding.UTF8.GetString(receivedData);
                Console.WriteLine($"Received: {message} from {clientEndpoint}");

                if (message.StartsWith("CONNECT:"))
                {
                    if (clients.Count >= MAX_CLIENTS)
                    {
                        string error = "ERROR: Max clients reached.";
                        udpServer.Send(Encoding.UTF8.GetBytes(error), error.Length, clientEndpoint);
                        continue;
                    }

                    if (!clients.Contains(clientEndpoint))
                    {
                        clients.Add(clientEndpoint);
                        string username = message.Substring(8).Trim();
                        BroadcastMessage($"SERVER: {username} joined the chat.");
                    }
                }
                else if (message.StartsWith("MESSAGE:"))
                {
                    BroadcastMessage(message.Substring(8).Trim());
                }
                else if (message.StartsWith("DISCONNECT:"))
                {
                    clients.Remove(clientEndpoint);
                    string username = message.Substring(11).Trim();
                    BroadcastMessage($"SERVER: {username} left the chat.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void BroadcastMessage(string message)
    {
        Console.WriteLine($"Broadcasting: {message}");
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (var client in clients)
        {
            udpServer.Send(data, data.Length, client);
        }
    }
}
