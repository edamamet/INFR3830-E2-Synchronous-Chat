using System.Net;
using System.Net.Sockets;
using System.Text;
using Sockets.Logging;
namespace Sockets.Core;

public abstract class Server {
    const string THIS = "SERVER";
    const string OTHER = "CLIENT";
    static Socket client = null!;
    static void Init() {
        try {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList[1]; // get ip on network
            var serverEp = new IPEndPoint(ip, 6969);

            Logger.Log(THIS, $"{host.HostName} @ {ip}");

            var socket = new Socket( // use udp
                ip.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            socket.Bind(serverEp);
            socket.Listen(1);
            Logger.Log(THIS, "Listening...");

            client = socket.Accept();
            if (client.RemoteEndPoint is IPEndPoint clientEp)
                Logger.Log(THIS, $"Connected to client: \nIP: {clientEp.Address}\nPort: {clientEp.Port}");

            Task.Run(Receive);
            Send();

        } catch (Exception e) {
            Logger.LogError(THIS, e switch {
                SocketException se => $"SocketException: {se.Message}\nTrace:\n{se.StackTrace}",
                ArgumentNullException ane => $"An argument was null: {ane.Message}\nTrace:\n{ane.StackTrace}",
                _ => e.Message,
            });
        }
    }

    static void Send() {
        string input;
        do {
            input = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(input)) continue;
            var message = Encoding.UTF8.GetBytes(input);
            client.Send(message);
            Logger.Log(THIS, input);
        } while(input.ToLower().Trim() != "exit");

        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    static void Receive() {
        var buffer = new byte[512];
        try {
            int bytesReceived;
            while((bytesReceived = client.Receive(buffer)) > 0) {
                Logger.Log(OTHER, $"Received: {Encoding.UTF8.GetString(buffer, 0, bytesReceived)}");
            }
        } catch (Exception e) {
            Logger.LogError(OTHER, e switch {
                SocketException se => $"SocketException: {se.Message}:\n{se.StackTrace}",
                _ => e.Message,
            });
        }
    }

    static int Main() {
        Init();
        return 0;
    }
}
