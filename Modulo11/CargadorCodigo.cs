using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Modulo11
{
    public static class CargadorCodigo
    {
        private enum CargadorCodigoModo
        {
            None = 0,
            LoadCSFile = 1,
            ReadCommands = 2,
            Close = 3
        };

        public static void Run()
        {
            bool nextOp = true;
            CargadorCodigoModo opChoice = CargadorCodigoModo.None;
            Console.WriteLine("----- Cargador de código -----");

            do
            {
                PromptMenu();
                do
                {
                    Console.Write("Introduzca un número: ");
                }
                while (!Enum.TryParse(Console.ReadLine(), out opChoice));

                switch (opChoice)
                {
                    case CargadorCodigoModo.LoadCSFile:
                        RunFromCSFile();
                        break;
                    case CargadorCodigoModo.ReadCommands:
                        RunFromConsole();
                        break;
                    case CargadorCodigoModo.Close:
                        nextOp = false;
                        break;
                    case CargadorCodigoModo.None:
                    default:
                        break;
                }
            } while (nextOp);
        }

        private static void PromptMenu()
        {
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1) Cargar y ejecutar fichero CS (autocontenido)\n" +
                              "2) Ejecutar instrucciones\n" +
                              "3) Finalizar");
        }

        private static void RunFromCSFile()
        {
        }

        private static void RunFromConsole()
        {
        }
    }
}
