// Written by Dr. Shane Wilson.
// The author licenses this file to you under the MIT license.
// See the LICENSE file in the solution root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portfolio.Model;
using Portfolio.Service;
using Portfolio.Service.TestDouble;

namespace Portfolio.Application
{
    public class PortfolioManager : IPortfolioSystem
    {
        /// <summary>
        /// List containing all assets purchased
        /// </summary>
        private List<Asset> _assets = new List<Asset>();
        
        /// <summary>
        /// List containing all assets that have been sold
        /// </summary>
        private List<Asset> _sales = new List<Asset>();

        /// <summary>
        /// Used in the constructor inorder to decide whether to use live or mock data
        /// </summary>
        private IMarketClient _client;

        /// <summary>
        /// The balance of the portfolio
        /// </summary>
        private decimal _balance;
         
        public decimal Balance
        {
            get { return _balance; }
            set { _balance = value; }
        }

        public List<Asset> Assets
        {
            get { return _assets; }
        }

        /// <summary>
        /// Used to obtain asset quotes in a number of different methods
        /// </summary>
        private AssetQuote _assetQuote;

        public PortfolioManager()
        {
            _client = new MockClient();
            _balance = 0;
            _assetQuote = new AssetQuote();
        }
        public PortfolioManager(decimal initialBalance)
        {
            _balance = initialBalance;
            _client = new MockClient();
            _assetQuote = new AssetQuote();
        }
        public PortfolioManager(decimal initialBalance, IMarketClient marketClient)
        {
            _balance = initialBalance;
            _client = marketClient;
            _assetQuote = new AssetQuote();
        }

        /// <summary>
        /// Adds the specified amount in USD to the total cash funds available within the portfolio 
        /// system.
        /// </summary>
        /// <param name="amount">An amount in USD to add to the portfolio</param>
        public void AddFunds(decimal amount)
        {
            if(amount <= 0) 
            {
                throw new ArgumentException("You must enter a valid amount to be added to your balance (value greater than 0)");
            }
            else
            {
                _balance += amount;
            }
        }

        /// <summary>
        /// Withdraw the specified amount in USD from the total cash funds available within the 
        /// portfolio management system.
        /// </summary>
        /// <param name="amount">the amount of money in USD to withdraw from the system.</param>
        /// <returns>True if we have successfully withdrawn the funds (sufficient funds are 
        /// available) otherwise false.</returns>
        public Boolean WithdrawFunds(decimal amount)
        {
            if (amount > _balance)
            {
                return false;
            }
            else
            {
                _balance -= amount;
                return true;
            }
        }

