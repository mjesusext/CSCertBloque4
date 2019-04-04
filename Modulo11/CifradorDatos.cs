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
            //RSA = 0,
            DES = 1,
            DES3 = 2,
            Rijndael = 3,
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
                              "4) Rijndael (AES + otras variantes)\n");
            do
            {
                Console.Write("Modo seleccionado: ");
            } while (!Enum.TryParse(Console.ReadLine(), out SelAlg));

            switch (SelAlg)
            {
                //case CifradorDatosAlgoritmo.RSA:
                //    RSACryptoServiceProvider rsa_prov = new RSACryptoServiceProvider();
                //    RSAParameters rsa_params = rsa_prov.ExportParameters(true);
                //    Console.WriteLine("Valor P RSA en Base64: {0}\n" +
                //                      "Valor Q RSA en Base64: {1}\n",
                //                      Convert.ToBase64String(rsa_params.P),
                //                      Convert.ToBase64String(rsa_params.Q));
                //    algorithm = rsa_prov;

                //    break;
                case CifradorDatosAlgoritmo.DES:
                    DESCryptoServiceProvider des_prov = new DESCryptoServiceProvider();
                    //Dividimos por 8 puesto que los tamaños vienen en bits
                    des_prov.IV = GeneratedKey.GetBytes(des_prov.KeySize / 8);
                    des_prov.Key = GeneratedKey.GetBytes(des_prov.BlockSize / 8);

                    AlgBuilder = des_prov;
                    AlgTransformer = encrypt ? des_prov.CreateEncryptor() : des_prov.CreateDecryptor();
                    break;
                case CifradorDatosAlgoritmo.DES3:
                    TripleDESCryptoServiceProvider tripledes_prov = new TripleDESCryptoServiceProvider();
                    tripledes_prov.IV = GeneratedKey.GetBytes(tripledes_prov.KeySize / 8);
                    tripledes_prov.Key = GeneratedKey.GetBytes(tripledes_prov.BlockSize / 8);

                    AlgBuilder = tripledes_prov;
                    AlgTransformer = encrypt ? tripledes_prov.CreateEncryptor() : tripledes_prov.CreateDecryptor();
                    break;
                case CifradorDatosAlgoritmo.Rijndael:
                    RijndaelManaged aes_prov = new RijndaelManaged();
                    aes_prov.IV = GeneratedKey.GetBytes(aes_prov.KeySize / 8);
                    aes_prov.Key = GeneratedKey.GetBytes(aes_prov.BlockSize / 8);

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
            FileStream destFS = null;
            StreamReader origSR = null;
            StreamWriter StToCs = null;

            object AlgBuilder = null;
            CryptoStream cs = null;
            ICryptoTransform AlgTransformer = null;

            Console.Write("Introduzca ruta de fichero a cifrar: ");
            origPath = Console.ReadLine();
            destPath = Path.GetDirectoryName(origPath) + "\\" +
                       Path.GetFileName(origPath) +
                       ".dat";

            try
            {
                GetAlgorithm(ref AlgBuilder, ref AlgTransformer, true);

                destFS = new FileStream(destPath, FileMode.Create);
                cs = new CryptoStream(destFS, AlgTransformer, CryptoStreamMode.Write);

                //Stream que escriba en Crypto de golpe sin indicar limites de bytes
                StToCs = new StreamWriter(cs);
                origSR = File.OpenText(origPath);
                StToCs.Write(origSR.ReadToEnd());
            }
            catch (Exception e)
            {
                Console.WriteLine("Proceso de cifrado con errores. Detalles: {0}", e.Message);
                return;
            }
            finally
            {
                StToCs?.Dispose();
                origSR?.Dispose();
                cs?.Dispose();
                ((IDisposable)AlgBuilder)?.Dispose();
                destFS?.Dispose();
            }
            
            Console.WriteLine("Fichero encriptado");
        }

        private static void DecryptFile()
        {
            string origPath;
            string destPath;
            FileStream origFS = null;
            StreamReader StFromCs = null;
            StreamWriter destSW = null;

            object AlgBuilder = null;
            CryptoStream cs = null;
            ICryptoTransform AlgTransformer = null;

            Console.Write("Introduzca ruta de fichero a descifrar: ");
            origPath = Console.ReadLine();
            destPath = Path.GetDirectoryName(origPath) + "\\" +
                       Path.GetFileNameWithoutExtension(origPath);

            try
            {
                GetAlgorithm(ref AlgBuilder, ref AlgTransformer, false);

                origFS = new FileStream(origPath, FileMode.Open);
                cs = new CryptoStream(origFS, AlgTransformer, CryptoStreamMode.Read);

                //Stream que lea Crypto de golpe sin indicar limites de bytes
                StFromCs = new StreamReader(cs);
                destSW = new StreamWriter(File.Open(destPath, FileMode.Create));
                destSW.Write(StFromCs.ReadToEnd());
            }
            catch (Exception e)
            {
                Console.WriteLine("Proceso de cifrado con errores. Detalles: {0}", e.Message);
                return;
            }
            finally
            {
                StFromCs?.Close();
                destSW?.Close();
                cs?.Dispose();
                ((IDisposable)AlgBuilder)?.Dispose();
                origFS?.Dispose();
            }

            Console.WriteLine("Fichero desencriptado");
        }
    }
}
