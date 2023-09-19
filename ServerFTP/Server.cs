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

namespace ServerFTP
{
    internal class Server
    {
        public struct ClientMessage
        { 
            public EndPoint ClientSocket { get; set; }
            public string Message { get; set; }
        }

        private const int MY_MTU = 500;

        private Socket serverSocket;
        private IPEndPoint endPoint;
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
                serverSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
                endPoint = new IPEndPoint( IPAddress.Any, port );
                serverSocket.Bind( endPoint );
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
            //Logger.Log( $"Начало прослушки порта {port}." );
            while ( isServerRunning )
            {
                try
                {
                    byte[] buffer = new byte[MY_MTU];
                    EndPoint clientEndPoint = new IPEndPoint( IPAddress.Any, 0 );
                    serverSocket.ReceiveFrom( buffer, SocketFlags.None, ref clientEndPoint );

                    string message = Encoding.UTF8.GetString( buffer );
                    NewMessage?.Invoke( new ClientMessage
                    {
                        ClientSocket = clientEndPoint,
                        Message = message
                    } );
                }
                catch ( Exception ex )
                {
                    //Logger.Log( "Ошибка обработки пришедшего сообщения: " + ex.Message );
                }
            }
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
            isServerRunning = false;
            threadSocketAccepter?.Abort();
        }
    }
}