        /// <summary>
        /// Record a purchase of the named asset if available funds >= the total value of the 
        /// assets(stock or cryptocurrency) being purchased.The price paid should be the real live 
        /// price of the asset.
        /// </summary>
        /// <param name="assetSymbol">the name of the asset (stock symbol or cryptocurrency) to 
        /// purchase.</param>
        /// <param name="amount">the amount of the asset to purchase.</param>
        /// <returns>True if the asset is purchased successfully, otherwise False</returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool PurchaseAsset(string assetSymbol, decimal amount)
        {
            // Get a quote from the data service for the asset symbol
            // Determine if we have the funds to purchase
            _assetQuote = _client.GetQuote(assetSymbol);
            if (_assetQuote.AssetQuoteValue * amount <= _balance)
            { 
                _balance -= _assetQuote.AssetQuoteValue * amount;
                Asset AssetToBeAdded = new Asset();
                AssetToBeAdded.AssetSymbol = _assetQuote.AssetSymbol;
                AssetToBeAdded.AssetFullName = _assetQuote.AssetFullName;
                AssetToBeAdded.AssetPurchaseDateTime = _assetQuote.AssetQuoteTimeStamp;
                AssetToBeAdded.AssetType = _assetQuote.AssetType;
                AssetToBeAdded.PurchaseCost = _assetQuote.AssetQuoteValue;
                AssetToBeAdded.UnitsPurchased = (int)amount;
                _assets.Add(AssetToBeAdded);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Record a sale of the named asset (stock or cryptocurrency) at the current live market 
        /// value if we hold that asset.The sale price should be the real live price of the asset 
        /// at the time of sale retrieved from an appropriate web API. The revenue generated from 
        /// the sale should be added to the total funds available to the user. <para> Business 
        /// logic: If we hold > 1 units of the specified asset (say 10 units of Microsoft stock 
        /// MSFT), and the parameter amount is < total units of the stock, we should sell the units 
        /// that maximise our profit. Remember some of the stock could have been purchased on 
        /// different dates and therefore have been purchased at different price points.</para>
        /// </summary>
        /// <param name="assetSymbol">the name of the asset (stock symbol or cryptocurrency) to 
        /// sell.</param>
        /// <param name="amount">the amount of the asset to sell.</param>
        /// <returns>True if the asset is sold successfully, otherwise false (we may not have 
        /// that asset in our portfolio)</returns>
        public bool SellAsset(string assetSymbol, decimal amount)
        {
            decimal amountLeftToSell =  amount;
            int amountCountEach = 0; //count number of assets purchased on same days

            _assets.Sort((x,y) => //sort assets first by assetSymbol then by their purchase cost
            {
                int comp = String.Compare(x.AssetSymbol, y.AssetSymbol);
                return comp != 0 ? comp : x.PurchaseCost.CompareTo(y.PurchaseCost);
            });

            foreach (Asset asset in _assets) //count total number of units of all assets we have that match the given symbol
            {
                if (asset.AssetSymbol == assetSymbol)
                {
                    amountCountEach += asset.UnitsPurchased;
                }
            }

            if (amountCountEach >= amount && _assets.Count > 0) //check if we have enough units to sell the amount the user wishes to sell
            {
                //add sale price to balence
                _assetQuote = _client.GetQuote(assetSymbol);
                _balance += _assetQuote.AssetQuoteValue * amount;

                //while we still have an amount left to sell, iterate through our assets, removing assets we have sold all units of and/or editing the amount of purchased units of assets
                while (amountLeftToSell > 0)
                {
                    foreach (var asset in _assets.ToList()) //_assets is converted to a new list each time as otherwise the removal of the asset breaks the loop
                    {
                        if (asset.AssetSymbol == assetSymbol && amountLeftToSell > 0)
                        {
                            if (asset.UnitsPurchased < amountLeftToSell)
                            {
                                amountLeftToSell -= asset.UnitsPurchased;
                                Asset soldAsset = new();
                                soldAsset.AssetSymbol = asset.AssetSymbol;
                                soldAsset.AssetFullName = asset.AssetFullName;
                                soldAsset.AssetPurchaseDateTime = asset.AssetPurchaseDateTime;
                                soldAsset.AssetType = asset.AssetType;
                                soldAsset.PurchaseCost = asset.PurchaseCost;
                                soldAsset.UnitsPurchased = asset.UnitsPurchased;
                                soldAsset.SalePrice = _assetQuote.AssetQuoteValue;
                                _sales.Add(soldAsset);
                                _assets.Remove(asset);
                            }
                            else
                            {
                                asset.UnitsPurchased -= (int)amountLeftToSell;
                                Asset soldAsset = new();
                                soldAsset.AssetSymbol = asset.AssetSymbol;
                                soldAsset.AssetFullName = asset.AssetFullName;
                                soldAsset.AssetPurchaseDateTime = asset.AssetPurchaseDateTime;
                                soldAsset.AssetType = asset.AssetType;
                                soldAsset.PurchaseCost = asset.PurchaseCost;
                                soldAsset.UnitsPurchased = (int)amountLeftToSell;
                                soldAsset.SalePrice = _assetQuote.AssetQuoteValue;
                                _sales.Add(soldAsset);
                                amountLeftToSell -= amount;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieve 'live' quote data for the assets within the list assetNames from the online
        /// exchange. In test mode live data is retrieved from the MockClient test double. 
        /// </summary>
        /// <param name="assetNames">a list of asset symbols for example, "Bitcoin-USD", "Appl", 
        /// "TSLA"</param>
        /// <returns> A list of AssetQuote objects. Return an empty list if we have no assets in 
        /// our portfolio.</returns>
        public List<AssetQuote> GetAssetInformation(List<string> assetNames)
        {
            List<AssetQuote> assetInfo = new List<AssetQuote>();
            try
            {
                assetInfo = _client.GetQuote(assetNames);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Asset Name");
                Console.WriteLine(ex.Message);
            }
            return assetInfo;
        }

        /// <summary>
        /// Return the current value of all of the assets in the portfolio based on the current 
        /// 'live' value of each asset.In test mode live data is retrieved from the MockClient test
        /// double. 
        /// </summary>
        /// <returns>the value of the portfoio in USD.</returns>
        public decimal GetPortfolioValue()
        {
            decimal portfolioValue = 0;
            foreach (Asset asset in _assets)
            {
                _assetQuote = _client.GetQuote(asset.AssetSymbol);
                portfolioValue += _assetQuote.AssetQuoteValue * asset.UnitsPurchased;        
            }
            return portfolioValue;
        }

        /// <summary>
        ///  Returns a formatted string detailing the name, symbol, average purchase price, current
        ///  value and amount of each asset within the portfolio.The difference in average purchase
        ///  price and current price should also be displayed in both USD and as a percentage.
        ///  In test mode live data is retrieved from the MockClient test double. 
        /// </summary>
        /// <returns>a string containing summary information on the assets in the portfolio.</returns>
        public string ListAllInvestements()
        {
            string investements = new("");

            foreach (var asset in _assets)
            {
                if (!investements.Contains(asset.AssetSymbol))
                {
                    decimal averageCost = new();
                    int unitCounter = new();
                     _assetQuote = _client.GetQuote(asset.AssetSymbol);
                    decimal difference = new();
                    float differencePercentage = new float();
                    foreach (var sameSymbolDiffObject in _assets)
                    {
                        if (sameSymbolDiffObject.AssetSymbol == asset.AssetSymbol)
                        {
                            averageCost += sameSymbolDiffObject.PurchaseCost * sameSymbolDiffObject.UnitsPurchased;
                            unitCounter += sameSymbolDiffObject.UnitsPurchased;
                        }
                    }
                    try
                    {
                        averageCost = averageCost / unitCounter;
                    }
                    catch (DivideByZeroException error)
                    {
                        Console.WriteLine(error.Message);
                    }
                    difference = _assetQuote.AssetQuoteValue - averageCost;
                    differencePercentage = ((float)difference / (float)averageCost)*100;

                    investements += $"Name: {asset.AssetFullName} Symbol: {asset.AssetSymbol} Average Purchase Price: ${averageCost} Current Value:" +
                        $"${_assetQuote.AssetQuoteValue} Amount of Assets: {unitCounter} Difference between Average Purchase Price and Current Value: ${difference} ({differencePercentage}%) \n";
                }
            }
            return investements;
        }

        /// <summary>
        ///  Returns a formatted string containing all of the assets within the portfolio of the 
        ///  specified asset type("stock" or "cryptocurrencies"). String contains the name, symbol,
        ///  average purchase price, current value and amount of each asset within the portfolio.
        ///  The difference in average purchase price and current price are displayed in USD and as
        ///  a percentage. In test mode live data is retrieved from the MockClient test double. 
        /// </summary>
        /// <param name="assetType">a string specifying the asset type. Valid values are "stock"
        /// or "crypto".</param>
        /// <returns>a formatted String containing summary of all of the investments within the 
        /// portfolio. </returns>
        public string ListPortfolioInvestementsByType(string assetType)
        {
            string investements = new("");

            foreach (var asset in _assets)
            {
                string tempAssetType = asset.AssetType.ToString();
                if (tempAssetType.Equals(assetType))
                {
                    if (!investements.Contains(asset.AssetSymbol))
                    {
                        decimal averageCost = new();
                        int unitCounter = new();
                        _assetQuote = _client.GetQuote(asset.AssetSymbol);
                        decimal difference = new();
                        float differencePercentage = new();
                        foreach (var sameSymbolDiffObject in _assets)
                        {
                            if (sameSymbolDiffObject.AssetSymbol == asset.AssetSymbol)
                            {
                                averageCost += sameSymbolDiffObject.PurchaseCost * sameSymbolDiffObject.UnitsPurchased;
                                unitCounter += sameSymbolDiffObject.UnitsPurchased;
                            }
                        }
                        averageCost = averageCost / unitCounter;
                        difference = _assetQuote.AssetQuoteValue - averageCost;
                        differencePercentage = ((float)difference / (float)averageCost) * 100;

                        investements += $"Name: {asset.AssetFullName} Symbol: {asset.AssetSymbol} Average Purchase Price: ${averageCost} Current Value:" +
                            $"${_assetQuote.AssetQuoteValue} Amount of Assets: {unitCounter} Difference between Average Purchase Price and Current Value: ${difference} ({differencePercentage}%) \n";
                    }
                }
            }
            return investements;
        }

        /// <summary>
        /// Retrieve a formatted String containing details on all of the assets within the 
        /// portfolio matching the assetName in full or partially. String contains the name, 
        /// symbol, average purchase price, current value and amount of each asset within the 
        /// portfolio. The difference in average purchase price and current price are displayed in 
        /// USD and as a percentage. In test mode live data is retrieved from the MockClient test 
        /// double. </summary>
        /// <param name="assetNames">a list of Strings containing asset symbols such as "MSFT" or 
        /// "BTC-USD" or full name "Bitcoin USD" or partial string "Bitco"</param>
        /// <returns>A formatted String containing summary information for the assetNames provided 
        /// in the list. Return an empty string if we have no matching assets.</returns>
        public string ListPortfolioAssetsByName(List<string> assetNames)
        {
            string allAssets = "---Assets---"; // initialise the string we will return
            //initialise variables that will be updated with information as we iterate through our assets 
            decimal totalUnits = 0;
            int assetCount = 0;
            string name = "";
            string symbol = "";
            decimal value= 0;
            List<decimal> assetValuesAverage = new List<decimal>();
            decimal assetAverage = 0;
            
            //iterate though both the given list and our assets, checking if the given name or symbol matches any in assets
            foreach(var assetName in assetNames) 
            {
                foreach(var asset in _assets)
                {
                    _assetQuote = _client.GetQuote(asset.AssetSymbol);
                    if (asset.AssetFullName.Contains(assetName) || asset.AssetSymbol.Contains(assetName))
                    {
                        totalUnits += asset.UnitsPurchased;
                        assetCount++;
                        symbol = asset.AssetSymbol;
                        name = asset.AssetFullName;
                        value = _assetQuote.AssetQuoteValue;
                        assetValuesAverage.Add(asset.PurchaseCost);
                    }
                }

                foreach(var assetValue in assetValuesAverage)//takes place outside the other inner foreach loop so that assets purchased on different days are used to calculate the average
                {
                    assetAverage += assetValue;
                }

                //can throw a DivideByZeroException if assets is empty and some of our test data has values that can trigger this exception too
                try
                {
                    assetAverage = assetAverage / assetValuesAverage.Count;
                }
                catch (Exception DivideByZeroException)
                {
                    Console.WriteLine("No assets to find average of\nException: " + DivideByZeroException);
                }

                allAssets += "\n------------";
                allAssets += "\nAsset Name: " + name;
                allAssets += "\nAsset Symbol: " + symbol;
                allAssets += "\nAsset Average Purchase Cost: " + assetAverage;
                allAssets += "\nAsset Value: " + value;
                allAssets += "\nAsset Amount: " + totalUnits;
                allAssets += "\n------------";
            }
            return allAssets;
        }

        /// <summary>
        /// Retrieve a formatted String containing summary information for all assets within the 
        /// portfolio purchased between the dates startTimeStamp and endTimeStamp.Summary 
        /// information contains the purchase price, current price, difference between the purchase
        /// and sale price(in USD and as a percentage). <para>If several units of the asset 
        /// have been purchased at different time points between the startTimeStamp and 
        /// endTimeStamp, list each asset purchase separately by date(oldest to most recent). In 
        /// test mode live data is retrieved from the MockClient test double. 
        /// </para> </summary>
        /// <param name="startDateTime">the start range date.</param>
        /// <param name="endDateTime">the end range date.</param>
        /// <param name=""></param>
        /// <returns>A formatted String containing summary information for all of the assets 
        /// purchased between the startTimeStamp and endTimeStamp.Return an empty string if we have
        /// no matching assets in our portfolio.</returns>  
        public string ListPortfolioPurchasesInRange(DateTime startDateTime, DateTime endDateTime)
        {
            String summary = "--Assets purchased in range--"; //initialise the string we will return
            //iterate through the list of assets, adding their data to the string 
            foreach (Asset asset in _assets)
            {
                _assetQuote = _client.GetQuote(asset.AssetSymbol);
                if (asset.AssetPurchaseDateTime > startDateTime && asset.AssetPurchaseDateTime < endDateTime)
                {
                    summary += "\n-------------------";
                    summary += "\nName: " + asset.AssetFullName.ToString();
                    summary += "\nDate: " + asset.AssetPurchaseDateTime.ToString();
                    summary += "\nCost: " + asset.PurchaseCost.ToString();
                    summary += "\nSale difference: " + (_assetQuote.AssetQuoteValue - asset.PurchaseCost );
                    summary += "\nSale difference(%): " + (asset.PurchaseCost / _assetQuote.AssetQuoteValue)*100;
                    summary += "\n-------------------";
                }
            }
            return summary;
        }

        /// <summary>
        /// Retrieve a formatted string containing a summary of all of the assets sales between the 
        /// dates startTimeStamp and endTimeStamp.Summary information contains the average purchase 
        /// price for each asset, the sale price and the profit or loss(in USD and as a 
        /// percentage).<para> If the several units of the asset have been sold at different time 
        /// points between the startTimeStamp and endTimeStamp, list by date(oldest to most recent)
        /// each of those individual sales.
        /// </para> </summary>
        /// <param name="startDateTime">the start range date.</param>
        /// <param name="endDateTime">the end range date.</param>
        /// <param name=""></param>
        /// <returns>A formatted String containing summary information for all of the assets 
        /// sold between the startTimeStamp and endTimeStamp.Return an empty string if we have
        /// no matching assets in our portfolio.</returns>
        public string ListPortfolioSalesInRange(DateTime startDateTime, DateTime endDateTime)
        {
            string sales = "";
            foreach (Asset asset in _sales)
            {
                decimal averageCost = new();
                int unitCounter = new();
                _assetQuote = _client.GetQuote(asset.AssetSymbol);
                decimal difference = new();
                float differencePercentage = new();
                foreach (var sameSymbolDiffObject in _sales)
                {
                    if (sameSymbolDiffObject.AssetSymbol == asset.AssetSymbol)
                    {
                        averageCost += sameSymbolDiffObject.PurchaseCost * sameSymbolDiffObject.UnitsPurchased;
                        unitCounter += sameSymbolDiffObject.UnitsPurchased;
                    }
                }
                averageCost = averageCost / unitCounter;
                difference = _assetQuote.AssetQuoteValue - averageCost;
                differencePercentage = ((float)difference / (float)averageCost) * 100;
                if (asset.AssetPurchaseDateTime > startDateTime && asset.AssetPurchaseDateTime < endDateTime)
                {
                    sales += $"Name: {asset.AssetFullName} Symbol: {asset.AssetSymbol} Average Purchase Price: ${averageCost} Sale Price: {asset.SalePrice} Profit/Loss: ${difference} ({differencePercentage}%) \n";
                }
            }
            return sales;
        }

        /// <summary>
        /// Allows us to add fictional set of historical purchases to list of assets in MainApplication
        /// </summary>
        /// <param name="assetSymbol">symbol of a purchased asset, for example AAPL, MSFT, etc.</param>
        /// <param name="assetName">name of a purchased asset, for example Apple, Tesla, etc.</param>
        /// <param name="assetType">type of a purchased asset (Equity, Currency and Cryptocurrency are all valid types)</param>
        /// <param name="amountOfUnits">amout of units purchased of a specific asset</param>
        /// <param name="datePurchased">date value to record when asset was purchased</param>
        /// <param name="assetCost">price paid for the asset at the time of purchase</param>
        public void SetPreviousPurchase(string assetSymbol, string assetName, AssetType assetType , decimal amountOfUnits, DateTime datePurchased, decimal assetCost)
        {
            Asset previouslyPurchasedAsset = new Asset();
            previouslyPurchasedAsset.AssetSymbol = assetSymbol;
            previouslyPurchasedAsset.AssetFullName = assetName;
            previouslyPurchasedAsset.AssetPurchaseDateTime = datePurchased;
            previouslyPurchasedAsset.AssetType = assetType;
            previouslyPurchasedAsset.PurchaseCost = assetCost;
            previouslyPurchasedAsset.UnitsPurchased = (int)amountOfUnits;
            _assets.Add(previouslyPurchasedAsset);
        }
    }
}
