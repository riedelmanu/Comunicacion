using System.Net;
using System.Net.Sockets;
using System.Text;

// Args: [puertoLocal] [puertoRemoto] [modo] [rol]
if (args.Length < 4)
{
    Console.WriteLine("Uso: Handshake <puertoLocal> <puertoRemoto> <modo:2|3|4> <rol:cliente|servidor>");
    return;
}

int localPort = int.Parse(args[0]);
int remotePort = int.Parse(args[1]);
int modo = int.Parse(args[2]); // 2,3,4
string rol = args[3].ToLower();

UdpClient udp = new UdpClient(localPort);
IPEndPoint remote = new IPEndPoint(IPAddress.Loopback, remotePort);

void Send(string msg)
{
    byte[] data = Encoding.UTF8.GetBytes(msg);
    udp.Send(data, data.Length, remote);
    Console.WriteLine($"[ENVIADO] {msg}");
}

string Receive()
{
    IPEndPoint anyRemote = new IPEndPoint(IPAddress.Any, 0); // inicializado
    var res = udp.Receive(ref anyRemote);
    string msg = Encoding.UTF8.GetString(res);
    Console.WriteLine($"[RECIBIDO] {msg} desde {anyRemote}");
    return msg;
}

if (rol == "cliente")
{
    if (modo == 2)
    {
        Send("SYN");
        Receive(); // ACK
    }
    else if (modo == 3)
    {
        Send("SYN");
        Receive(); // SYN-ACK
        Send("ACK");
    }
    else if (modo == 4)
    {
        Send("SYN");
        Receive(); // SYN-ACK
        Send("ACK");
        Receive(); // FINAL-ACK
    }
}
else if (rol == "servidor")
{
    if (modo == 2)
    {
        Receive(); // SYN
        Send("ACK");
    }
    else if (modo == 3)
    {
        Receive(); // SYN
        Send("SYN-ACK");
        Receive(); // ACK
    }
    else if (modo == 4)
    {
        Receive(); // SYN
        Send("SYN-ACK");
        Receive(); // ACK
        Send("FINAL-ACK");
    }
}
