using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            foreach(var perf in invoice.Performances) 
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                switch (play.Type) 
                {
                    case "tragedy":
                        thisAmount = 40000;
                        if (perf.Audience > 30) {
                            thisAmount += 1000 * (perf.Audience - 30);
                        }
                        break;
                    case "comedy":
                        thisAmount = 30000;
                        if (perf.Audience > 20) {
                            thisAmount += 10000 + 500 * (perf.Audience - 20);
                        }
                        thisAmount += 300 * perf.Audience;
                        break;
                    default:
                        throw new Exception("unknown type: " + play.Type);
                }
                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type) volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);

                // print line for this order
                result += string.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(thisAmount / 100), perf.Audience);
                totalAmount += thisAmount;
            }
            result += string.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += string.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }

        public string PrintAsHtml(Invoice invoice, Dictionary<string, Play> plays)
        {
            return @"<html>
  <h1>Statement for BigCo</h1>
  <table>
    <tr><th>play</th><th>seats</th><th>cost</th></tr>
    <tr><td>Hamlet</td><td>55</td><td>$650.00</td></tr>
    <tr><td>As You Like It</td><td>35</td><td>$580.00</td></tr>
    <tr><td>Othello</td><td>40</td><td>$500.00</td></tr>
  </table>
  <p>Amount owed is <em>$1,730.00</em></p>
  <p>You earned <em>47</em> credits</p>
</html>";
        }
    }
}
