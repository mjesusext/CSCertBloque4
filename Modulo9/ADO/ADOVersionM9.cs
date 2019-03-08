using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulo9
{
    public static class ADOVersionM9
    {
        public static string conn_str { get; set; }
        
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
                        Console.WriteLine("Opción incorrecta. Reintentelo");
                        break;
                }
            }
            while (next_opt);
        }

        public static int Menu()
        {
            int menu_opt = -1;
            Console.WriteLine("1) Mostrar productos\n" +
                              "2) Mostrar pedidos\n" +
                              "3) Seleccionar pedido\n");
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
