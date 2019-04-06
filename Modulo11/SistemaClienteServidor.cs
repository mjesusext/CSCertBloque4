using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Modulo11
{
    public static class SistemaClienteServidor
    {
        private enum SistemaClienteServidorModo
        {
            Cliente = 1,
            Servidor = 2
        };

        public static void Run()
        {
            SistemaClienteServidorModo Mode;

            Console.WriteLine("Seleccione modo de ejecución:\n" +
                              "1) Cliente\n" +
                              "2) Servidor\n");

            do
            {
                Console.Write("Modo seleccionado: ");
            } while(!Enum.TryParse(Console.ReadLine(), out Mode));

            switch (Mode)
            {
                case SistemaClienteServidorModo.Cliente:
                    RunClient();
                    break;
                case SistemaClienteServidorModo.Servidor:
                    RunServer();
                    break;
                default:
                    break;
            }
        }

        private static void RunClient()
        {
            Console.WriteLine("Instanciando cliente... ");
            Cliente client = new Cliente();

            Console.WriteLine("Ejecutando cliente... ");
            client.Run();
        }

        private static void RunServer()
        {
            Console.WriteLine("Instanciando servidor... ");
            Servidor server = new Servidor();

            Console.WriteLine("Ejecutando servidor... ");
            server.Run();
        }

        private class Cliente
        {
            private IPEndPoint endpoint;
            private Socket socket;

            public Cliente()
            {
                endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            public void Run()
            {
                bool keeprun = true;

                try
                {
                    socket.Connect(endpoint);
                    Console.WriteLine("Cliente conectado");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error en la conexión. Detalle: {0}", e.Message);
                    return;
                }

                Console.WriteLine("Escriba sus mensajes y pulse INTRO. Para salir escriba EXIT");

                while (keeprun)
                {
                    string DataInput = Console.ReadLine();

                    if (DataInput.ToLower() == "exit")
                    {
                        keeprun = false;
                    }
                    else
                    {
                        try
                        {
                            socket.Send(Encoding.Unicode.GetBytes(DataInput));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error enviando mensaje. Detalle: {0}", e.Message);
                            break;
                        }  
                    }
                }

                socket.Shutdown(SocketShutdown.Send);
                socket.Close();
            }
        }

        private class Servidor
        {
            private IPEndPoint endpoint;
            private Socket socketListener;

            public Servidor()
            {
                endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
                socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            public void Run()
            {
                bool keeprun = true;
                bool keepsocket = true;

                try
                {
                    socketListener.Bind(endpoint);
                    socketListener.Listen(10);
                    Console.WriteLine("Servidor en escucha...");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error en la conexión. Detalle: {0}", e.Message);
                    return;
                }

                Console.WriteLine("A la espera de conexión del cliente");
                
                while (keeprun)
                {
                    Socket socketWorker = socketListener.Accept();
                    Console.WriteLine("Conexión aceptada");

                    while (keepsocket)
                    {
                        byte[] DataInput = new byte[1024];

                        try
                        {
                            socketWorker.Receive(DataInput);
                            Console.WriteLine(Encoding.Unicode.GetString(DataInput));
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine("Error recibiendo datos. Detalle: {0}", e.Message);
                            break;
                        }
                    }

                    socketWorker.Shutdown(SocketShutdown.Receive);
                    socketWorker.Close();
                }

                socketListener.Dispose();
            }
        }
    }


}
