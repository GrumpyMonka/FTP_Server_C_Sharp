using API;
using Newtonsoft.Json;
using ServerFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using static GrumpyMonk.GMUdpClient;

namespace GrumpyMonk
{
    internal class GMUdpClient
    {
        public struct Packet
        {
            private UdpAPI packetUdpAPI;

            public IPEndPoint PacketEndPoint;
            
            public UdpAPI PacketUdpAPI 
            {
                get { return packetUdpAPI; }
                set { packetUdpAPI = value; }
            }
            public string PacketDataString
            {
                get { return JsonConvert.SerializeObject( PacketUdpAPI ); }
                set 
                { 
                    PacketUdpAPI = JsonConvert.DeserializeObject<UdpAPI>( value );
                    if (  PacketUdpAPI != null ) 
                        throw new Exception( "Ошибка парсинга пакета: " + value );
                }
            }

            public byte[] PacketDataByte
            {
                get { return Encoding.UTF8.GetBytes( PacketDataString ); }
                set { PacketDataString = Encoding.UTF8.GetString( value ); }
            }
        }

        private UdpClient udpClient = null;
        private Thread threadAccepter = null;

        public delegate void NewMessageHandler ( Packet packet );
        public event NewMessageHandler NewMessage;

        public GMUdpClient ()
        {
            udpClient = new UdpClient();
            UdpAccepter();
        }

        public GMUdpClient ( int port )
        {
            udpClient = new UdpClient( port );
            UdpAccepter();
        }

        public int Ping( IPEndPoint endPoint )
        {
            Packet packet = new Packet()
            {
                PacketEndPoint = endPoint,
                PacketUdpAPI = new UdpAPI()
                {
                    PacketType = UdpAPI.TypePacket.Ping
                }
            };

            SendPacket( packet );

            Packet packetAnswer = new Packet() 
            { 
                PacketEndPoint = new IPEndPoint( IPAddress.Any, 0 ) 
            };

            bool isAnswerReceived = false;
            Thread thread = new Thread( () =>
            {
                packetAnswer.PacketDataByte = udpClient.Receive( ref packet.PacketEndPoint );
                isAnswerReceived = true;
            } );

            int WaitTime = 5000;
            int WaitStep = 10;
            while ( WaitTime > 0 )
            {
                if ( isAnswerReceived )
                {
                    break;
                }

                Thread.Sleep( WaitStep );
                WaitTime -= WaitStep;
            }

            if ( !isAnswerReceived )
            {
                thread.Abort();
                return -1;
            }

            if ( packetAnswer.PacketUdpAPI.PacketType == UdpAPI.TypePacket.Ping &&
                packetAnswer.PacketUdpAPI.PacketGuid == packet.PacketUdpAPI.PacketGuid )
            {
                return packetAnswer.PacketUdpAPI.PacketTimestamp - packet.PacketUdpAPI.PacketTimestamp;
            }

            return -1;
        }

        public void SendPacket ( Packet packet )
        {
            var buffer = packet.PacketDataByte;
            udpClient.Send( buffer, buffer.Length, packet.PacketEndPoint );
        }

        private void PacketParser( Packet packet )
        {
            Task.Run( () =>
            {
                switch ( packet.PacketUdpAPI.PacketType )
                {
                    case UdpAPI.TypePacket.Ping:
                        {
                            packet.PacketUdpAPI.PacketTimestamp = DateTime.Now.Millisecond;
                            SendPacket( packet );
                            break;
                        }
                    case UdpAPI.TypePacket.Message:
                        {
                            NewMessage?.Invoke( packet );
                        }
                        break;
                }
            } );
        }

        private void UdpAccepter()
        {
            threadAccepter = new Thread( () =>
            {
                while ( true )
                {
                    Packet packet = new Packet()
                    {
                        PacketEndPoint = new IPEndPoint( IPAddress.Any, 0 )
                    };
                    packet.PacketDataByte = udpClient.Receive( ref packet.PacketEndPoint );
                    PacketParser( packet );
                }
            } )
            {

            };
            threadAccepter.IsBackground = true;
            threadAccepter.Start();
        }
    }
}
