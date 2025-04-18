using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
/* using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element; */

public enum Segment
{
    Equity,
    FO,
    Comodity
}

public enum TradeType
{
    Delivery = 0,
    Futures,
    Options,
    Intraday
}

public enum ComodityTradeType
{
    Futures,
    Options
}
public enum OrderSide
{
    Buy,
    Sell
}
public class TradeDetails
{
    public TradeType type { get; set; }
    public Segment segment { get; set; }
    public OrderSide orderType { get; set; }
    public decimal BuyPrice { get; set; }
    public decimal SellPrice { get; set; }
    public int LotSize
    { get; set; }
    public int LotCount { get; set; }
    public decimal MarginPercent { get; set; } // e.g. 0.2 for 20%

    public int Quantity => LotSize * LotCount;
}



public class RMSCalcultor
{
    private readonly TradeDetails tradeDetails;
    private readonly decimal BrokerageRate = 0.0003m;
    private const decimal MaxBrokerage = 20m;

    private readonly List<decimal> STTChargesList = new List<decimal> { 0.001m, 0.0125m, 0.0625m, 0.025m };

    private readonly List<decimal> ComodityTaxCharge = new List<decimal> { 0.001m, 0.005m };
    private const decimal SEBITurnOverFeesPercentage = 0.00000025m;

    private readonly List<decimal> StampDutyCharges = new List<decimal> { 0.00015m, 0.00003m, 0.00002m, 0.00003m };

    private const decimal GSTRate = 0.18m;
    private const decimal ExchangeChargeRate = 0.0000325m;


    public RMSCalcultor(TradeDetails _tradeDetails)
    {
        tradeDetails = _tradeDetails;
    }


    public decimal CalculateTurnOver(TradeType _tradeType, decimal TBV, decimal TSV)
    {
        decimal turnOverCal = 0.0m;
        switch (_tradeType)
        {
            case TradeType.Delivery:
                {
                    turnOverCal = Math.Abs(TSV - TBV);

                }
                break;
            case TradeType.Futures:
                {
                    turnOverCal = TBV + TSV;
                }
                break;
            case TradeType.Options:
                {
                    turnOverCal = TBV + TSV;
                }
                break;
            default: break;
        }
        return turnOverCal;

    }

    public decimal calculateSTTSegementWise(TradeType _tradeType, decimal TSV)
    {
        return (TSV * STTChargesList[(int)_tradeType]);
    }
    public Dictionary<string, decimal> GetResults()
    {
        int qty = tradeDetails.Quantity;
        decimal buyValue = tradeDetails.BuyPrice * qty;
        decimal sellValue = tradeDetails.SellPrice * qty;
        /* Calculte TurnOver */
        decimal turnover = CalculateTurnOver(tradeDetails.type, buyValue, sellValue);
        /* Calculte Security Transaction Tax */
        decimal stt = calculateSTTSegementWise(tradeDetails.type, sellValue);
        /* Calculate Brokerage Rate */
        decimal brokerage = Math.Min(buyValue * BrokerageRate, MaxBrokerage) + Math.Min(sellValue * BrokerageRate, MaxBrokerage);
        /* Calculate Exchage Charges Rate */
        decimal exchangeCharge = turnover * ExchangeChargeRate;
        /* SEBI TurnOver Charges */
        decimal sebiCharge = turnover * SEBITurnOverFeesPercentage;
        /* Stamp Duty Charges  */
        decimal stampDutyCharges = buyValue * StampDutyCharges[(int)tradeDetails.type];
        /* GST Rate */
        decimal gst = (brokerage + exchangeCharge) * GSTRate;

        decimal totalCharges = brokerage + stt + exchangeCharge + sebiCharge + stampDutyCharges + gst;

        return new Dictionary<string, decimal>
        {
            { "Turnover", turnover },
            { "Brokerage", brokerage },
            { "STT", stt },
            { "Exchange Charges", exchangeCharge },
            { "SEBI Charges", sebiCharge },
            { "Stamp Duty", stampDutyCharges },
            { "GST", gst },
            { "Total Charges", totalCharges },
            /* { "Gross Profit", grossProfit },
            { "Net Profit", netProfit },
            { "Margin Required", margin } */
        };
        /* Need to Add feature to calculte Gross Exposure Net Profit and  */
    }

}

public static class TradeCsvImporter
{
    public static List<TradeDetails> Import(string filePath)
    {
        var trades = new List<TradeDetails>();
        var lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            var trade = new TradeDetails
            {
                type = Enum.Parse<TradeType>(parts[0], true),
                BuyPrice = decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                SellPrice = decimal.Parse(parts[2], CultureInfo.InvariantCulture),
                LotSize = int.Parse(parts[3]),
                LotCount = int.Parse(parts[4]),
                MarginPercent = decimal.Parse(parts[5], CultureInfo.InvariantCulture) / 100
            };
            trades.Add(trade);
        }

        return trades;
    }
}




