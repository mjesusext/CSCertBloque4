using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulo9.ADO;
using Modulo9.ADO.ADOM9DatasetTableAdapters;

namespace Modulo9
{
    public static class ADOVersionM9
    {
        public static void Run()
        {
            bool next_opt = true;
            Console.WriteLine("----- Ejecución de versión ADO del programa -----\n");

            do
            {
                switch (Menu())
                {
                    case 0:
                        next_opt = false;
                        break;
                    case 1:
                        ADOShowProducts();
                        break;
                    case 2:
                        ADOShowQuotes();
                        break;
                    case 3:
                        ADOSGetAndManageQuote();
                        break;
                    default:
                        Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                        break;
                }
            }
            while (next_opt);
        }

        public static int Menu()
        {
            int menu_opt = -1;
            Console.WriteLine("\n" +
                              "1) Mostrar productos\n" +
                              "2) Mostrar pedidos\n" +
                              "3) Seleccionar pedido\n" +
                              "0) Salir\n");
            do
            {
                Console.Write("Opción: ");
            }
            while (!int.TryParse(Console.ReadLine(), out menu_opt));

            return menu_opt;
        }

        #region Product Methods
        public static void ADOShowProducts()
        {
            int counter = 0;
            string prompt_input = "";

            //Base de datos que contiene estructura de tablas
            ADOM9Dataset DataADO = new ADOM9Dataset();

            //Adaptador para descargar datos sobre tabla de dataset
            ProductTableAdapter ProdTabAdpt = new ProductTableAdapter();

            //Rellenar con todo
            ProdTabAdpt.Fill(DataADO.Product);

            foreach (ADOM9Dataset.ProductRow Producto in DataADO.Product.Rows)
            {
                Console.WriteLine($"ID: {Producto.ProductID} - Nombre producto {Producto.Name}");
                counter++;

                if(counter % 10 == 0)
                {
                    Console.Write("Introduzca X para salir. Si quiere 10 elementos siguientes, pulse una tecla: ");
                    prompt_input = Console.ReadLine();

                    if(prompt_input.ToLower() == "x")
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        #endregion

        #region Quote Methods
        public static void ADOShowQuotes() { }

        public static void ADOSGetAndManageQuote() { }

        public static void ADODeleteQuote() { }

        public static void ADOShowQuoteHeader() { }
        #endregion

        #region QuoteDetail Methods
        public static void ADOShowAndManageQuoteDetails() { }

        public static void ADOEditQuantityQuoteDetail() { }

        public static void ADOEditUnitCostQuoteDetail() { }

        public static void ADODeleteQuoteDetail() { }

        public static void ADOAddQuoteDetail() { }
        #endregion


    }
}
