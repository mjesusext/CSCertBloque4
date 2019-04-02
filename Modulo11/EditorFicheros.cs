﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Modulo11
{
    public static class EditorFicheros
    {
        private enum OpcionEditorFicheros
        {
            None = 0,
            WriteFile = 1,
            ReadFile = 2,
            MoveFileOrDir = 3,
            CopyFileOrDir = 4,
            RenameFileOrDir = 5,
            DeleteFileOrDir = 6,
            Close = 7
        };

        public static void Run()
        {
            bool nextOp = true;
            OpcionEditorFicheros opChoice = OpcionEditorFicheros.None;
            Console.WriteLine("----- Editor de textos -----");

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
                    case OpcionEditorFicheros.WriteFile:
                        //WriteFileFromPromptV1();
                        WriteFileFromPromptV2();
                        break;
                    case OpcionEditorFicheros.ReadFile:
                        //ReadFileFromPromptV1();
                        ReadFileFromPromptV2();
                        break;
                    case OpcionEditorFicheros.MoveFileOrDir:
                        MoveFileOrDirectory();
                        break;
                    case OpcionEditorFicheros.CopyFileOrDir:
                        CopyFileOrDirectory();
                        break;
                    case OpcionEditorFicheros.RenameFileOrDir:
                        RenameFileOrDirectory();
                        break;
                    case OpcionEditorFicheros.DeleteFileOrDir:
                        DeleteFileOrDirectory();
                        break;
                    case OpcionEditorFicheros.Close:
                        nextOp = false;
                        break;
                    case OpcionEditorFicheros.None:
                    default:
                        break;
                }

            } while (nextOp);
        }

        private static void PromptMenu()
        {
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1) Crear fichero texto\n" +
                              "2) Leer fichero texto\n" +
                              "3) Mover fichero o directorio\n" +
                              "4) Copiar fichero o directorio\n" +
                              "5) Renombrar fichero o directorio\n" +
                              "6) Borrar fichero o directorio\n" +
                              "7) Finalizar\n");
        }

        #region Write / Read text files
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
            catch (Exception e)
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
            catch (Exception e)
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
        #endregion

        #region Move/Copy/Rename/Delete files
        private static void MoveFileOrDirectory()
        {
            //Se podria hacer con las clases estáticas, pero se debe manipular la ruta desde el mismo string para que el destino lo cree con el mismo nombre.
            //Desde las clases instanciadas, podemos recuperar como propiedad el nombre para mantenerlo (tanto en fichero como directorio)
            DirectoryInfo OrigDir = null;
            FileInfo OrigFile = null;

            string origPath, destPath;
            bool origFileOK = false;
            bool origDirOK = false;
            bool destOK = false;

            Console.Write("Introduzca nombre de fichero o directorio: ");
            origPath = Console.ReadLine();

            origFileOK = File.Exists(origPath);
            origDirOK = Directory.Exists(origPath);

            Console.Write("Introduzca directorio de destino: ");
            destPath = Console.ReadLine();

            destOK = Directory.Exists(destPath);

            //Validamos que origen y destino sean coherentes antes de continuar
            if (origDirOK && destOK)
            {
                OrigDir = new DirectoryInfo(origPath);
                OrigDir.MoveTo(destPath + "\\" + OrigDir.Name);
            }
            else if(origFileOK && destOK)
            {
                OrigFile = new FileInfo(origPath);
                OrigFile.MoveTo(destPath + "\\" + OrigFile.Name);
            }
            else
            {
                Console.WriteLine("Origen y/o destino erroneos");
                return;
            }
        }

        private static void CopyFileOrDirectory()
        {
            DirectoryInfo OrigDir = null, DestDir = null;
            FileInfo OrigFile = null;

            string origPath, destPath;
            bool origFileOK = false;
            bool origDirOK = false;
            bool destOK = false;

            Console.Write("Introduzca nombre de fichero o directorio: ");
            origPath = Console.ReadLine();

            origFileOK = File.Exists(origPath);
            origDirOK = Directory.Exists(origPath);

            Console.Write("Introduzca directorio de destino: ");
            destPath = Console.ReadLine();

            destOK = Directory.Exists(destPath);

            //Validamos que origen y destino sean coherentes antes de continuar
            if (origDirOK && destOK)
            {
                //Creamos directorio raiz destino y copiamos ficheros que esten en la raíz
                OrigDir = new DirectoryInfo(origPath);
                DestDir = Directory.CreateDirectory(destPath + "\\" + OrigFile.Name);

                foreach (FileInfo rootFile in OrigDir.GetFiles())
                {
                    rootFile.CopyTo(DestDir.FullName + "\\" + rootFile.Name);
                }

                //Iteramos para crear arbol de carpetas y ficheros internos 
                bool DrillDown = true;

                while (DrillDown)
                {
                    DirectoryInfo[] CurrentDirectories = OrigDir.GetDirectories();

                    foreach (DirectoryInfo subdir in CurrentDirectories)
                    {
                        DestDir.CreateSubdirectory(subdir.Name);
                    }
                }
            }
            else if (origFileOK && destOK)
            {
                OrigFile = new FileInfo(origPath);
                OrigFile.CopyTo(destPath + "\\" + OrigFile.Name);
            }
            else
            {
                Console.WriteLine("Origen y/o destino erroneos");
                return;
            }
        }

        private static void RenameFileOrDirectory()
        {
            //Pedir fichero o directorio a renombrar
            //Pedir nuevo nombre
            //Actuar
        }

        private static void DeleteFileOrDirectory()
        {
            //Pedir fichero o directorio a borrar
            //Pedir confirmación
            //Borrar este y sus contenidos anidados en caso de directorios
        }
        #endregion
    }
}
