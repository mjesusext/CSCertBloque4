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
                        //WriteFileFromPromptV1();
                        WriteFileFromPromptV2();
                        break;
                    case 2:
                        //ReadFileFromPromptV1();
                        ReadFileFromPromptV2();
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

        private static void WriteFileFromPromptV1()
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
            catch(Exception e)
            {
                Console.WriteLine("Error creando fichero (o abriendo si existe). Retorno al menu. Detalle error: {0}", e.Message);
                return;
            }
        }

        private static void ReadFileFromPromptV1()
        {
            string fileName;
            string filePath;
            FileStream fs;
            byte[] textBytes;

            Console.Write("Introduzca nombre de fichero: ");
            fileName = Console.ReadLine();

            Console.Write("Introduzca ruta: ");
            filePath = Console.ReadLine();

            try
            {
                using (fs = new FileStream(filePath + "\\" + fileName + ".txt", FileMode.Open, FileAccess.Read))
                {
                    //Hay que pasar el array de bytes dimensionado al método de lectura para que vuelque. No lo instancia internamente
                    textBytes = new byte[(int)fs.Length];

                    fs.Read(textBytes, 0, (int)fs.Length - 1);
                    Console.Write(Encoding.Unicode.GetString(textBytes));
                }

                Console.WriteLine("");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error creando fichero (o abriendo si existe). Retorno al menu. Detalle error: {0}", e.Message);
                return;
            }
        }

        private static void WriteFileFromPromptV2()
        {
            string fileName;
            string filePath;
            StreamWriter sr;
            string exitCommand = "EXIT";
            string textRow = string.Empty;
            bool nextRow = true;

            Console.Write("Introduzca nombre de fichero: ");
            fileName = Console.ReadLine();

            Console.Write("Introduzca ruta: ");
            filePath = Console.ReadLine();

            try
            {
                using (sr = new StreamWriter(filePath + "\\" + fileName + ".txt", false, Encoding.Unicode)) //(filePath + "\\" + fileName + ".txt", FileMode.Append, FileAccess.Write))
                {
                    while (nextRow)
                    {
                        textRow = Console.ReadLine();

                        if (textRow != exitCommand)
                        {
                            sr.WriteLine(textRow);
                        }
                        else
                        {
                            nextRow = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creando fichero (o abriendo si existe). Retorno al menu. Detalle error: {0}", e.Message);
                return;
            }
        }

        private static void ReadFileFromPromptV2()
        {
            string fileName;
            string filePath;
            StreamReader sr;

            Console.Write("Introduzca nombre de fichero: ");
            fileName = Console.ReadLine();

            Console.Write("Introduzca ruta: ");
            filePath = Console.ReadLine();

            try
            {
                using (sr = new StreamReader(filePath + "\\" + fileName + ".txt", Encoding.Unicode)) 
                {
                    Console.Write(sr.ReadToEnd());
                }

                Console.WriteLine("");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creando fichero (o abriendo si existe). Retorno al menu. Detalle error: {0}", e.Message);
                return;
            }
        }

        private static void WriteFileFromPromptV3()
        {
            //StringWriter
        }

        private static void ReadFileFromPromptV3()
        {
            //StringReader
        }
    }
}
