using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Modulo11
{
    public static class EditorTextos
    {
        public static void Run()
        {
            bool nextOp = true;
            int opCode = -1;
            Console.WriteLine("----- Editor de textos -----");

            do
            {
                PromptMenu();
                do
                {
                    Console.Write("Introduzca un número: ");
                }
                while (!int.TryParse(Console.ReadLine(), out opCode));
                
                switch (opCode)
                {
                    case 1:
                        WriteFileFromPrompt();
                        break;
                    case 2:
                        ReadFileFromPrompt();
                        break;
                    case 3:
                        nextOp = false;
                        break;
                    default:
                        opCode = -1;
                        break;
                }

            } while (nextOp);
        }

        private static void PromptMenu()
        {
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1) Crear fichero texto\n" +
                              "2) Leer fichero texto\n" +
                              "3) Finalizar\n");
        }

        private static void WriteFileFromPrompt()
        {
            string fileName;
            string filePath;
            FileStream fs;
            byte[] textRow = new byte[] { };
            byte[] exitCommand = Encoding.Unicode.GetBytes("EXIT\r\n");
            bool nextRow = true;

            Console.Write("Introduzca nombre de fichero: ");
            fileName = Console.ReadLine();

            Console.Write("Introduzca ruta: ");
            filePath = Console.ReadLine();

            try
            {
                using (fs = new FileStream(filePath + "\\" + fileName + ".txt", FileMode.Append, FileAccess.Write))
                {
                    while (nextRow)
                    {
                        textRow = Encoding.Unicode.GetBytes((Console.ReadLine() + "\r\n"));
                        
                        if (!textRow.SequenceEqual(exitCommand))
                        {
                            fs.Write(textRow, 0, textRow.Length);
                        }
                        else
                        {
                            nextRow = false;
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error creando fichero (o abriendo si existe). Retorno al menu");
                return;
            }
        }

        private static void ReadFileFromPrompt()
        {
            string fileName;
            string filePath;
            FileStream fs;
            byte[] textBytes = new byte[] { };

            Console.Write("Introduzca nombre de fichero: ");
            fileName = Console.ReadLine();

            Console.Write("Introduzca ruta: ");
            filePath = Console.ReadLine();

            try
            {
                using (fs = new FileStream(filePath + "\\" + fileName + ".txt", FileMode.Open, FileAccess.Read))
                {
                    fs.Read(textBytes, 0, (int)fs.Length);
                    Console.Write(textBytes);
                }

                Console.WriteLine("");
            }
            catch
            {
                Console.WriteLine("Error creando fichero (o abriendo si existe). Retorno al menu");
                return;
            }

        }
    }
}
