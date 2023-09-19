using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace ServerFTP
{
    internal class Server
    {
        public struct ClientMessage
        {
            public IPEndPoint EndPointClient { get; set; }
            public string Message { get; set; }
        }

        private UdpClient udpServer;
        private int port;
        private bool isServerRunning;

        private Thread threadSocketAccepter;

        public delegate void NewMessageHandler ( ClientMessage message );
        public event NewMessageHandler NewMessage;

        public Server ( int port = 5000 )
        {
            this.port = port;
            isServerRunning = false;
        }
        public void ServerStart ()
        {
            try
            {
                udpServer = new UdpClient ( port );
            }
            catch ( Exception ex )
            {
                Logger.Log( "Ошибка запуска сервера: " + ex.ToString() );
            }
            isServerRunning = true;
            Logger.Log( "Сервер запущен. Порт: " + port );

            threadSocketAccepter = new Thread( () =>
            {
                SocketAccepter();
            } );
            threadSocketAccepter.IsBackground = true;
            threadSocketAccepter.Start();
         }

        private void SocketAccepter ()
        {
            while ( isServerRunning )
            {
                try
                {
                    IPEndPoint endPoint = new IPEndPoint( IPAddress.Any, 0 );
                    var buffer = udpServer.Receive( ref endPoint );

                    Task.Run( () =>
                    {
                        NewMessage?.Invoke( new ClientMessage
                        {
                            EndPointClient = endPoint,
                            Message = Encoding.UTF8.GetString( buffer )
                        } );
                    } );
                }
                catch ( Exception ex )
                {
                    Logger.Log( "Ошибка обработки пришедшего сообщения: " + ex.Message );
                }
            }
        }

        public async void SendMessage ( ClientMessage message )
        { 
            byte[] buffer = Encoding.UTF8.GetBytes( message.Message );
            int bytes = await udpServer.SendAsync( buffer, buffer.Length, message.EndPointClient );
        }

        /*
        // Этот метод принимает пакеты от всех подключенных клиентов
        private void MessageReceiver ( Socket r_client )
        {
            Thread th = new Thread( delegate ()
            {
                // Для каждого нового подключения, будет создан свой поток для приема пакетов
                while ( isServerRunning )
                {
                    try
                    {
                        // Сюда будем записывать принятые байты
                        byte[] bytes = new byte[1024];

                        foreach ( Socket s_client in clients.Keys )
                        {
                            // Принимаем 
                            r_client.Receive( bytes );
                            Console.WriteLine( "" + s_client.ToString() + ": " + encoding.GetString( bytes, 0, bytes.Length ) );
                            if ( encoding.GetString( bytes ) == "QUIT" )
                            {
                                Close();
                            }
                        }

                        if ( bytes.Length != 0 )
                        {
                            // Отсылаем принятый пакет от клиента всем клиентам
                            foreach ( Socket s_client in clients.Keys )
                            {
                                MessageSender( s_client, bytes );
                            }
                        }
                    }
                    catch ( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }
            } );
            th.Start();
            threads.Add( th );
        }*/

        // Этот метод отсылает пакет указанному  клиенту
        /*
        private void MessageSender ( Socket c_client, byte[] bytes )
        {
            Thread.Sleep( 400 );
            try
            {
                // Отправляем пакет
                c_client.Send( bytes );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }*/
        public void Close ()
        {
            threadSocketAccepter?.Abort();
            udpServer.Close();
            isServerRunning = false;
            Logger.Log( "Сервер остановлен!" );
        }
    }
}
