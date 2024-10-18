using Portfolio.Model;

namespace Portfolio.Application
{
    /// <summary>
    /// The PortfolioSystem intface defines the core functionality of the PortfolioManager system.
    /// Add additional members to this class as necessary.
    /// </summary>
    public interface IPortfolioSystem
    {
        /// <summary>
        /// Adds the specified amount in USD to the total cash funds available within the portfolio 
        /// system.
        /// </summary>
        /// <param name="amount">An amount in USD to add to the portfolio</param>
        void AddFunds(decimal amount);

        /// <summary>
        /// Withdraw the specified amount in USD from the total cash funds available within the 
        /// portfolio management system.
        /// </summary>
        /// <param name="amount">the amount of money in USD to withdraw from the system.</param>
        /// <returns>True if we have successfully withdrawn the funds (sufficient funds are 
        /// available) otherwise false.</returns>
        Boolean WithdrawFunds(decimal amount);

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
        Boolean PurchaseAsset(String assetSymbol, decimal amount);

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
        Boolean SellAsset(String assetSymbol, decimal amount);

        /// <summary>
        /// Retrieve 'live' quote data for the assets within the list assetNames from the online
        /// exchange. In test mode live data is retrieved from the MockClient test double. 
        /// </summary>
        /// <param name="assetNames">a list of asset symbols for example, "Bitcoin-USD", "Appl", 
        /// "TSLA"</param>
        /// <returns> A list of AssetQuote objects. Return an empty list if we have no assets in 
        /// our portfolio.</returns>
        List<AssetQuote> GetAssetInformation(List<string> assetNames);

        /// <summary>
        /// Return the current value of all of the assets in the portfolio based on the current 
        /// 'live' value of each asset.In test mode live data is retrieved from the MockClient test
        /// double. 
        /// </summary>
        /// <returns>the value of the portfoio in USD.</returns>

        decimal GetPortfolioValue();
        /// <summary>
        ///  Returns a formatted string detailing the name, symbol, average purchase price, current
        ///  value and amount of each asset within the portfolio.The difference in average purchase
        ///  price and current price should also be displayed in both USD and as a percentage.
        ///  In test mode live data is retrieved from the MockClient test double. 
        /// </summary>
        /// <returns>a string containing summary information on the assets in the portfolio.</returns>
        String ListAllInvestements();

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
        String ListPortfolioInvestementsByType(String assetType);

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
        string ListPortfolioAssetsByName(List<string> assetNames);

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
        string ListPortfolioPurchasesInRange(DateTime startDateTime, DateTime endDateTime);

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
        string ListPortfolioSalesInRange(DateTime startDateTime, DateTime endDateTime);

    }
}
