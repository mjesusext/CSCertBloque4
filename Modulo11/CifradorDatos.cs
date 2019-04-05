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
            RSA = 1,
            DES = 2,
            T_DES = 3,
            Rijndael = 4,
        };

        public static void Run()
        {
            bool nextOp = true;
            CifradorDatosModo opChoice = CifradorDatosModo.None;
            Console.WriteLine("----- Cifrador de ficheros -----");

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
            Console.WriteLine("1) Cifrar fichero y guardar en dat file\n" +
                              "2) Descifrar dat file y guardar en fichero original\n" +
                              "3) Finalizar\n");
        }

        private static void GetAlgorithm(ref object AlgBuilder, ref ICryptoTransform AlgTransformer, bool encrypt)
        {
            CifradorDatosAlgoritmo SelAlg;
            Rfc2898DeriveBytes GeneratedKey = new Rfc2898DeriveBytes("1234", Encoding.Unicode.GetBytes("Mi vector de inicialización (la sal)"));

            Console.WriteLine("Introduzca modo de cifrado: ");
            Console.WriteLine("1) RSA\n" +
                              "2) DES\n" +
                              "3) 3-DES\n" +
                              "4) Rijndael (AES)\n");
            do
            {
                Console.Write("Modo seleccionado: ");
            } while (!Enum.TryParse(Console.ReadLine(), out SelAlg));

            switch (SelAlg)
            {
                //Si dividimos por 8 es porque los tamaños vienen en bits
                //Primero seteamos clave a partir del tamaño de clave predeterminado. Una vez seteado, el provider setea el blocksize adecuado.
                //Aprovechamos el blocksize calculado para pasarle una sal equivalente al blocksize

                case CifradorDatosAlgoritmo.RSA:
                    RSACryptoServiceProvider rsa_prov = new RSACryptoServiceProvider();
                    RSAParameters rsa_params = rsa_prov.ExportParameters(true);
                    Console.WriteLine("Valor P RSA en Base64: {0}\n" +
                                      "Valor Q RSA en Base64: {1}\n",
                                      Convert.ToBase64String(rsa_params.P),
                                      Convert.ToBase64String(rsa_params.Q));
                    AlgBuilder = rsa_prov;

                    break;

                case CifradorDatosAlgoritmo.DES:
                    DESCryptoServiceProvider des_prov = new DESCryptoServiceProvider();
                    des_prov.Key = GeneratedKey.GetBytes(des_prov.KeySize / 8);
                    des_prov.IV = GeneratedKey.GetBytes(des_prov.BlockSize / 8);
                    
                    AlgBuilder = des_prov;
                    AlgTransformer = encrypt ? des_prov.CreateEncryptor() : des_prov.CreateDecryptor();
                    break;

                case CifradorDatosAlgoritmo.T_DES:
                    TripleDESCryptoServiceProvider tripledes_prov = new TripleDESCryptoServiceProvider();
                    tripledes_prov.Key = GeneratedKey.GetBytes(tripledes_prov.KeySize / 8);
                    tripledes_prov.IV = GeneratedKey.GetBytes(tripledes_prov.BlockSize / 8);

                    AlgBuilder = tripledes_prov;
                    AlgTransformer = encrypt ? tripledes_prov.CreateEncryptor() : tripledes_prov.CreateDecryptor();
                    break;

                case CifradorDatosAlgoritmo.Rijndael:
                    RijndaelManaged aes_prov = new RijndaelManaged();
                    aes_prov.Key = GeneratedKey.GetBytes(aes_prov.KeySize / 8);
                    aes_prov.IV = GeneratedKey.GetBytes(aes_prov.BlockSize / 8);
                    
                    AlgBuilder = aes_prov;
                    AlgTransformer = encrypt ? aes_prov.CreateEncryptor() : aes_prov.CreateDecryptor();
                    break;

                default:
                    AlgBuilder = null;
                    AlgTransformer = null;
                    break;
            }
        }

        private static void EncryptFile()
        {
            string origPath;
            string destPath;
            FileStream origFS = null;
            FileStream destFS = null;

            byte[] DataBuffer;
            object AlgBuilder = null;
            CryptoStream cs = null;
            ICryptoTransform AlgTransformer = null;

            Console.Write("Introduzca ruta de fichero a cifrar: ");
            origPath = Console.ReadLine();
            destPath = Path.GetDirectoryName(origPath) + "\\" +
                       Path.GetFileName(origPath) +
                       ".dat";

            //Recuperamos información para procesar
            using (origFS = new FileStream(origPath, FileMode.Open))
            {
                int lengthToRead = (int)origFS.Length;
                int posDataArray = 0;
                int lengthRead = -1;

                DataBuffer = new byte[lengthToRead];

                while (lengthRead != 0)
                {
                    lengthRead = origFS.Read(DataBuffer, posDataArray, lengthToRead - posDataArray);
                    posDataArray += lengthRead;
                }
            }

            //Creamos algoritmo y stream para escribir mediante objeto que transforma el dato
            try
            {
                GetAlgorithm(ref AlgBuilder, ref AlgTransformer, true);

                if(AlgTransformer != null)
                {
                    using (destFS = new FileStream(destPath, FileMode.Create))
                    {
                        using (cs = new CryptoStream(destFS, AlgTransformer, CryptoStreamMode.Write))
                        {
                            cs.Write(DataBuffer, 0, DataBuffer.Length);
                        }
                    }
                }
                else if(AlgBuilder != null)
                {
                    //En este caso estamos seguros que es RSA
                    using (destFS = new FileStream(destPath, FileMode.Create))
                    {
                        DataBuffer = ((RSACryptoServiceProvider)AlgBuilder).Encrypt(DataBuffer, false);
                        destFS.Write(DataBuffer, 0, DataBuffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Proceso de cifrado con errores. Detalles: {0}", e.Message);
                return;
            }
            finally
            {
                ((IDisposable)AlgBuilder)?.Dispose();
            }
            
            Console.WriteLine("Fichero encriptado");
        }

        private static void DecryptFile()
        {
            string origPath;
            string destPath;
            FileStream origFS = null;
            FileStream destFS = null;

            byte[] DataBuffer = null;
            object AlgBuilder = null;
            CryptoStream cs = null;
            ICryptoTransform AlgTransformer = null;

            Console.Write("Introduzca ruta de fichero a descifrar: ");
            origPath = Console.ReadLine();
            destPath = Path.GetDirectoryName(origPath) + "\\" +
                       Path.GetFileNameWithoutExtension(origPath);

            //Creamos algoritmo y stream para escribir mediante objeto que transforma el dato
            try
            {
                GetAlgorithm(ref AlgBuilder, ref AlgTransformer, false);

                if(AlgTransformer != null)
                {
                    //Recuperamos información para procesar a traves de CryptoStream
                    using (origFS = new FileStream(origPath, FileMode.Open))
                    {
                        using (cs = new CryptoStream(origFS, AlgTransformer, CryptoStreamMode.Read))
                        {
                            int lengthToRead = (int)origFS.Length;
                            int posDataArray = 0;
                            int lengthRead = -1;

                            DataBuffer = new byte[lengthToRead];

                            while (lengthRead != 0)
                            {
                                lengthRead = cs.Read(DataBuffer, posDataArray, lengthToRead - posDataArray);
                                posDataArray += lengthRead;
                            }
                        }
                    }
                }
                else if(AlgBuilder != null)
                {
                    using (origFS = new FileStream(origPath, FileMode.Open))
                    {
                        int lengthToRead = (int)origFS.Length;
                        int posDataArray = 0;
                        int lengthRead = -1;

                        DataBuffer = new byte[lengthToRead];

                        while (lengthRead != 0)
                        {
                            lengthRead = cs.Read(DataBuffer, posDataArray, lengthToRead - posDataArray);
                            posDataArray += lengthRead;
                        }
                    }

                    DataBuffer = ((RSACryptoServiceProvider)AlgBuilder).Decrypt(DataBuffer, false);
                }
                
                //Escribimos resultado
                using (destFS = new FileStream(destPath, FileMode.Create))
                {
                    destFS.Write(DataBuffer, 0, DataBuffer.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Proceso de descifrado con errores. Detalles: {0}", e.Message);
                return;
            }
            finally
            {
                ((IDisposable)AlgBuilder)?.Dispose();
            }
            
            Console.WriteLine("Fichero desencriptado");
        }
    }
}
