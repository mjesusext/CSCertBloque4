using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Modulo11
{
    public static class SerializadorAlumnos
    {
        private enum OpcionSerializadorAlumnos
        {
            None = 0,
            AddStudent = 1,
            ShowStudents = 2,
            FlushStudentList = 3,
            Serialize = 4,
            Deserialize = 5,
            Close = 6
        };

        private enum OpcionSerializadorModo
        {
            Binary = 1,
            XML = 2,
            JSON = 3
        };

        private static readonly string DataPath;
        private static OpcionSerializadorModo SerMode;
        private static List<AlumnoM11> Alumnos;

        static SerializadorAlumnos()
        {
            string UserName;
            Alumnos = new List<AlumnoM11>();
            
            Console.Write("Introduza nombre usuario: ");
            UserName = Console.ReadLine();

            //Generalización del ejercicio, para permitir hacer pruebas de los diferentes modos de serialización
            #if DEBUG
            Console.WriteLine("Seleccione modo de serialización:");
            Console.WriteLine("1) Binaria\n" +
                                "2) XML\n" +
                                "3) JSON\n");
            do
            {
                Console.Write("Modo escogido: ");
            } while (!Enum.TryParse(Console.ReadLine(), out SerMode));
            #else
            SerMode = OpcionSerializadorModo.XML;
            #endif

            switch (SerMode)
            {
                case OpcionSerializadorModo.Binary:
                    DataPath = @"C:\Users\" + UserName + @"\Desktop\SerializadoAlumnosM11.dat";
                    break;
                case OpcionSerializadorModo.XML:
                    DataPath = @"C:\Users\" + UserName + @"\Desktop\SerializadoAlumnosM11.xml";
                    break;
                case OpcionSerializadorModo.JSON:
                    DataPath = @"C:\Users\" + UserName + @"\Desktop\SerializadoAlumnosM11.txt";
                    break;
                default:
                    break;
            }
        }

        public static void Run()
        {
            bool nextOp = true;
            OpcionSerializadorAlumnos opChoice = OpcionSerializadorAlumnos.None;
            Console.WriteLine("----- Serializador de Alumnos -----");

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
                    case OpcionSerializadorAlumnos.AddStudent:
                        AddStudent();
                        break;
                    case OpcionSerializadorAlumnos.ShowStudents:
                        ShowStudents();
                        break;
                    case OpcionSerializadorAlumnos.FlushStudentList:
                        FlushStudents();
                        break;
                    case OpcionSerializadorAlumnos.Serialize:
                        SerializeStudents();
                        break;
                    case OpcionSerializadorAlumnos.Deserialize:
                        DeserializeStudents();
                        break;
                    case OpcionSerializadorAlumnos.Close:
                        nextOp = false;
                        break;
                    case OpcionSerializadorAlumnos.None:
                    default:
                        break;
                }
            } while (nextOp);
        }

        private static void PromptMenu()
        {
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1) Añadir alumno\n" +
                              "2) Mostrar alumnos\n" +
                              "3) Borrar alumnos\n" +
                              "4) Serializar en archivo\n" +
                              "5) Deserializar desde archivo\n" +
                              "6) Finalizar\n");
        }

        public static void AddStudent()
        {
            AlumnoM11 al = new AlumnoM11();

            Console.Write("Introduzca nombre: ");
            al.Nombre = Console.ReadLine();

            Console.Write("Introduzca apellidos: ");
            al.Apellidos = Console.ReadLine();

            Alumnos.Add(al);
            Console.WriteLine("Alumnos en lista: {0}", Alumnos.Count);
            Console.WriteLine();
        }

        public static void ShowStudents()
        {
            Console.WriteLine();
            Console.WriteLine("Listado de alumnos: ");

            foreach (AlumnoM11 al in Alumnos)
            {
                Console.WriteLine(al.ToString());
            }
            Console.WriteLine();
        }

        public static void FlushStudents()
        {
            Alumnos.Clear();
        }

        public static void SerializeStudents()
        {
            switch (SerMode)
            {
                case OpcionSerializadorModo.Binary:
                    BinaryFormatter BinFormatter = new BinaryFormatter();

                    using (FileStream fs = new FileStream(DataPath, FileMode.Create))
                    {
                        try
                        {
                            BinFormatter.Serialize(fs, Alumnos);
                        }
                        catch (SerializationException e)
                        {
                            Console.WriteLine("Error en serialización binaria de la colección de alumnos. Mensaje: {0}", e.Message);
                        }
                    }
                    break;
                case OpcionSerializadorModo.XML:
                    //Requiere visibilidad de clase a serializar
                    //El serializador ya codifica correctamente el objeto a string (no tenemos que hacer encodings)
                    XmlSerializer Xmlformatter = new XmlSerializer(typeof(List<AlumnoM11>));

                    using (FileStream fs = new FileStream(DataPath, FileMode.Create))
                    {
                        try
                        {
                            Xmlformatter.Serialize(fs, Alumnos);
                        }
                        catch (SerializationException e)
                        {
                            Console.WriteLine("Error en serialización XML de la colección de alumnos. Mensaje: {0}", e.Message);
                        }
                    }
                    break;
                case OpcionSerializadorModo.JSON:
                    //Requiere visibilidad de clase a serializar
                    //El serializador ya codifica correctamente el objeto a string (no tenemos que hacer encodings)
                    DataContractJsonSerializer JSONFormatter = new DataContractJsonSerializer(typeof(List<AlumnoM11>));

                    using (FileStream fs = new FileStream(DataPath, FileMode.Create))
                    {
                        try
                        {
                            JSONFormatter.WriteObject(fs, Alumnos);
                        }
                        catch (SerializationException e)
                        {
                            Console.WriteLine("Error en serialización JSON de la colección de alumnos. Mensaje: {0}", e.Message);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public static void DeserializeStudents()
        {
            switch (SerMode)
            {
                case OpcionSerializadorModo.Binary:
                    BinaryFormatter BinFormatter = new BinaryFormatter();

                    using (FileStream fs = new FileStream(DataPath, FileMode.Open))
                    {
                        try
                        {
                            Alumnos = (List<AlumnoM11>)BinFormatter.Deserialize(fs);
                        }
                        catch (SerializationException e)
                        {
                            Console.WriteLine("Error en deserialización binaria del fichero de alumnos. Mensaje: {0}", e.Message);
                        }
                    }
                    break;
                case OpcionSerializadorModo.XML:
                    //Requiere visibilidad de clase a serializar
                    //El serializador ya codifica correctamente el objeto a string (no tenemos que hacer encodings)
                    XmlSerializer XmlFormatter = new XmlSerializer(typeof(List<AlumnoM11>));

                    using (FileStream fs = new FileStream(DataPath, FileMode.Open))
                    {
                        try
                        {
                            Alumnos = (List<AlumnoM11>)XmlFormatter.Deserialize(fs);
                        }
                        catch (SerializationException e)
                        {
                            Console.WriteLine("Error en deserialización XML del fichero de alumnos. Mensaje: {0}", e.Message);
                        }
                    }
                    break;
                case OpcionSerializadorModo.JSON:
                    //Requiere visibilidad de clase a serializar
                    //El serializador ya codifica correctamente el objeto a string (no tenemos que hacer encodings)
                    DataContractJsonSerializer JSONFormatter = new DataContractJsonSerializer(typeof(List<AlumnoM11>));

                    using (FileStream fs = new FileStream(DataPath, FileMode.Open))
                    {
                        try
                        {
                            Alumnos = (List<AlumnoM11>)JSONFormatter.ReadObject(fs);
                        }
                        catch (SerializationException e)
                        {
                            Console.WriteLine("Error en deserialización JSON del fichero de alumnos. Mensaje: {0}", e.Message);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    [Serializable]
    public class AlumnoM11
    {
        public enum TipoPersona
        {
            Alumno = 1,
            Profesor = 2
        };

        public string Nombre { get; set; }
        public string Apellidos { get; set; }

        [XmlAttribute]
        public DateTime FechaNacimiento = DateTime.Today;
        [XmlAttribute]
        public TipoPersona TipoPers = TipoPersona.Alumno;

        public override string ToString()
        {
            return Nombre + " " + Apellidos;
        }
    }
}
