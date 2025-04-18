

# 🧮 BackOffice RMS Calculator

A C#-based trading and risk calculation tool for equity, futures, and options trades — designed for back-office reporting in the financial domain. It supports batch trade processing via CSV, generates detailed trade breakdowns, and exports results in PDF format.

---

## ✨ Features

- ✅ Supports **Equity**, **Futures**, and **Options** trades
- 📥 Import multiple trades from a **CSV file**
- 🧾 Export batch results to **PDF reports**
- 🔢 Calculates:
  - Turnover
  - Gross and Net Profit
  - Total Charges (Brokerage, STT, SEBI Charges, Exchange Charges, GST, Stamp Duty)
  - Margin Requirements
- 📊 Detailed per-trade calculations and batch summaries
- 💡 Ideal for brokers, back-office teams, and traders

---

## 📁 CSV Format

Input CSV should have the following structure (without header):

```csv
TradeType,BuyPrice,SellPrice,LotSize,LotCount,MarginPercent
Equity,150.5,152.3,100,2,20
Futures,1850,1865,75,3,15
Options,120,130,50,10,0

```
