using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
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
        //Marcamos ejemplo de código para probar: Bloque 3 - Modulo7
        private static readonly string[] CodeFiles = 
                    { @"C:\Users\mextreme\Documents\Visual Studio 2017\Projects\CertificacionCS\CSCertBloque3\Modulo7\PersonaM7.cs",
                    @"C:\Users\mextreme\Documents\Visual Studio 2017\Projects\CertificacionCS\CSCertBloque3\Modulo7\PersonaM7Collection.cs",
                    @"C:\Users\mextreme\Documents\Visual Studio 2017\Projects\CertificacionCS\CSCertBloque3\Modulo7\Program.cs"};

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
            Console.WriteLine("1) Cargar y ejecutar ficheros CS\n" +
                              "2) Ejecutar instrucciones\n" +
                              "3) Finalizar");
        }

        private static void RunFromCSFile()
        {
            CSharpCodeProvider cs_provider;
            CompilerParameters comp_params;
            CompilerResults comp_results;

            //Instanciamos un proveedor de compilación específico de CS. Si quisieramos hacerlo dinámico, trabajaríamos con su clase base CodeDomProvider
            cs_provider = new CSharpCodeProvider();

            //Definimos parametros del compilador
            comp_params = new CompilerParameters();
            comp_params.GenerateInMemory = true;
            comp_params.GenerateExecutable = false;
            comp_params.ReferencedAssemblies.Add("System.dll");
            comp_params.ReferencedAssemblies.Add("System.Linq.dll");

            //Compilamos
            comp_results = cs_provider.CompileAssemblyFromFile(comp_params, CodeFiles);

            if(comp_results.Errors.Count == 0)
            {
                Console.WriteLine("Compilacion de biblioteca en memoria correcta");
                Assembly created_assembly = comp_results.CompiledAssembly;

                //Podriamos acceder desde modulo (que solo debe existir 1 puesto que no mezclamos lenguajes) o desde el tipo identificado en el ensamblado
                //Recuperamos un objeto de la clase principal que contiene el metodo main
                Type SelType = created_assembly.GetType("Modulo7.Program");

                //Si no se dice lo contrario, solo recupera métodos publicos (sean estáticos o de instancia). Forzamos recuperar todos para ver el Main (que es privado)
                MethodInfo MainStaticMethod = SelType.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                Console.WriteLine("Invocando programa compilado en memoria....\n");
                //El método Invoke va encapsulado del siguiente modo --> Matriz de objetos por cada argumento de función.
                //Main es de un argumento que es una matriz de strings, por lo tanto hay que anidarle dicho array vacío como primer y único argumento
                MainStaticMethod.Invoke(SelType, new object[] { new string[] { } });
            }
            else
            {
                Console.WriteLine("Compilacion de biblioteca en memoria con errores");
            }
        }

        private static void RunFromConsole()
        {
        }
    }
}
