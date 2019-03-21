using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketServer
{
    class Program
    {
        public static int Main(string[] args)
        {
            StartServer();
            return 0;
        }

        private static double calcular(int valor1, int valor2, string operacao)
        {
            double resultado = 0;

            if (operacao == "soma")
            {
                resultado = valor1 + valor2;
            }
            if (operacao == "sub")
            {
                resultado = valor1 - valor2;
            }
            if (operacao == "mult")
            {
                resultado = valor1 * valor2;
            }
            if (operacao == "div")
            {
                resultado = valor1 / valor2;
            }

            return resultado;
        }

        public static void StartServer()
        {
            //IP usado para estabelecer a ligação
            //Neste caso, iremos buscar o IP do localhost
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            try
            {
                //Criar um socket que vai usar o Protocolo TCP
                Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

                //O socket tem de estar associado com um endpoint que usa o metodo Bind
                listener.Bind(localEndPoint);
                //Especifica quantos pedidos o socket pode fazer antes de dar ao servidor uma resposta "ocupada"
                listener.Listen(10);
                
                //Espera por uma ligação
                Console.WriteLine("Waiting for a connection...");

                //Cria a ligação por socket
                Socket handler = listener.Accept();
                Console.WriteLine("Uma ligação foi efetuada!");

                //Dados enviados pelo cliente
                string operacao = null;
                int valor1 = 0;
                int valor2 = 0;
                string resposta = null;

                while (true)
                {
                    byte[] bytes = new byte[1024];
                    byte[] bytes2 = new byte[1024];
                    byte[] bytes3 = new byte[1024];
                    byte[] bytes4 = new byte[1024];

                    //Recebe a operação inserida no cliente
                    int bytesRecOperacao = handler.Receive(bytes);
                    operacao = Encoding.ASCII.GetString(bytes, 0, bytesRecOperacao);

                    //Recebe o valor1 inserido no cliente
                    int bytesrecvalor1 = handler.Receive(bytes2);
                    valor1 = Convert.ToInt32(Encoding.ASCII.GetString(bytes2, 0, bytesrecvalor1));

                    //Recebe o valor2 inserido no cliente
                    int bytesRecValor2 = handler.Receive(bytes3);
                    valor2 = Convert.ToInt32(Encoding.ASCII.GetString(bytes3, 0, bytesRecValor2));          

                    //Faz output dos valores inseridos pelo utilizador
                    Console.WriteLine("Operacao: {0}", operacao);
                    Console.WriteLine("Valor1: {0}", valor1);
                    Console.WriteLine("Valor2: {0}", valor2);

                    //Calcula a operação em conjunto com os valores inseridos pelo cliente
                    double result = calcular(valor1, valor2, operacao);

                    //Mostra o resultado da operação
                    Console.WriteLine("Resultado: {0}", result);

                    //Recebe o valor do resultado
                    byte[] resultadoByte = Encoding.ASCII.GetBytes(result.ToString());
                    handler.Send(resultadoByte);

                    //Recebe a resposta do utilizador (se pretende ou não continuar a inserir)
                    int bytesResposta = handler.Receive(bytes4);
                    resposta = Encoding.ASCII.GetString(bytes4, 0, bytesResposta);

                    //Termina o while se a resposta do utilizador for não
                    if (resposta == "n")
                    {
                        break;
                    }

                    //Espaçamento
                    Console.WriteLine("  ");               
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }
    }
}
