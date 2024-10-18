using Microsoft.Extensions.Configuration;
using Portfolio.Model;
using Portfolio.Service.Live;

namespace Portfolio.Application
{
    /// <summary>
    /// Main application class. This class should be used to initialise all necessary core 
    /// services and objects including the class that implements the IPortfolioSystem interface.
    /// </summary>
    public class MainApplication
    {
        /// <summary>
        /// AppConfig is used to access application configuration settings <see cref="AppConfig"/>.
        /// </summary>
        public AppConfig AppSettings { get; set; }
        public PortfolioManager _portfolioManager;

        public MainApplication(String appSettingsFile)
        {
            // Load the application settings;
            LoadAppSettings(appSettingsFile);
            YahooClient financeClient = new YahooClient(url: AppSettings.BaseURL, AppSettings.API_keys);
            if (!AppSettings.InTest)
            {                
                _portfolioManager = new PortfolioManager(1000,financeClient);
            }
            else
            {
                _portfolioManager = new PortfolioManager();
            }
            _portfolioManager.SetPreviousPurchase("TSLA", "Tesla Inc.", AssetType.Equity,10,new DateTime(2021,10,1), 755.22m);
            _portfolioManager.SetPreviousPurchase("AAPL", "Apple Inc.", AssetType.Equity, 20, new DateTime(2023, 3, 6), 155.576m);
            _portfolioManager.SetPreviousPurchase("NVDA", "NVIDIA Corporation", AssetType.Equity, 12, new DateTime(2021, 4, 14), 152.77m);
            _portfolioManager.SetPreviousPurchase("BTC-USD", "Bitcoin USD", AssetType.Cryptocurrency, (decimal)0.0445881, new DateTime(2021, 2, 9), 2_000m);
        }

