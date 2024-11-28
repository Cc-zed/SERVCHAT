using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 12345;

    private static UdpClient udpClient;
    private static string username;

    static void Main()
    {
        Console.Write("Enter your username: ");
        username = Console.ReadLine();

        udpClient = new UdpClient();
        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT);

        // Connect to the server
        string connectMessage = $"CONNECT:{username}";
        udpClient.Send(Encoding.UTF8.GetBytes(connectMessage), connectMessage.Length, serverEndpoint);

        // Start a thread to listen for incoming messages
        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        // Send messages
        while (true)
        {
            string message = Console.ReadLine();
            if (message.ToLower() == "exit")
            {
                string disconnectMessage = $"DISCONNECT:{username}";
                udpClient.Send(Encoding.UTF8.GetBytes(disconnectMessage), disconnectMessage.Length, serverEndpoint);
                break;
            }

            string formattedMessage = $"MESSAGE:[{username}] {message}";
            udpClient.Send(Encoding.UTF8.GetBytes(formattedMessage), formattedMessage.Length, serverEndpoint);
        }
    }

    private static void ReceiveMessages()
    {
        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                byte[] receivedData = udpClient.Receive(ref serverEndpoint);
                string receivedMessage = Encoding.UTF8.GetString(receivedData);
                Console.WriteLine(receivedMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                break;
            }
        }
    }
}
