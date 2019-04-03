using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Modulo11
{
    public static class CifradorDatos
    {
        private enum CifradorDatosModo
        {
            None = 0,
            Encrypt = 1,
            Decrypt = 2,
            Close = 3
        };

        private enum CifradorDatosAlgoritmo
        {
            RSA = 0,
            DES = 1,
            DES3 = 2,
            Rijndael = 3,
        };

        public static void Run()
        {
            bool nextOp = true;
            CifradorDatosModo opChoice = CifradorDatosModo.None;
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
                    
                    case CifradorDatosModo.Encrypt:
                        EncryptFile();
                        break;
                    case CifradorDatosModo.Decrypt:
                        DecryptFile();
                        break;
                    case CifradorDatosModo.Close:
                        nextOp = false;
                        break;
                    case CifradorDatosModo.None:
                    default:
                        break;
                }
            } while (nextOp);
        }

        private static void PromptMenu()
        {
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1) Cifrar fichero y guardar en txt file\n" +
                              "2) Descifrar txt file y guardar en fichero original\n" +
                              "3) Finalizar\n");
        }

        private static void EncryptFile()
        {
            string origPath;
            string destPath;
            FileStream origFS, destFS;
            CifradorDatosAlgoritmo SelAlg;

            Console.Write("Introduzca ruta de fichero a cifrar: ");
            origPath = Console.ReadLine();
            destPath = Path.GetDirectoryName(origPath) + 
                       "Encrypted" + 
                       Path.GetFileNameWithoutExtension(origPath) +
                       ".dat";

            Console.WriteLine("Introduzca modo de cifrado: ");
            Console.WriteLine("1) RSA\n" +
                              "2) DES\n" +
                              "3) 3-DES\n" +
                              "4) Rijndael (AES + otras variantes)\n");
            do
            {
                Console.Write("Modo seleccionado: ");
            } while (!Enum.TryParse(Console.ReadLine(), out SelAlg));

            //Secuencia de usings segun el cifrador
            //using()

            Console.WriteLine("Fichero encriptado");
        }

        private static void DecryptFile()
        {

        }
    }
}