        /// <summary>
        /// The LoadAppSettings method attempts to load the app settings from file and initialise 
        /// the AppSettings property. If an exception is raised the method will initilaise the
        /// <see cref="AppSettings"/> property to a default InDevelopment/Test state.
        /// </summary>
        /// <param name="appSettingsFile">the name of the app config json file to read.</param>
        /// <returns>True if the app settings was sucessfully created from the file otherwise 
        /// false. </returns>
        Boolean LoadAppSettings(String appSettingsFile)
        {

            // Load app configuration settings from the app settings file and set relevant feature flags.
            try
            {
                // Create a config object, using JSON provider specifying the appSetting.json file.
                IConfiguration appConfiguration = new ConfigurationBuilder()
                    .AddJsonFile(appSettingsFile)
                    .Build();

                // Get values from the appSettings.json configuation file
                //AppConfig? settings = appConfiguration.GetRequiredSection("AppConfig").Get<AppConfig>();
                AppSettings = appConfiguration.GetRequiredSection("AppConfig").Get<AppConfig>()!;
                return true;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("The application settings file. AppSettings.json file was not " +
                    "found, deferring to in development and test settings.");
                Console.WriteLine(ex.Message);

                // We can't load app settings so create an InDevelopment / test AppConfig object
                AppSettings = new()
                {
                    InDevelopment = true,
                    InTest = true,
                    BaseURL = "Not using live service - Using mock data"
                };
                return false;
            }
        }
        public void DisplayUserInterface()
        {   
            Console.WriteLine("Welcome to the ATU Porfolio management system");
            string choice = "";
            while (choice != "11")
            {
                Console.WriteLine("\n");
                Console.WriteLine("\n----Menu----");
                Console.WriteLine("\nBalance: " + _portfolioManager.Balance);
                Console.WriteLine("\n0.Add Funds");
                Console.WriteLine("\n1.Withdraw Funds");
                Console.WriteLine("\n2.Purchase Asset");
                Console.WriteLine("\n3.Sell Asset");
                Console.WriteLine("\n4.Get quote of Asset");
                Console.WriteLine("\n5.Full Value of your portfolio");
                Console.WriteLine("\n6.View Investments");
                Console.WriteLine("\n7.Filter Assets by Type");
                Console.WriteLine("\n8.Search Assets by Name or Symbol");
                Console.WriteLine("\n9.Filter Purchases by Date");
                Console.WriteLine("\n10.Filter Sales by Date");
                /*Console.WriteLine("\n11.See Trending Stocks For Region");*/
                Console.WriteLine("\n11.Exit");
                Console.WriteLine("\n");

                choice = Console.ReadLine();
                try
                {
                    switch (Convert.ToInt32(choice))
                    {
                        case 0:
                            Console.WriteLine("\nAmount of funds to deposit:");
                            decimal amountToAdd = Convert.ToDecimal(Console.ReadLine());
                            _portfolioManager.AddFunds(amountToAdd);
                            break;
                        case 1:
                            Console.WriteLine("\nAmount of funds to withdraw:");
                            decimal amountToWithdraw = Convert.ToDecimal(Console.ReadLine());
                            _portfolioManager.WithdrawFunds(amountToWithdraw);
                            break;
                        case 2:
                            Console.WriteLine("\nAsset to purchase:");
                            string assetToPurchase = Console.ReadLine();
                            Console.WriteLine("\nAmount to purchase:");
                            decimal amountToPurchase = Convert.ToDecimal(Console.ReadLine());
                            _portfolioManager.PurchaseAsset(assetToPurchase, amountToPurchase);
                            break;
                        case 3:
                            Console.WriteLine("\nAsset to sell:");
                            string assetToSell = Console.ReadLine();
                            Console.WriteLine("\nAmount to sell:");
                            int amountToSell = Convert.ToInt32(Console.ReadLine());
                            _portfolioManager.SellAsset(assetToSell, amountToSell);
                            break;
                        case 4:
                            List<string> assetsToGet = new List<string>();
                            string assetToGet = "";
                            while (assetToGet != "n")
                            {
                                Console.WriteLine("\nAsset to get a quote of(enter n to stop): ");
                                assetToGet = Console.ReadLine();
                                if (assetToGet != "n")
                                {
                                    assetsToGet.Add(assetToGet);
                                }
                            }
                            List<AssetQuote> quotes = _portfolioManager.GetAssetInformation(assetsToGet);
                            foreach (AssetQuote quote in quotes)
                            {
                                string response = $"\n----------\nName: {quote.AssetFullName} \nAsset Symbol: {quote.AssetSymbol} \nAsset Type: {quote.AssetType} \nAsset Quote Timestamp: {DateTime.Now}" +
                                    $"\nAsset Quote Value: {quote.AssetQuoteValue} \nRegular Market Change: {quote.RegularMarketChange} \nRegular Market Change Percentage {quote.RegularMarketChangePercentage}" +
                                    $"\nRegular Market Previous Close: {quote.RegularMarketPreviousClose} \nRegular Market Open: {quote.RegularMarketOpen}\n----------";
                                Console.WriteLine(response);
                            }
                            break;
                        case 5:
                            Console.WriteLine("\nValue of portfolio: " + _portfolioManager.GetPortfolioValue());
                            break;
                        case 6:
                            Console.WriteLine("\nValue of portfolio:\n" + _portfolioManager.ListAllInvestements());
                            break;
                        case 7:
                            Console.WriteLine("\nType of investment: ");
                            string assetType = Console.ReadLine();
                            Console.WriteLine(_portfolioManager.ListPortfolioInvestementsByType(assetType));
                            break;
                        case 8:
                            string assetToFind = "";
                            List<string> assetsToFind = new List<string>();

                            while (assetToFind != "n")
                            {
                                Console.WriteLine("\nSearch(enter n to stop): ");
                                assetToFind = Console.ReadLine();
                                if (assetToFind != "n")
                                {
                                    assetsToFind.Add(assetToFind);
                                }
                            }
                            Console.WriteLine(_portfolioManager.ListPortfolioAssetsByName(assetsToFind));
                            break;
                        case 9:
                            Console.WriteLine("\nStart Date Year: ");
                            int startDateYear = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("\nStart Date Month: ");
                            int startDateMonth = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("\nStart Date Day: ");
                            int startDateDay = Convert.ToInt32(Console.ReadLine());

                            Console.WriteLine("\nEnd Date Year: ");
                            int endDateYear = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("\nEnd Date Month: ");
                            int endDateMonth = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("\nEnd Date Day: ");
                            int endDateDay = Convert.ToInt32(Console.ReadLine());

                            Console.WriteLine("\n" + _portfolioManager.ListPortfolioPurchasesInRange(new DateTime(startDateYear, startDateMonth, startDateDay), new DateTime(endDateYear, endDateMonth, endDateDay)));
                            break;
                        case 10:
                            Console.WriteLine("\nStart Date Year: ");
                            int startDateYearSale = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("\nStart Date Month: ");
                            int startDateMonthSale = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("\nStart Date Day: ");
                            int startDateDaySale = Convert.ToInt32(Console.ReadLine());

                            Console.WriteLine("\nEnd Date Year: ");
                            int endDateYearSale = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("\nEnd Date Month: ");
                            int endDateMonthSale = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("\nEnd Date Day: ");
                            int endDateDaySale = Convert.ToInt32(Console.ReadLine());

                            Console.WriteLine("\n" + _portfolioManager.ListPortfolioSalesInRange(new DateTime(startDateYearSale, startDateMonthSale, startDateDaySale), new DateTime(endDateYearSale, endDateMonthSale, endDateDaySale)));
                            break;
                       /* case 11:
                            Console.WriteLine("Desired Region: ");
                            string region = Console.ReadLine();
                            *//*for (int i = 0; i < financeClient.GetTrendingStocksForRegion(region).Count; i++)
                            {
                                Console.WriteLine(financeClient.GetTrendingStocksForRegion(region)[i]);
                            }
                            break;*/ //This worked in program.cs although the way main app works it cannot read financeClient 
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
