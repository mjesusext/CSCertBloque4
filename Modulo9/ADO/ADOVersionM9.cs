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

            Console.Write("Confirme la eliminación del pedido escribiendo \"x\": ");

            if(Console.ReadLine().ToLower() == "x")
            {
                //Persistimos fila eliminada en base de datos
                OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter();
                OrderHeadTabAdpt.Update(Row);
                OrderHeadTabAdpt.Dispose();
            }
            else
            {
                //Desmarcamos cambios para la fila. Podriamos hacerlo a nivel de tabla o de dataset
                Row.RejectChanges();
                //Console.WriteLine("Estado de fila al anular: {0}", Row.RowState);
            }

            //Existe regla de integridad referencial que borra los detalles asociados a la cabecera si esta es eliminada
            /*ALTER TABLE [Sales].[SalesOrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID] FOREIGN KEY([SalesOrderID])
            REFERENCES [Sales].[SalesOrderHeader] ([SalesOrderID])
            ON DELETE CASCADE
            GO*/
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
        private static void ADOShowAndManageQuoteDetails(ADOM9Dataset.SalesOrderHeaderRow QuoteHeaderRow)
        {
            bool next_opt = true;
            SalesOrderDetailTableAdapter OrderDetailTabAdpt;
            ADOM9Dataset.SalesOrderDetailRow [] RelatedOrderDetails;

            //Nos traemos todos los detalles de pedido para operar
            using (OrderDetailTabAdpt = new SalesOrderDetailTableAdapter())
            {
                OrderDetailTabAdpt.Fill(DataADO.SalesOrderDetail);
            }

            //Mostramos información
            Console.WriteLine("Información de detalle asociado a la cabecera escogida (ID {0})", QuoteHeaderRow.SalesOrderID);

            //Recuperamos todo el set de Order Details asociados para mostrar y seleccionar en métodos posteriores
            RelatedOrderDetails = QuoteHeaderRow.GetSalesOrderDetailRows();

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

            //Bucle de procesado sobre OrderDetails recuperados
            do
            {
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
                        ADOAddQuoteDetail(QuoteHeaderRow);
                        
                        //Actualizamos datos internos que no tenemos en el momento de crear (IDs internos, autonumerados...)
                        using (OrderDetailTabAdpt = new SalesOrderDetailTableAdapter())
                        {
                            OrderDetailTabAdpt.Fill(DataADO.SalesOrderDetail);
                        }

                        break;
                    default:
                        Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                        break;
                }
            }
            while (next_opt);
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

        private static void ADOEditQuantityQuoteDetail(ADOM9Dataset.SalesOrderDetailRow[] Rows)
        {
            SalesOrderDetailTableAdapter OrderDetTabAdpt;
            SalesOrderHeaderTableAdapter OrderHeadTabAdpt;
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
            try
            {
                //Quitamos precio total de linea actual de la cabecera, luego recalculamos detalle y añadimos el resultado a cabecera
                HeaderRow.SubTotal -= Row.LineTotal;

                Row.OrderQty = Quantity;
                HeaderRow.SubTotal += Row.LineTotal;

                //Actualizamos tablas
                using (OrderDetTabAdpt = new SalesOrderDetailTableAdapter())
                {
                    OrderDetTabAdpt.Update(Row);
                }

                using (OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter())
                {
                    OrderHeadTabAdpt.Update(HeaderRow);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ERROR: Argumento nulo en comando update. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("ERROR: Operación no valida. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR desconocido. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }

            Console.WriteLine("Estado de fila al final de comando: {0}", Row.RowState);
        }

        private static void ADOEditUnitCostQuoteDetail(ADOM9Dataset.SalesOrderDetailRow[] Rows)
        {
            SalesOrderDetailTableAdapter OrderDetTabAdpt;
            SalesOrderHeaderTableAdapter OrderHeadTabAdpt;
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

                Row = Rows.First(x => x.SalesOrderDetailID == OrderDetailID);
            } while (Row == null);

            do
            {
                Console.Write("Nuevo precio unitario: ");
            } while (!decimal.TryParse(Console.ReadLine(), out UnitCost));

            HeaderRow = DataADO.SalesOrderHeader.FindBySalesOrderID(Row.SalesOrderID);

            try
            {
                //Quitamos precio total de linea actual de la cabecera, luego recalculamos detalle y añadimos el resultado a cabecera
                HeaderRow.SubTotal -= Row.LineTotal;

                Row.UnitPrice = UnitCost;
                HeaderRow.SubTotal += Row.LineTotal;

                //Actualizamos tablas
                using (OrderDetTabAdpt = new SalesOrderDetailTableAdapter())
                {
                    OrderDetTabAdpt.Update(Row);
                }
                using (OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter())
                {
                    OrderHeadTabAdpt.Update(HeaderRow);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ERROR: Argumento nulo en comando update. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("ERROR: Operación no valida. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR desconocido. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }
        }

        private static void ADODeleteQuoteDetail(ADOM9Dataset.SalesOrderDetailRow[] Rows)
        {
            SalesOrderDetailTableAdapter OrderDetTabAdpt;
            SalesOrderHeaderTableAdapter OrderHeadTabAdpt;
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

            //Realizamos consultas sobre valor y marcamos fila para eliminación (y automaticamente todo lo relacionado con esta, debido a restricciones de tabla)
            //Una vez marcado para eliminación, no se puede usar el dato...
            //Console.WriteLine("Estado de fila inicial: {0}", Row.RowState);
            HeaderRow = DataADO.SalesOrderHeader.FindBySalesOrderID(Row.SalesOrderID);
            HeaderRow.SubTotal -= Row.LineTotal;

            Row.Delete();
            Console.WriteLine("Estado de fila al solicitar eliminar: {0}", Row.RowState);

            Console.Write("Confirme la eliminación del pedido escribiendo \"x\": ");

            if (Console.ReadLine().ToLower() == "x")
            {
                try
                {
                    //Actualizamos tablas
                    using (OrderDetTabAdpt = new SalesOrderDetailTableAdapter())
                    {
                        OrderDetTabAdpt.Update(Row);
                    }
                    using (OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter())
                    {
                        OrderHeadTabAdpt.Update(HeaderRow);
                    }
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ERROR: Argumento nulo en comando update. Detalle: {0}", e.Message);
                    HeaderRow.RejectChanges();
                    Row.RejectChanges();
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("ERROR: Operación no valida. Detalle: {0}", e.Message);
                    HeaderRow.RejectChanges();
                    Row.RejectChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR desconocido. Detalle: {0}", e.Message);
                    HeaderRow.RejectChanges();
                    Row.RejectChanges();
                }
            }
            else
            {
                //Desmarcamos cambios para las filas. Podriamos hacerlo a nivel de tabla o de dataset
                Row.RejectChanges();
                HeaderRow.RejectChanges();
                //Console.WriteLine("Estado de fila al anular: {0}", Row.RowState);
            }

            Console.WriteLine("Estado de fila final comando: {0}", Row.RowState);
        }

        private static void ADOAddQuoteDetail(ADOM9Dataset.SalesOrderHeaderRow HeaderRow)
        {
            int ProdID = 0;
            short ProdQty = 0;
            decimal ProdPrice = 0M;
            SalesOrderDetailTableAdapter OrderDetTabAdpt;
            SalesOrderHeaderTableAdapter OrderHeadTabAdpt;
            ADOM9Dataset.SalesOrderDetailRow Row;
            
            //Creamos fila vacía con configuraciones a partir de definición de tabla.
            Row = DataADO.SalesOrderDetail.NewSalesOrderDetailRow();
            //Console.WriteLine("Estado de fila recien creada: {0}", Row.RowState);

            //Relacionamos el pedido y datos por defecto.
            Row.SalesOrderID = HeaderRow.SalesOrderID;

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
            using (ProductTableAdapter ProdTabAdpt = new ProductTableAdapter())
            {
                ProdTabAdpt.Fill(DataADO.Product);

                if(DataADO.Product.FindByProductID(ProdID) == null)
                {
                    Console.WriteLine("Producto no existe");
                    Row.RejectChanges();
                    return;
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
            Row.UnitPrice = ProdPrice * ProdQty;
            
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
            HeaderRow.SubTotal += Row.UnitPrice;

            try
            {
                //Actualizamos tablas
                using (OrderHeadTabAdpt = new SalesOrderHeaderTableAdapter())
                {
                    OrderHeadTabAdpt.Update(HeaderRow);
                }
                using (OrderDetTabAdpt = new SalesOrderDetailTableAdapter())
                {
                    OrderDetTabAdpt.Update(Row);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ERROR: Argumento nulo en comando update. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("ERROR: Operación no valida. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }
            //Claves foraneas de tablas
            catch (Exception e)
            {
                Console.WriteLine("ERROR desconocido. Detalle: {0}", e.Message);
                Row.RejectChanges();
                HeaderRow.RejectChanges();
            }
            Console.WriteLine("Estado de fila al final de comando: {0}", Row.RowState);
        }
        #endregion
    }
}
