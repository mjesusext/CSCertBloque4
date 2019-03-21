using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Modulo10
{
    class Program
    {
        static void Main(string[] args)
        {
            //MainV1();
            //MainV2();
            //MainV3();
            MainV4();

            Console.ReadLine();
        }

        #region Arranques
        private static void MainV1()
        {
            string valor_V1 = Metodo1_V1();
            Metodo2_V1();

            Console.WriteLine("Thread ID: {0}. Valor de método 1: {1}", Thread.CurrentThread.ManagedThreadId, valor_V1);
            Console.ReadLine();
        }

        private static async void MainV2()
        {
            Task<string> valor_V2 = Metodo1_V2();
            Metodo2_V2();

            Console.WriteLine("Thread ID: {0}. Valor de método 1: {1}", Thread.CurrentThread.ManagedThreadId, await valor_V2);
            Console.ReadLine();
        }

        private static async void MainV3()
        {
            Task<string> valor_V3 = Metodo1_V3();
            //Esperamos a final de ejecución
            //await Metodo2_V3();
            //Continuamos ejecución escribiendo valor_V3 en cuanto lo tengamos. Mientras que se lance async los mensjaes dentro de Metodo2_V3()
            Metodo2_V3();

            Console.WriteLine("Thread ID: {0}. Valor de método 1: {1}", Thread.CurrentThread.ManagedThreadId, await valor_V3);
            Console.ReadLine();
        }

        private static async void MainV4()
        {
            Task<string> valor_V3 = Metodo1_V3();
            //Esperamos a final de ejecución
            //await Metodo2_V3();
            //Continuamos ejecución escribiendo valor_V3 en cuanto lo tengamos. Mientras que se lance async los mensjaes dentro de Metodo2_V3()
            Metodo2_V4();

            Console.WriteLine("Thread ID: {0}. Valor de método 1: {1}", Thread.CurrentThread.ManagedThreadId, await valor_V3);
            Console.ReadLine();
        }
        #endregion

        #region Métodos V1
        private static string Metodo1_V1()
        {
            Console.WriteLine("Thread ID: {0}. Inicio Método 1", Thread.CurrentThread.ManagedThreadId);

            Thread.Sleep(4000);

            Console.WriteLine("Thread ID: {0}. Fin Método 1", Thread.CurrentThread.ManagedThreadId);
            return "Resultado Método 1";
        }

        private static void Metodo2_V1()
        {
            string[] valores =
            {
                "Método 2 - Valor 1",
                "Método 2 - Valor 2",
                "Método 2 - Valor 3",
                "Método 2 - Valor 4",
                "Método 2 - Valor 5",
                "Método 2 - Valor 6",
                "Método 2 - Valor 7",
                "Método 2 - Valor 8",
                "Método 2 - Valor 9",
                "Método 2 - Valor 10"
            };

            foreach (string valor in valores)
            {
                Console.WriteLine("Thread ID: {0}. {1}", Thread.CurrentThread.ManagedThreadId, valor);
            }
        }

        #endregion

        #region Métodos V2
        private static async Task<string> Metodo1_V2()
        {
            Console.WriteLine("Thread ID: {0}. Inicio Método 1", Thread.CurrentThread.ManagedThreadId);

            await Task.Delay(4000);

            Console.WriteLine("Thread ID: {0}. Fin Método 1", Thread.CurrentThread.ManagedThreadId);
            return "Resultado Método 1";
        }

        private static async Task Metodo2_V2()
        {
            string[] valores =
            {
                "Método 2 - Valor 1",
                "Método 2 - Valor 2",
                "Método 2 - Valor 3",
                "Método 2 - Valor 4",
                "Método 2 - Valor 5",
                "Método 2 - Valor 6",
                "Método 2 - Valor 7",
                "Método 2 - Valor 8",
                "Método 2 - Valor 9",
                "Método 2 - Valor 10"
            };

            foreach (string valor in valores)
            {
                Console.WriteLine("Thread ID: {0}. {1}", Thread.CurrentThread.ManagedThreadId, valor);
            }
        }
        #endregion

        #region Métodos V3
        private static async Task<string> Metodo1_V3()
        {
            Console.WriteLine("Thread ID: {0}. Inicio Método 1", Thread.CurrentThread.ManagedThreadId);

            await Task.Delay(4000);

            Console.WriteLine("Thread ID: {0}. Fin Método 1", Thread.CurrentThread.ManagedThreadId);
            return "Resultado Método 1";
        }

        private static async Task Metodo2_V3()
        {
            string[] valores =
            {
                "Método 2 - Valor 1",
                "Método 2 - Valor 2",
                "Método 2 - Valor 3",
                "Método 2 - Valor 4",
                "Método 2 - Valor 5",
                "Método 2 - Valor 6",
                "Método 2 - Valor 7",
                "Método 2 - Valor 8",
                "Método 2 - Valor 9",
                "Método 2 - Valor 10"
            };

            foreach (string valor in valores)
            {
                Console.WriteLine("Thread ID: {0}. {1}", Thread.CurrentThread.ManagedThreadId, valor);
                await Task.Delay(1000);
            }
        }
        #endregion

        #region Métodos V4
        private static async Task Metodo2_V4()
        {
            string[] valores =
            {
                "Método 2 - Valor 1",
                "Método 2 - Valor 2",
                "Método 2 - Valor 3",
                "Método 2 - Valor 4",
                "Método 2 - Valor 5",
                "Método 2 - Valor 6",
                "Método 2 - Valor 7",
                "Método 2 - Valor 8",
                "Método 2 - Valor 9",
                "Método 2 - Valor 10"
            };

            Parallel.ForEach(
                valores, 
                async (x) => {
                    await Task.Delay(5000);
                    Console.WriteLine("Thread ID: {0}. {1}", Thread.CurrentThread.ManagedThreadId, x);
                }
            );
        }
        #endregion
    }
}
