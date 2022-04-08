using System;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata.extensions
{
    internal static class Helper
    {
        internal static string ToTotalAmount(this int totalAmount)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            return string.Format(cultureInfo, "{0:C}", Convert.ToDecimal(totalAmount / 100));
        }
    }
}
