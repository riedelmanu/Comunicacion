using System.Net;
using System.Net.Sockets;
using System.Text;

// Args: [ipRemota] [puertoLocal] [puertoRemoto]
if (args.Length < 3)
{
    Console.WriteLine("Uso: UdpChat <ipRemota> <puertoLocal> <puertoRemoto>");
    return;
}

string remoteIp = args[0];
int localPort = int.Parse(args[1]);
int remotePort = int.Parse(args[2]);

// Crear socket UDP
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

// Asociar al puerto local
socket.Bind(new IPEndPoint(IPAddress.Any, localPort));

// Definir el endpoint remoto
IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);

// Hilo para escuchar mensajes entrantes
Thread receiver = new Thread(() =>
{
    byte[] buffer = new byte[1024];
    EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

    while (true)
    {
        int bytes = socket.ReceiveFrom(buffer, ref remote);
        string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
        Console.WriteLine($"[RECIBIDO] {msg}");
    }
});
receiver.IsBackground = true;
receiver.Start();

// Bucle de envío
Console.WriteLine($"Chat UDP iniciado. Local {localPort}, Remoto {remoteIp}:{remotePort}");
Console.WriteLine("Escriba mensajes y Enter para enviar. /q para salir.");

while (true)
{
    string? line = Console.ReadLine();
    if (string.IsNullOrEmpty(line)) continue;
    if (line.Trim() == "/q") break;

    byte[] data = Encoding.UTF8.GetBytes(line);
    socket.SendTo(data, remoteEndPoint);
}
