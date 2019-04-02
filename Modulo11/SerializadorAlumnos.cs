using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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

        private static List<AlumnoM11> Alumnos;

        static SerializadorAlumnos()
        {
            Alumnos = new List<AlumnoM11>();
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
        }

        public static void ShowStudents()
        {
            Console.WriteLine("Listado de alumnos: ");

            foreach (AlumnoM11 al in Alumnos)
            {
                Console.WriteLine(al.ToString());
            }
        }

        public static void FlushStudents()
        {
            Alumnos.Clear();
        }

        public static void SerializeStudents()
        {
            #region Binary Serialization
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(@"C:\Users\mextreme\Desktop\SerializadoAlumnosM11.dat", FileMode.Create))
            {
                try
                {
                    formatter.Serialize(fs, Alumnos);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Error en serialización de la colección de alumnos. Mensaje: {0}", e.Message);
                }
            }
            #endregion
        }

        public static void DeserializeStudents()
        {
            #region Binary Deserialization
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(@"C:\Users\mextreme\Desktop\SerializadoAlumnosM11.dat", FileMode.Open))
            {
                try
                {
                    Alumnos = (List<AlumnoM11>)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Error en deserialización del fichero de alumnos. Mensaje: {0}", e.Message);
                }
            }
            #endregion
        }
    }

    [Serializable]
    class AlumnoM11
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }

        public override string ToString()
        {
            return Nombre + " " + Apellidos;
        }
    }
}
