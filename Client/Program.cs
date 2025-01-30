using System.Net;
using System.Net.Sockets;
using System.Text;
using Sockets.Logging;
namespace Sockets.Core;

public abstract class Client {
    const string THIS = "CLIENT";
    const string OTHER = "SERVER";

    static Socket socket = null!;
    static IPEndPoint remoteEp = null!;
    static void Init() {
        try {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList[1]; 
            remoteEp = new(ip, 6969);

            socket = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            Logger.Log(THIS, "Connecting to server...");

            socket.Connect(remoteEp);
            if (socket.RemoteEndPoint is IPEndPoint clientEp) Logger.Log(THIS, $"Connected to server: \nIP: {clientEp.Address}\nPort: {clientEp.Port}");
        } catch (Exception e) {
            Logger.LogError(THIS, e switch {
                SocketException se => $"The server is unavailable: {se.Message}\nTrace:\n{se.StackTrace}",
                ArgumentNullException ane => $"An argument was null: {ane.Message}\nTrace:\n{ane.StackTrace}",
                _ => e.Message,
            });
            return;
        }

        Task.Run(Receive);
        Send();
    }

    static void Send() {
        string input;
        do {
            input = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(input))continue;
            var buffer = Encoding.UTF8.GetBytes(input);
            socket.Send(buffer);
            Logger.Log(THIS, input);
        } while(input.ToLower().Trim() != "exit");
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    static void Receive() {
        var buffer = new byte[512];
        try {
            int bytesReceived;
            while((bytesReceived = socket.Receive(buffer)) > 0) {
                Logger.Log(OTHER, $"Received: {Encoding.UTF8.GetString(buffer, 0, bytesReceived)}");
            }
        } catch (Exception e) {
            Logger.LogError(OTHER, e switch { // switch expression gang :) no need for a million catch blocks
                SocketException se => $"SocketException: {se.Message}:\n{se.StackTrace}",
                _ => e.Message,
            });
        }
        Logger.Log(THIS, "Disconnected from server.");
    }
    public static int Main() {
        Init();
        return 0;
    }
}
