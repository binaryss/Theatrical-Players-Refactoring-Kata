using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using TheatricalPlayersRefactoringKata.extensions;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = GetTotalAmount(invoice, plays);
            var volumeCredits = GetVolumeCredits(invoice, plays);
            var result = $"Statement for {invoice.Customer}\n";
            CultureInfo cultureInfo = new CultureInfo("en-US");

            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = GetPerformanceAmount(play, perf);

                // print line for this order
                result += string.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(thisAmount / 100), perf.Audience);
            }
            result += $"Amount owed is {totalAmount.ToTotalAmount()}\n";
            result += $"You earned {volumeCredits} credits\n";
            return result;
        }

        public string PrintAsHtml(Invoice invoice, Dictionary<string, Play> plays)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");

            var template = File.ReadAllText("templates/Statement.html");
            template = template.Replace(Constants.Placeholder.Company, invoice.Customer);
            template = template.Replace(Constants.Placeholder.Amount, Convert.ToDecimal(GetTotalAmount(invoice, plays) / 100, cultureInfo).ToString("C"));
            return template;
        }

        private static int GetTotalAmount(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;

            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = GetPerformanceAmount(play, perf);
                totalAmount += thisAmount;
            }

            return totalAmount;
        }

        private static int GetVolumeCredits(Invoice invoice, Dictionary<string, Play> plays)
        {
            var volumeCredits = 0;

            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                volumeCredits += GetVolumeCredits(perf);
                volumeCredits += GetExtraCredit(play, perf);
            }

            return volumeCredits;
        }

        private static int GetExtraCredit(Play play, Performance perf)
        {
            if ("comedy" == play.Type)
            {
                return (int)Math.Floor((decimal)perf.Audience / 5);
            }

            return 0;
        }

        private static int GetVolumeCredits(Performance perf)
        {
            return Math.Max(perf.Audience - 30, 0);
        }

        private static int GetPerformanceAmount(Play play, Performance perf)
        {
            int thisAmount;
            switch (play.Type)
            {
                case "tragedy":
                    thisAmount = 40000;
                    if (perf.Audience > 30)
                    {
                        thisAmount += 1000 * (perf.Audience - 30);
                    }
                    break;
                case "comedy":
                    thisAmount = 30000;
                    if (perf.Audience > 20)
                    {
                        thisAmount += 10000 + 500 * (perf.Audience - 20);
                    }
                    thisAmount += 300 * perf.Audience;
                    break;
                default:
                    throw new Exception("unknown type: " + play.Type);
            }

            return thisAmount;
        }
    }
}
