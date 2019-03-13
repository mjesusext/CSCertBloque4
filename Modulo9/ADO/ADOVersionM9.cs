using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulo9.ADO;
using Modulo9.ADO.ADOM9DatasetTableAdapters;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

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
                            ADOManageOrderHeader();
                            break;
                        default:
                            Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                            break;
                    }
                }
                while (next_opt);
            }
        }

        #region Helpers And Menus
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

        private static int ManageOrderDetailMenu()
        {
            int menu_opt = -1;
            Console.WriteLine("\n" +
                              "1) Modificar cantidad detalle pedido\n" +
                              "2) Modificar importe detalle pedido\n" +
                              "3) Eliminar detalle pedido\n" +
                              "4) Añadir detalle pedido\n" +
                              "0) Volver\n");
            do
            {
                Console.Write("Opción: ");
            }
            while (!int.TryParse(Console.ReadLine(), out menu_opt));

            return menu_opt;
        }

        private static int PickSalesOrderHeader()
        {
            int OrdID = -1;
            ADOM9Dataset.SalesOrderHeaderRow SelOrdHeader;

            do
            {
                Console.Write("Introduzca identificador de Order Header: ");
            } while (!int.TryParse(Console.ReadLine(), out OrdID));

            SelOrdHeader = DataADO.SalesOrderHeader.FindBySalesOrderID(OrdID);

            if (SelOrdHeader == null)
            {
                Console.WriteLine("Dataset vacío o clave introducida no existe.\n");
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
            }

            return OrdID;
        }

        private static void GetProducts() { }

        private static void GetSalesOrderHeaders() { }

        private static void GetSalesOrderDetails() { }

        private static void SendSalesOrderHeader(ADOM9Dataset.SalesOrderHeaderRow Row)
        {
            int send_rows = 0;
            int received_rows = 0;

            using (SalesOrderHeaderTableAdapter OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter())
            {
                try
                {
                    send_rows = OrderHeadTabAdpt.Update(Row);
                    //Forzamos get del dataset para recuperar campos calculados o cambiados por trigger
                    received_rows = OrderHeadTabAdpt.Fill(DataADO.SalesOrderHeader);
                }
                catch (DBConcurrencyException e)
                {
                    Console.WriteLine("ERROR de concurrencia.");

                    switch (e.Row.RowState)
                    {
                        case DataRowState.Detached:
                            break;
                        case DataRowState.Unchanged:
                            break;
                        case DataRowState.Added:
                            break;
                        case DataRowState.Deleted:
                            //Console.WriteLine("Intento de borrado en destino de registro que ya no existe. Resuelto");
                            //Marcamos el borrado como cambio aceptado para que desaparezca del dataset
                            //e.Row.AcceptChanges();
                            //rows_OK = 1;
                            break;
                        case DataRowState.Modified:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR desconocido. Detalle: {0}", e.Message);
                }

                Console.WriteLine("Registros OrderHeader\n" +
                                  "\tenviados: {0}\n" +
                                  "\trecibidos {1}",
                                  send_rows,
                                  received_rows);
            }
        }

        private static void SendSalesOrderDetail(ADOM9Dataset.SalesOrderDetailRow Row)
        {
            int send_rows = 0;
            int received_rows = 0;

            using (SalesOrderDetailTableAdapter OrderDetailTabAdpt = new SalesOrderDetailTableAdapter())
            {
                try
                {
                    send_rows = OrderDetailTabAdpt.Update(Row);
                    //Forzamos get del dataset para recuperar campos calculados o cambiados por trigger
                    received_rows = OrderDetailTabAdpt.Fill(DataADO.SalesOrderDetail);

                }
                catch (DBConcurrencyException e)
                {
                    Console.WriteLine("ERROR de concurrencia.");

                    switch (e.Row.RowState)
                    {
                        case DataRowState.Detached:
                            break;
                        case DataRowState.Unchanged:
                            break;
                        case DataRowState.Added:
                            break;
                        case DataRowState.Deleted:
                            //Console.WriteLine("Intento de borrado en destino de registro que ya no existe. Resuelto");
                            //Marcamos el borrado como cambio aceptado para que desaparezca del dataset
                            //e.Row.AcceptChanges();
                            //rows_OK = 1;
                            break;
                        case DataRowState.Modified:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR desconocido. Detalle: {0}", e.Message);
                }

                Console.WriteLine("Registros OrderDetail\n" +
                                  "\tenviados: {0}\n" +
                                  "\trecibidos {1}",
                                  send_rows,
                                  received_rows);
            }
        }

        #endregion

        #region Product Methods
        private static void ADOShowProducts()
        {
            int counter = 0;
            string prompt_input = "";

            if (DataADO.Product.Rows == null)
            {
                using (ProductTableAdapter ProdTabAdpt = new ProductTableAdapter())
                {
                    ProdTabAdpt.Fill(DataADO.Product);
                }
            }

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
        #endregion

        #region OrderHeader Methods

        private static void ADOManageOrderHeader()
        {
            bool next_opt = true;
            int OrdHeaderID;

            if (DataADO.SalesOrderHeader.Rows.Count == 0)
            {
                using (SalesOrderHeaderTableAdapter OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter())
                {
                    OrderHeadTabAdpt.Fill(DataADO.SalesOrderHeader);
                }
            }

            OrdHeaderID = PickSalesOrderHeader();

            if (OrdHeaderID == -1)
            {
                return;
            }

            //Bucle de procesado sobre OrderHeader recuperado
            do
            {
                switch (ManageOrderHeaderMenu())
                {
                    case 0:
                        next_opt = false;
                        break;
                    case 1:
                        ADODeleteOrderHeader(OrdHeaderID);
                        next_opt = false;
                        break;
                    case 2:
                        ADOShowFullOrderHeader(OrdHeaderID);
                        break;
                    case 3:
                        ADOShowAndManageQuoteDetails(OrdHeaderID);
                        break;
                    default:
                        Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                        break;
                }
            }
            while (next_opt);
        }

        private static void ADOShowFullOrderHeader(int OrdHeaderID)
        {
            ADOM9Dataset.SalesOrderHeaderRow Row = DataADO.SalesOrderHeader.FindBySalesOrderID(OrdHeaderID);

            Console.WriteLine("Información completa de la cabecera escogida:");

            foreach (System.Data.DataColumn item in Row.Table.Columns)
            {
                Console.WriteLine("\t- {0}: {1} ", item.ColumnName, Row[item.ColumnName]);
            }

            Console.WriteLine();
        }

        private static void ADOShowOrderHeaders()
        {
            int counter = 0;
            string prompt_input = "";

            if (DataADO.SalesOrderHeader.Rows.Count == 0)
            {
                using (SalesOrderHeaderTableAdapter OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter())
                {
                    OrderHeadTabAdpt.Fill(DataADO.SalesOrderHeader);
                }
            }

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

        private static void ADODeleteOrderHeader(int OrdHeaderID)
        {
            ADOM9Dataset.SalesOrderHeaderRow Row;
            Console.Write("Confirme la eliminación del pedido escribiendo \"x\": ");

            if (Console.ReadLine().ToLower() == "x")
            {
                //Existe regla de integridad referencial que borra los detalles asociados a la cabecera si esta es eliminada
                /*ALTER TABLE [Sales].[SalesOrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID] FOREIGN KEY([SalesOrderID])
                REFERENCES [Sales].[SalesOrderHeader] ([SalesOrderID])
                ON DELETE CASCADE
                GO*/

                Row = DataADO.SalesOrderHeader.FindBySalesOrderID(OrdHeaderID);
                Row.Delete();
                SendSalesOrderHeader(Row);
                //Recuperar OrderDetails para quitar eliminados
            }
        }
        #endregion

        #region OrderDetail Methods
        private static void ADOShowAndManageQuoteDetails(int OrdHeaderID)
        {
            bool next_opt = true;
            ADOM9Dataset.SalesOrderDetailRow [] RelatedOrderDetails;
            ADOM9Dataset.SalesOrderHeaderRow HeaderRow;

            if(DataADO.SalesOrderDetail.Rows.Count == 0)
            {
                using (SalesOrderDetailTableAdapter OrderDetailTabAdpt = new SalesOrderDetailTableAdapter())
                {
                    OrderDetailTabAdpt.Fill(DataADO.SalesOrderDetail);
                }
            }

            do
            {
                //Recuperamos cabecera y mostramos información de sus detalles
                HeaderRow = DataADO.SalesOrderHeader.FindBySalesOrderID(OrdHeaderID);
                
                Console.WriteLine("Información de detalle asociado a la cabecera escogida (ID {0} - Total acumulado {1})", OrdHeaderID, HeaderRow.TotalDue);

                //Recuperamos todo el set de Order Details asociados para mostrar y seleccionar en métodos posteriores
                //IMPORTANTE: Recalculamos a cada iteración puesto que se reinstancian los objetos al pasar por FILL...
                RelatedOrderDetails = HeaderRow.GetSalesOrderDetailRows();

                foreach (ADOM9Dataset.SalesOrderDetailRow OrderDetailRow in RelatedOrderDetails)
                {
                    Console.WriteLine("ID: {0}" +
                                      "\n\t- Producto: {1} " +
                                      "\n\t- Cantidad: {2} " +
                                      "\n\t- Importe unitario: {3} ",
                                      OrderDetailRow.SalesOrderDetailID,
                                      OrderDetailRow.ProductID,
                                      OrderDetailRow.OrderQty,
                                      OrderDetailRow.UnitPrice);
                }

                switch (ManageOrderDetailMenu())
                {
                    case 0:
                        next_opt = false;
                        break;
                    case 1:
                        ADOEditQuantityQuoteDetail(RelatedOrderDetails);
                        break;
                    case 2:
                        ADOEditUnitCostQuoteDetail(RelatedOrderDetails);
                        break;
                    case 3:
                        ADODeleteQuoteDetail(RelatedOrderDetails);
                        break;
                    case 4:
                        ADOAddQuoteDetail(OrdHeaderID);
                        break;
                    default:
                        Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                        break;
                }
            }
            while (next_opt);
        }

        private static void ADOEditQuantityQuoteDetail(ADOM9Dataset.SalesOrderDetailRow[] Rows)
        {
            ADOM9Dataset.SalesOrderDetailRow Row;
            ADOM9Dataset.SalesOrderHeaderRow HeaderRow;

            int OrderDetailID = 0;
            short Quantity = 0;

            //Indicar ID de quote detail a procesar
            do
            {
                do
                {
                    Console.Write("Indique ID de order detail a editar cantidad: ");
                } while (!int.TryParse(Console.ReadLine(), out OrderDetailID));

                Row = Rows.FirstOrDefault(x => x.SalesOrderDetailID == OrderDetailID);
            } while (Row == default(ADOM9Dataset.SalesOrderDetailRow));

            do
            {
                Console.Write("Nueva cantidad: ");
            } while (!short.TryParse(Console.ReadLine(), out Quantity));

            HeaderRow = DataADO.SalesOrderHeader.FindBySalesOrderID(Row.SalesOrderID);

            Console.WriteLine("Estado de fila al solicitar modificación: {0}", Row.RowState);

            //Quitamos precio total de linea actual de la cabecera, luego recalculamos detalle y añadimos el resultado a cabecera
            //Lo hace el trigger al editar detalle
            //HeaderRow.SubTotal -= Row.LineTotal;

            Row.OrderQty = Quantity;
            //Lo hace el trigger
            //HeaderRow.SubTotal += Row.LineTotal;

            //Actualizamos detalle, dispara trigger para padre y nos traemos padre
            SendSalesOrderDetail(Row);
            //SendSalesOrderHeader(HeaderRow);

            Console.WriteLine("Estado de fila al final de comando: {0}", Row.RowState);
        }

        private static void ADOEditUnitCostQuoteDetail(ADOM9Dataset.SalesOrderDetailRow[] Rows)
        {
            ADOM9Dataset.SalesOrderDetailRow Row;
            ADOM9Dataset.SalesOrderHeaderRow HeaderRow;
            int OrderDetailID = 0;
            decimal UnitCost = 0;

            //Indicar ID de quote detail a procesar
            do
            {
                do
                {
                    Console.Write("Indique ID de order detail a editar precio unitario: ");
                } while (!int.TryParse(Console.ReadLine(), out OrderDetailID));

                Row = Rows.FirstOrDefault(x => x.SalesOrderDetailID == OrderDetailID);
            } while (Row == default(ADOM9Dataset.SalesOrderDetailRow));

            do
            {
                Console.Write("Nuevo precio unitario: ");
            } while (!decimal.TryParse(Console.ReadLine(), out UnitCost));

            HeaderRow = DataADO.SalesOrderHeader.FindBySalesOrderID(Row.SalesOrderID);

            //Quitamos precio total de linea actual de la cabecera, luego recalculamos detalle y añadimos el resultado a cabecera
            //Lo hace el trigger de OrderDetail
            //HeaderRow.SubTotal -= Row.LineTotal;

            Row.UnitPrice = UnitCost;
            //Lo hace el trigger de OrderDetail
            //HeaderRow.SubTotal += Row.LineTotal;

            //Actualizamos detalle, dispara trigger para padre y nos traemos padre
            SendSalesOrderDetail(Row);
            //SendSalesOrderHeader(HeaderRow);
        }

        private static void ADODeleteQuoteDetail(ADOM9Dataset.SalesOrderDetailRow[] Rows)
        {
            ADOM9Dataset.SalesOrderDetailRow Row;
            ADOM9Dataset.SalesOrderHeaderRow HeaderRow;
            int OrderDetailID = 0;
            
            //Indicar ID de quote detail a procesar
            do
            {
                do
                {
                    Console.Write("Indique ID de order detail a eliminar: ");
                } while (!int.TryParse(Console.ReadLine(), out OrderDetailID));

                Row = Rows.FirstOrDefault(x => x.SalesOrderDetailID == OrderDetailID);
            } while (Row == default(ADOM9Dataset.SalesOrderDetailRow));

            Console.Write("Confirme la eliminación del pedido escribiendo \"x\": ");

            if (Console.ReadLine().ToLower() == "x")
            {
                //Realizamos consultas sobre valor y marcamos fila para eliminación (y automaticamente todo lo relacionado con esta, debido a restricciones de tabla)
                //Una vez marcado para eliminación, no se puede usar el dato...

                //Lo hace el trigger de OrderDetail
                HeaderRow = DataADO.SalesOrderHeader.FindBySalesOrderID(Row.SalesOrderID);
                //HeaderRow.SubTotal -= Row.LineTotal;
                Row.Delete();

                //Actualizamos detalle, dispara trigger para padre y nos traemos padre
                SendSalesOrderDetail(Row);
                //SendSalesOrderHeader(HeaderRow);
            }
        }

        private static void ADOAddQuoteDetail(int OrdHeaderID)
        {
            int ProdID = 0;
            short ProdQty = 0;
            decimal ProdPrice = 0M;
            ADOM9Dataset.SalesOrderDetailRow Row;
            ADOM9Dataset.SalesOrderHeaderRow HeaderRow;

            //Creamos fila vacía con configuraciones a partir de definición de tabla.
            Row = DataADO.SalesOrderDetail.NewSalesOrderDetailRow();
            //Console.WriteLine("Estado de fila recien creada: {0}", Row.RowState);

            //Relacionamos el pedido y datos por defecto.
            Row.SalesOrderID = OrdHeaderID;

            //Restriccion clave compuesta producto y descuento producto
            /*ALTER TABLE [Sales].[SalesOrderDetail]  WITH CHECK 
            ADD  CONSTRAINT [FK_SalesOrderDetail_SpecialOfferProduct_SpecialOfferIDProductID] FOREIGN KEY([SpecialOfferID], [ProductID])
            REFERENCES [Sales].[SpecialOfferProduct] ([SpecialOfferID], [ProductID])
            GO*/
            
            //Implicitamente, asumimos que si no exist en la tabla de configuración de descuentos, no existira producto.
            //Por eso no peta de forma directa si metemos IDProducto mal
            Row.SpecialOfferID = 1;
            Row.UnitPriceDiscount = 0M;
            Row.rowguid = Guid.NewGuid();
            Row.ModifiedDate = DateTime.Today;

            do
            {
                Console.Write("Introduzca ID producto: ");
            }
            while (!int.TryParse(Console.ReadLine(), out ProdID));
            Row.ProductID = ProdID;

            //Comprobación forzada de producto
            if(DataADO.Product.Rows.Count == 0)
            {
                using (ProductTableAdapter ProdTabAdpt = new ProductTableAdapter())
                {
                    ProdTabAdpt.Fill(DataADO.Product);

                    if (DataADO.Product.FindByProductID(ProdID) == null)
                    {
                        Console.WriteLine("Producto no existe");
                        Row.RejectChanges();
                        return;
                    }
                }
            }

            do
            {
                Console.Write("Introduzca cantidad: ");
            }
            while (!short.TryParse(Console.ReadLine(), out ProdQty));
            Row.OrderQty = ProdQty;

            do
            {
                Console.Write("Introduzca precio unitario: ");
            }
            while (!decimal.TryParse(Console.ReadLine(), out ProdPrice));
            Row.UnitPrice = ProdPrice;
            
            Console.WriteLine("Estado de fila antes de incluir: {0}", Row.RowState);
            //Añadimos la fila instanciada al dataset/tabla para poder disponer de esta tanto en local como en remoto
            try
            {
                DataADO.SalesOrderDetail.AddSalesOrderDetailRow(Row);
            }
            catch (ConstraintException e)
            {
                Console.WriteLine("ERROR: Violación de restricción en comando Add. Detalle: {0}", e.Message);
                return;
            }
            catch (NoNullAllowedException e)
            {
                Console.WriteLine("ERROR: No se admite valor nulo de campo en comando Add. Detalle: {0}", e.Message);
                return;
            }

            Console.WriteLine("Estado de fila recien atachada: {0}", Row.RowState);
            //Recalculamos precio total de cabecera de pedido
            HeaderRow = DataADO.SalesOrderHeader.FindBySalesOrderID(OrdHeaderID);
            HeaderRow.SubTotal += ProdPrice * ProdQty;

            SendSalesOrderHeader(HeaderRow);
            SendSalesOrderDetail(Row);

            Console.WriteLine("Estado de fila al final de comando: {0}", Row.RowState);
        }
        
        #endregion
    }
}
