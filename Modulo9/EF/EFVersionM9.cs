﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulo9.EF;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace Modulo9
{
    public static class EFVersionM9
    {
        private static AdventureWorks2014Entities DataEF;

        public static void Run()
        {
            bool next_opt = true;

            //Base de datos que contiene estructura de tablas
            using (DataEF = new AdventureWorks2014Entities())
            {
                Console.WriteLine("----- Ejecución de versión EF del programa -----\n");
                
                do
                {
                    switch (Menu())
                    {
                        case 0:
                            next_opt = false;
                            break;
                        case 1:
                            EFShowProducts();
                            break;
                        case 2:
                            EFShowOrderHeaders();
                            break;
                        case 3:
                            EFManageOrderHeader();
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
            SalesOrderHeader SelOrdHeader = null;

            do
            {
                Console.Write("Introduzca identificador de Order Header: ");
            } while (!int.TryParse(Console.ReadLine(), out OrdID));

            try
            {
                SelOrdHeader = DataEF.SalesOrderHeaders.Find(OrdID);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR recuperando OrderHeader seleccionEF. Detalles {0}", e.Message);
                OrdID = -1;
            }

            if (SelOrdHeader != null)
            {
                Console.WriteLine("ID: {0} " +
                                    "\n\t- Fecha de pedido:  {1} " +
                                    "\n\t- Núm pedido: {2} " +
                                    "\n\t- Núm cliente: {3} " +
                                    "\n\t- Importe total: {4} ",
                                    SelOrdHeader.SalesOrderID,
                                    SelOrdHeader.OrderDate,
                                    SelOrdHeader.PurchaseOrderNumber ?? "----",
                                    SelOrdHeader.CustomerID,
                                    SelOrdHeader.TotalDue);
            }
            else
            {
                Console.WriteLine("OrderHeader solicitado no existe");
                OrdID = -1;
            }

            return OrdID;
        }

        private static void SendSalesOrderHeader(int OrderHeaderID)
        {
            int send_rows = 0;

            //Ambito de using grande por si debemos usar adaptEFr en concurrency exception
            try
            {
                send_rows = DataEF.SaveChanges();
                Console.WriteLine("Registros OrderHeader enviados: {0}", send_rows);
                ReSyncFromHeader(OrderHeaderID);
            }
            catch (DbUpdateConcurrencyException e)
            {
                //La excepción no se lanza si en el archivo .EDMX no seteamos un campo de tabla con la propiedad del asistente "Concurrency mode = FIXED"
                Console.WriteLine("ERROR de concurrencia. Detalle: {0}", e.Message);

                Console.WriteLine("Marque X si desea resincronizar con BD: ");
                if (Console.ReadLine().ToLower() == "x")
                {
                    ReSyncFromHeader(OrderHeaderID);
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR desconocido. Detalle: {0}", e.Message);
            }
        }

        private static void SendSalesOrderDetail(int OrderDetailID, int OrderHeaderID)
        {
            int send_rows = 0;
            
            //Ambito de using grande por si debemos usar adaptEFr en concurrency exception
            try
            {
                send_rows = DataEF.SaveChanges();
                Console.WriteLine("Registros OrderDetail enviados: {0}", send_rows);
                ReSyncFromDetail(OrderDetailID, OrderHeaderID);
            }
            catch (DbUpdateConcurrencyException e)
            {
                //La excepción no se lanza si en el archivo .EDMX no seteamos un campo de tabla con la propiedad del asistente "Concurrency mode = FIXED"
                Console.WriteLine("ERROR de concurrencia. Detalle: {0}", e.Message);

                Console.WriteLine("Marque X si desea resincronizar con BD: ");
                if (Console.ReadLine().ToLower() == "x")
                {
                    ReSyncFromDetail(OrderDetailID, OrderHeaderID);
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR desconocido. Detalle: {0}", e.Message);
            }
        }
        
        private static void ReSyncFromHeader(int HeaderID)
        {
            SalesOrderHeader HeaderRow = DataEF.SalesOrderHeaders.Find(HeaderID);

            //Si es nulo es que se ha eliminado. En local ya se detecta las restricción de eliminación y no hace falta resincronizar los hijos
            if(HeaderRow != null)
            {
                DataEF.Entry(HeaderRow).Reload();
                Console.WriteLine("Registros OrderHeader resync");

                //No controlamos borrado, resincronizamos y el mismo machacará con nulos.

                DataEF.Entry(HeaderRow)
                    .Collection(x => x.SalesOrderDetails)
                    .Query()
                    .Where(x => x.SalesOrderID == HeaderID)
                    .Load();

                Console.WriteLine("Registros OrderDetail recibidos (resync)");
            }
        }
        
        private static void ReSyncFromDetail(int DetailID, int HeaderID)
        {
            SalesOrderDetail DetailRow = DataEF.SalesOrderDetails.Find(HeaderID, DetailID);
            
            if(DetailRow != null)
            {
                DataEF.Entry(DetailRow).Reload();
                Console.WriteLine("Registros OrderDetail resync");
            }
            
            //No controlamos borrado, resincronizamos y el mismo machacará con nulos.
            DataEF.Entry(DataEF.SalesOrderHeaders.Find(HeaderID)).Reload();

            Console.WriteLine("Registros OrderHeader recibidos (resync)");
        }
        #endregion

        #region Product Methods
        private static void EFShowProducts()
        {
            int counter = 0;
            string prompt_input = "";

            foreach (Product Producto in DataEF.Products)
            {
                Console.WriteLine("ID: {0} " +
                                    "- Nombre producto {1}",
                                    Producto.ProductID,
                                    Producto.Name ?? "----");
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
        private static void EFManageOrderHeader()
        {
            bool next_opt = true;
            int OrdHeaderID;

            OrdHeaderID = PickSalesOrderHeader();

            if (OrdHeaderID == -1)
            {
                return;
            }

            //Bucle de procesEF sobre OrderHeader recuperEF
            do
            {
                switch (ManageOrderHeaderMenu())
                {
                    case 0:
                        next_opt = false;
                        break;
                    case 1:
                        EFDeleteOrderHeader(OrdHeaderID);
                        next_opt = false;
                        break;
                    case 2:
                        EFShowFullOrderHeader(OrdHeaderID);
                        break;
                    case 3:
                        EFShowAndManageQuoteDetails(OrdHeaderID);
                        break;
                    default:
                        Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                        break;
                }
            }
            while (next_opt);
        }

        private static void EFShowFullOrderHeader(int OrdHeaderID)
        {
            SalesOrderHeader Row = DataEF.SalesOrderHeaders.Find(OrdHeaderID);

            Console.WriteLine("Información completa de la cabecera escogida:");
            Console.WriteLine("\t- AccountNumber: {0}" +
                              "\n\t- AccountNumber: {0}" +
                              "\n\t- BillToAddressID: {1}" +
                              "\n\t- Comment: {2}" +
                              "\n\t- CreditCardApprovalCode: {3}" +
                              "\n\t- CreditCardID: {4}" +
                              "\n\t- CurrencyRateID: {5}" +
                              "\n\t- CustomerID: {6}" +
                              "\n\t- DueDate: {7}" +
                              "\n\t- Freight: {8}" +
                              "\n\t- ModifiedDate: {9}" +
                              "\n\t- OnlineOrderFlag: {10}" +
                              "\n\t- OrderDate: {11}" +
                              "\n\t- PurchaseOrderNumber: {12}" +
                              "\n\t- RevisionNumber: {13}" +
                              "\n\t- rowguid: {14}" +
                              "\n\t- SalesOrderID: {15}" +
                              "\n\t- SalesOrderNumber: {16}" +
                              "\n\t- SalesPersonID: {17}" +
                              "\n\t- ShipDate: {18}" +
                              "\n\t- ShipMethodID: {19}" +
                              "\n\t- ShipToAddressID: {20}" +
                              "\n\t- Status: {21}" +
                              "\n\t- SubTotal: {22}" +
                              "\n\t- TaxAmt: {23}" +
                              "\n\t- TerritoryID: {24}" +
                              "\n\t- TotalDue: {25}",
                              Row.AccountNumber,
                              Row.BillToAddressID,
                              Row.Comment,
                              Row.CreditCardApprovalCode,
                              Row.CreditCardID,
                              Row.CurrencyRateID,
                              Row.CustomerID,
                              Row.DueDate,
                              Row.Freight,
                              Row.ModifiedDate,
                              Row.OnlineOrderFlag,
                              Row.OrderDate,
                              Row.PurchaseOrderNumber,
                              Row.RevisionNumber,
                              Row.rowguid,
                              Row.SalesOrderID,
                              Row.SalesOrderNumber,
                              Row.SalesPersonID,
                              Row.ShipDate,
                              Row.ShipMethodID,
                              Row.ShipToAddressID,
                              Row.Status,
                              Row.SubTotal,
                              Row.TaxAmt,
                              Row.TerritoryID,
                              Row.TotalDue);

            Console.WriteLine();
        }

        private static void EFShowOrderHeaders()
        {
            int counter = 0;
            string prompt_input = "";

            foreach (SalesOrderHeader OrderHeader in DataEF.SalesOrderHeaders)
            {

                Console.WriteLine("ID: {0} " +
                                    "\n\t- Fecha de pedido:  {1} " +
                                    "\n\t- Núm pedido: {2} " +
                                    "\n\t- Núm cliente: {3} " +
                                    "\n\t- Importe total: {4} ",
                                    OrderHeader.SalesOrderID,
                                    OrderHeader.OrderDate,
                                    OrderHeader.PurchaseOrderNumber ?? "----",
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

        private static void EFDeleteOrderHeader(int OrdHeaderID)
        {
            Console.Write("Confirme la eliminación del pedido escribiendo \"x\": ");

            if (Console.ReadLine().ToLower() == "x")
            {
                //Existe regla de integridad referencial que borra los detalles asociEFs a la cabecera si esta es eliminada
                /*ALTER TABLE [Sales].[SalesOrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID] FOREIGN KEY([SalesOrderID])
                REFERENCES [Sales].[SalesOrderHeader] ([SalesOrderID])
                ON DELETE CASCADE
                GO*/
                DataEF.SalesOrderHeaders.Remove(DataEF.SalesOrderHeaders.Find(OrdHeaderID));
                SendSalesOrderHeader(OrdHeaderID);
            }
        }
        #endregion

        #region OrderDetail Methods
        private static void EFShowAndManageQuoteDetails(int OrdHeaderID)
        {
            bool next_opt = true;
            IEnumerable<SalesOrderDetail> RelatedOrderDetails;
            SalesOrderHeader HeaderRow;

            do
            {
                //Recuperamos cabecera y mostramos información de sus detalles
                HeaderRow = DataEF.SalesOrderHeaders.Find(OrdHeaderID);

                Console.WriteLine("Información de detalle asociado a la cabecera escogida (ID {0} - Total acumulado {1})", OrdHeaderID, HeaderRow.TotalDue);

                //Recuperamos de BD los Order Details asociEFs y los buscamos a partir del Header para mostrar y seleccionar en métodos posteriores
                //Siempre encontraremos, aunque un no esta de mas controlar excepción por fallo de conexión
                //IMPORTANTE: Recalculamos a cada iteración puesto que se reinstancian los objetos al pasar por FILL...
                try
                {
                    RelatedOrderDetails = DataEF.SalesOrderDetails.Where(x => x.SalesOrderID == OrdHeaderID);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR recuperando OrderDetails del Header seleccionEF. Detalles {0}", e.Message);
                    return;
                }

                foreach (SalesOrderDetail OrderDetailRow in RelatedOrderDetails)
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
                        EFEditQuantityQuoteDetail(RelatedOrderDetails);
                        break;
                    case 2:
                        EFEditUnitCostQuoteDetail(RelatedOrderDetails);
                        break;
                    case 3:
                        EFDeleteQuoteDetail(RelatedOrderDetails);
                        break;
                    case 4:
                        EFAddQuoteDetail(OrdHeaderID);
                        break;
                    default:
                        Console.WriteLine("\n--- ERROR: Opción incorrecta. Reintentelo --\n");
                        break;
                }
            }
            while (next_opt);
        }

        private static void EFEditQuantityQuoteDetail(IEnumerable<SalesOrderDetail> Rows)
        {
            SalesOrderDetail Row;

            int OrderDetailID = 0;
            int OrderHeaderID = 0;
            short Quantity = 0;

            //Indicar ID de quote detail a procesar
            do
            {
                do
                {
                    Console.Write("Indique ID de order detail a editar cantidad: ");
                } while (!int.TryParse(Console.ReadLine(), out OrderDetailID));

                Row = Rows.FirstOrDefault(x => x.SalesOrderDetailID == OrderDetailID);
            } while (Row == default(SalesOrderDetail));

            do
            {
                Console.Write("Nueva cantidad: ");
            } while (!short.TryParse(Console.ReadLine(), out Quantity));

            OrderHeaderID = Row.SalesOrderID;
            //Quitamos precio total de linea actual de la cabecera, luego recalculamos detalle y añadimos el resultEF a cabecera
            //Recordar que el trigger actualiza la cabecera
            Row.OrderQty = Quantity;
            
            //Actualizamos detalle, dispara trigger para padre y nos traemos padre
            SendSalesOrderDetail(OrderDetailID, OrderHeaderID);
        }

        private static void EFEditUnitCostQuoteDetail(IEnumerable<SalesOrderDetail> Rows)
        {
            SalesOrderDetail Row;
            int OrderDetailID = 0;
            int OrderHeaderID = 0;
            decimal UnitCost = 0;

            //Indicar ID de quote detail a procesar
            do
            {
                do
                {
                    Console.Write("Indique ID de order detail a editar precio unitario: ");
                } while (!int.TryParse(Console.ReadLine(), out OrderDetailID));

                Row = Rows.FirstOrDefault(x => x.SalesOrderDetailID == OrderDetailID);
            } while (Row == default(SalesOrderDetail));

            do
            {
                Console.Write("Nuevo precio unitario: ");
            } while (!decimal.TryParse(Console.ReadLine(), out UnitCost));

            OrderHeaderID = Row.SalesOrderID;
            //Quitamos precio total de linea actual de la cabecera, luego recalculamos detalle y añadimos el resultado a cabecera
            //Recordar que el trigger actualiza la cabecera
            Row.UnitPrice = UnitCost;

            //Actualizamos detalle, dispara trigger para padre y nos traemos padre
            SendSalesOrderDetail(OrderDetailID, OrderHeaderID);
        }

        private static void EFDeleteQuoteDetail(IEnumerable<SalesOrderDetail> Rows)
        {
            SalesOrderDetail Row;
            SalesOrderHeader HeaderRow;
            int OrderDetailID = 0;
            int OrderHeaderID = 0;
            decimal LineTotalToSubstract = 0;

            //Indicar ID de quote detail a procesar
            do
            {
                do
                {
                    Console.Write("Indique ID de order detail a eliminar: ");
                } while (!int.TryParse(Console.ReadLine(), out OrderDetailID));

                Row = Rows.FirstOrDefault(x => x.SalesOrderDetailID == OrderDetailID);
            } while (Row == default(SalesOrderDetail));

            Console.Write("Confirme la eliminación del pedido escribiendo \"x\": ");

            if (Console.ReadLine().ToLower() == "x")
            {
                //Realizamos consultas sobre valor y marcamos fila para eliminación (y automaticamente todo lo relacionEF con esta, debido a restricciones de tabla)
                //Una vez marcEF para eliminación, no se puede usar el dato...
                //NO lo hace el trigger de OrderDetail
                LineTotalToSubstract = Row.LineTotal;
                OrderHeaderID = Row.SalesOrderID;

                DataEF.SalesOrderDetails.Remove(Row);
                
                //Actualizamos detalle pero no dispara trigger. Se trae header pero sin actualizar importes
                SendSalesOrderDetail(OrderDetailID, OrderHeaderID);

                //Segunda operación con la corrección de totales
                HeaderRow = DataEF.SalesOrderHeaders.Find(OrderHeaderID);
                HeaderRow.SubTotal -= LineTotalToSubstract;
                SendSalesOrderHeader(OrderHeaderID);
            }
        }

        private static void EFAddQuoteDetail(int OrdHeaderID)
        {
            int ProdID = 0;
            short ProdQty = 0;
            decimal ProdPrice = 0M;
            SalesOrderDetail Row;

            //Creamos fila vacía con configuraciones a partir de definición de tabla.
            Row = DataEF.SalesOrderDetails.Create();

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

            if (DataEF.Products.Find(ProdID) == null)
            {
                Console.WriteLine("Producto no existe");
                return;
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

            //Añadimos la fila instanciada al dataset/tabla para poder disponer de esta tanto en local como en remoto
            DataEF.SalesOrderDetails.Add(Row);
            
            //Recalculamos precio total de cabecera de pedido
            //Lo hace el trigger al editar detalle
            SendSalesOrderDetail(OrdHeaderID, OrdHeaderID);
        }
        #endregion
    }
}
