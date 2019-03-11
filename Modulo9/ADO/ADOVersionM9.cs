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
        private static ADOM9Dataset DataADO;

        public static void Run()
        {
            bool next_opt = true;

            //Base de datos que contiene estructura de tablas
            using (DataADO = new ADOM9Dataset())
            {
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
                            ADOShowOrderHeaders();
                            break;
                        case 3:
                            ADOSGetAndManageOrderHeader();
                            break;
                        default:
                            Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                            break;
                    }
                }
                while (next_opt);
            }
        }

        private static int Menu()
        {
            int menu_opt = -1;
            Console.WriteLine("\n" +
                              "1) Mostrar productos\n" +
                              "2) Mostrar cabecera pedido\n" +
                              "3) Seleccionar cabecera pedido\n" +
                              "0) Salir\n");
            do
            {
                Console.Write("Opción: ");
            }
            while (!int.TryParse(Console.ReadLine(), out menu_opt));

            return menu_opt;
        }

        #region Product Methods
        private static void ADOShowProducts()
        {
            int counter = 0;
            string prompt_input = "";

            //Adaptador para descargar datos sobre tabla de dataset
            using (ProductTableAdapter ProdTabAdpt = new ProductTableAdapter())
            {
                //Rellenar con todo
                ProdTabAdpt.Fill(DataADO.Product);

                foreach (ADOM9Dataset.ProductRow Producto in DataADO.Product.Rows)
                {
                    Console.WriteLine("ID: {0} " +
                                      "- Nombre producto {1}",
                                      Producto.ProductID,
                                      Producto.IsNull("Name") ? "----" : Producto.Name);
                    counter++;

                    if (counter % 10 == 0)
                    {
                        Console.Write("Introduzca X para salir. Si quiere 10 elementos siguientes, pulse una tecla: ");
                        prompt_input = Console.ReadLine();

                        if (prompt_input.ToLower() == "x")
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
        }
        #endregion

        #region OrderHeader Methods
        private static void ADOShowOrderHeaders()
        {
            int counter = 0;
            string prompt_input = "";


            using (SalesOrderHeaderTableAdapter OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter())
            {
                //Rellenar con todo
                OrderHeadTabAdpt.Fill(DataADO.SalesOrderHeader);

                foreach (ADOM9Dataset.SalesOrderHeaderRow OrderHeader in DataADO.SalesOrderHeader.Rows)
                {

                    Console.WriteLine("ID: {0} " +
                                      "\n\t- Fecha de pedido:  {1} " +
                                      "\n\t- Núm pedido: {2} " +
                                      "\n\t- Núm cliente: {3} " +
                                      "\n\t- Importe total: {4} ",
                                      OrderHeader.SalesOrderID,
                                      OrderHeader.OrderDate,
                                      OrderHeader.IsNull("PurchaseOrderNumber") ? "----" : OrderHeader.PurchaseOrderNumber,
                                      OrderHeader.CustomerID,
                                      OrderHeader.TotalDue);
                    counter++;

                    if (counter % 10 == 0)
                    {
                        Console.Write("Introduzca X para salir. Si quiere 10 elementos siguientes, pulse una tecla: ");
                        prompt_input = Console.ReadLine();

                        if (prompt_input.ToLower() == "x")
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
        }

        private static void ADOSGetAndManageOrderHeader()
        {
           
        }

        private static void ADODeleteOrderHeader() { }

        private static void ADOShowQuoteHeader() { }
        #endregion

        #region OrderDetail Methods
        private static void ADOShowAndManageQuoteDetails() { }

        private static void ADOEditQuantityQuoteDetail() { }

        private static void ADOEditUnitCostQuoteDetail() { }

        private static void ADODeleteQuoteDetail() { }

        private static void ADOAddQuoteDetail() { }
        #endregion


    }
}
