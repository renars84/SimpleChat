using ChatServer.Net.IO;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: User has connected with the username: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    // Not events but you could do it see other part of the project
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now:HH:mm:ss}]: [{Username}]: {msg}");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"User with Username: {Username} and UID: [{UID.ToString()}]: Disconnected!");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
