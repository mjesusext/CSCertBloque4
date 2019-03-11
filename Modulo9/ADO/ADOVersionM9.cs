using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulo9.ADO;
using Modulo9.ADO.ADOM9DatasetTableAdapters;
using System.Data.SqlClient;

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

        private static int ManageOrderHeaderMenu()
        {
            int menu_opt = -1;
            Console.WriteLine("\n" +
                              "1) Eliminar pedido\n" +
                              "2) Mostrar datos cabecera pedido\n" +
                              "3) Mostrar desglose de pedido\n" +
                              "0) Volver\n");
            do
            {
                Console.Write("Opción: ");
            }
            while (!int.TryParse(Console.ReadLine(), out menu_opt));

            return menu_opt;
        }

        private static void ADOSGetAndManageOrderHeader()
        {
            int OrdID = 0;
            int RetrieveMenuOpt = 0;
            bool next_opt = true;

            ADOM9Dataset.SalesOrderHeaderRow SelOrdHeader;
            SalesOrderHeaderTableAdapter OrderHeadTabAdpt;

            //Bucle de retrieve
            do
            {
                Console.Write("Introduzca identificador de Order Header: ");

                while (!int.TryParse(Console.ReadLine(), out OrdID))
                {
                    Console.WriteLine("Input incorrecto. Introduzca un número entero");
                }

                SelOrdHeader = DataADO.SalesOrderHeader.FindBySalesOrderID(OrdID);

                if (SelOrdHeader == null)
                {
                    Console.WriteLine("Dataset vacío o clave introducida no existe.\n" +
                                      "Escoja una opcion:\n" +
                                      "1) Reintentar recuperando BD previamente\n" +
                                      "2) Reintentar\n" +
                                      "3) Volver a menú pirncipal");
                    do
                    {
                        Console.Write("Opción: ");

                    } while (!int.TryParse(Console.ReadLine(), out RetrieveMenuOpt));

                    switch (RetrieveMenuOpt)
                    {
                        case 1:
                            OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter();
                            OrderHeadTabAdpt.Fill(DataADO.SalesOrderHeader);
                            OrderHeadTabAdpt.Dispose();
                            break;
                        case 2:
                            break;
                        case 3:
                            return;
                    }
                }
                else
                {
                    Console.WriteLine("ID: {0} " +
                                     "\n\t- Fecha de pedido:  {1} " +
                                     "\n\t- Núm pedido: {2} " +
                                     "\n\t- Núm cliente: {3} " +
                                     "\n\t- Importe total: {4} ",
                                     SelOrdHeader.SalesOrderID,
                                     SelOrdHeader.OrderDate,
                                     SelOrdHeader.IsNull("PurchaseOrderNumber") ? "----" : SelOrdHeader.PurchaseOrderNumber,
                                     SelOrdHeader.CustomerID,
                                     SelOrdHeader.TotalDue);
                    next_opt = false;
                }
            } while (next_opt);

            //Reset de bucle de menu
            next_opt = true;

            //Bucle de procesado sobre OrderHeader recuperado
            do
            {
                switch (ManageOrderHeaderMenu())
                {
                    case 0:
                        next_opt = false;
                        break;
                    case 1:
                        ADODeleteOrderHeader(SelOrdHeader);
                        next_opt = false;
                        break;
                    case 2:
                        ADOShowFullOrderHeader(SelOrdHeader);
                        break;
                    case 3:
                        ADOShowAndManageQuoteDetails(SelOrdHeader);
                        break;
                    default:
                        Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                        break;
                }
            }
            while (next_opt);
        }

        private static void ADODeleteOrderHeader(ADOM9Dataset.SalesOrderHeaderRow Row)
        {
            SalesOrderHeaderTableAdapter OrderHeadTabAdpt;

            //Marcamos fila para eliminación (y automaticamente todo lo relacionado con esta, debido a restricciones de tabla)
            //Console.WriteLine("Estado de fila inicial: {0}", Row.RowState);
            Row.Delete();
            //Console.WriteLine("Estado de fila al eliminar: {0}", Row.RowState);

            Console.WriteLine("Confirme la eliminación del pedido escribiendo \"x\": ");

            if(Console.ReadLine().ToLower() == "x")
            {
                //Persistimos fila eliminada en base de datos
                OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter();
                OrderHeadTabAdpt.Update(DataADO.SalesOrderHeader);
                OrderHeadTabAdpt.Dispose();
            }
            else
            {
                //Desmarcamos cambios para la fila. Podriamos hacerlo a nivel de tabla o de dataset
                Row.RejectChanges();
                //Console.WriteLine("Estado de fila al anular: {0}", Row.RowState);
            }
        }

        private static void ADOShowFullOrderHeader(ADOM9Dataset.SalesOrderHeaderRow Row)
        {
            Console.WriteLine("Información completa de la cabecera escogida:");

            foreach (System.Data.DataColumn item in Row.Table.Columns)
            {
                Console.WriteLine("\t- {0}: {1} ", item.ColumnName, Row[item.ColumnName]);
            }

            Console.WriteLine();
        }
        #endregion

        #region OrderDetail Methods
        private static void ADOShowAndManageQuoteDetails(ADOM9Dataset.SalesOrderHeaderRow Row)
        {
            Console.WriteLine("Información de detalle asociado a la cabecera escogida:");

            //Procedimiento: Sencillo pero poco eficiente
                //GetData en dataset de detalles
                //Recuperamos los registros de SalesOrderDetail a partir de la relación entre Header y Detail.
                //Esto dará una colección de filas a procesar para el resto de métodos
            

        }

        private static void ADOEditQuantityQuoteDetail() { }

        private static void ADOEditUnitCostQuoteDetail() { }

        private static void ADODeleteQuoteDetail() { }

        private static void ADOAddQuoteDetail() { }
        #endregion


    }
}
