using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulo9
{
    public static class EFVersionM9
    {
        public static void Run()
        {
            bool next_opt = true;
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
                        EFShowQuotes();
                        break;
                    case 3:
                        EFGetAndManageQuote();
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
        public static void EFShowProducts() { }
        #endregion

        #region Quote Methods
        public static void EFShowQuotes() { }

        public static void EFGetAndManageQuote() { }

        public static void EFDeleteQuote() { }

        public static void EFShowQuoteHeader() { }
        #endregion

        #region QuoteDetail Methods
        public static void EFShowAndManageQuoteDetails() { }

        public static void EFEditQuantityQuoteDetail() { }

        public static void EFEditUnitCostQuoteDetail() { }

        public static void EFDeleteQuoteDetail() { }

        public static void EFAddQuoteDetail() { }
        #endregion
    }
}
