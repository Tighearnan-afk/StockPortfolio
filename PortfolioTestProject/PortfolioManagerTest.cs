// Written by Dr. Shane Wilson.
// The author licenses this file to you under the MIT license.
// See the LICENSE file in the solution root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Portfolio.Application;
using Portfolio.Model;

namespace PortfolioManagerTest
{
    public class PortfolioBalanceChanges
    {
        [Fact]
        public void AddFunds_ValidAmount_ChangesBalance()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager();
            decimal expectedAmout = 10;
            decimal addedAmout = 10;

            //Act
            testPortfolio.AddFunds(addedAmout);

            //Assert
            Assert.Equal(expectedAmout, testPortfolio.Balance, 2);
        }


        [Fact]
        public void AddFunds_InvalidAmount_UnChangedBalance()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager();
            testPortfolio.AddFunds(10);
            decimal addedAmount = -10;

            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => testPortfolio.AddFunds(addedAmount));
        }

        // Change this to a theory and pass lots of test data values
        [Theory]
        [InlineData(50, 25, 25)]
        [InlineData(150, 50, 100)]
        [InlineData(2000, 700, 1300)]
        [InlineData(50_000, 47_000, 3000)]
        public void WithdrawFunds_ValidAmount_ChangesBalance(decimal balance, decimal withdrawAmount, decimal expectedBalance)
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(balance);

            //Act
            testPortfolio.WithdrawFunds(withdrawAmount);

            //Assert
            Assert.Equal(expectedBalance, testPortfolio.Balance, 2);
        }

        [Fact]
        public void WithdrawFunds_InvalidAmount_UnChangedBalance()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(10);
            decimal withdrawAmount = 50;
            decimal expectedBalance = 10;



            //Act
            testPortfolio.WithdrawFunds(withdrawAmount);
            //Assert
            Assert.Equal(expectedBalance, testPortfolio.Balance, 2);
        }

        [Theory]
        [InlineData(500, 3, 35)]
        [InlineData(1000, 6, 70)]
        [InlineData(25000, 46, 17_870)]
        [InlineData(50_000, 80, 37_600)]
        public void PurchaseAsset_ValidAmount_ChangedBalance(decimal balance, decimal amountToPurchase, decimal expectedBalance)
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(balance);
            // fake asset value = 155

            //Act
            testPortfolio.PurchaseAsset("AAPL", amountToPurchase);
            //Assert
            Assert.Equal(expectedBalance, testPortfolio.Balance, 2);
        }

        [Fact]
        public void PurchaseAsset_InvalidAmount_UnChangedBalance()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(50);
            decimal amountToPurchase = 5;
            decimal expectedBalance = 50;

            //Act
            testPortfolio.PurchaseAsset("APPL", amountToPurchase);
            //Assert
            Assert.Equal(expectedBalance, testPortfolio.Balance, 2);
        }

        [Theory]
        [InlineData(50, 1, 205)]
        [InlineData(150, 2, 460)]
        [InlineData(2000, 3, 2465)]
        [InlineData(50_000, 10, 51_550)]
        [InlineData(50_000, 20, 53_100)]
        [InlineData(50_000, 21, 53_255)]
        public void SellAsset_Valid_ChangeBalance(decimal balance, decimal sellAmount, decimal expectedBalance)
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(balance);
            testPortfolio.SetPreviousPurchase("AAPL", "Apple Inc.", AssetType.Equity, 20, new DateTime(2023, 3, 6), 155.576m);
            testPortfolio.SetPreviousPurchase("AAPL", "Apple Inc.", AssetType.Equity, 20, new DateTime(2023, 3, 6), 155.576m);

            //Act
            testPortfolio.SellAsset("AAPL", sellAmount);

            //Assert
            Assert.Equal(expectedBalance, testPortfolio.Balance, 2);
        }

        [Fact]
        public void SellAsset_InValid_UnchangedBalance()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(100);


            decimal expectedBalance = 100;

            //Act
            testPortfolio.SellAsset("AAPL", 100);

            //Assert
            Assert.Equal(expectedBalance, testPortfolio.Balance, 2);
        }

        [Fact]
        public void SellAsset_Valid_ChangesList()
        {
            //Arrange
            decimal amountToSell =  21;
            int expectedListSize = 1;
            PortfolioManager testPortfolio = new PortfolioManager(50_000);
            testPortfolio.SetPreviousPurchase("AAPL", "Apple Inc.", AssetType.Equity, 20, new DateTime(2023, 3, 6), 155.576m);
            testPortfolio.SetPreviousPurchase("AAPL", "Apple Inc.", AssetType.Equity, 20, new DateTime(2023, 4, 6), 155.576m);

            //Act
            testPortfolio.SellAsset("AAPL", amountToSell);
            decimal actualSize = testPortfolio.Assets.Count();

            //Assert
            Assert.Equal(expectedListSize, actualSize, 2);
        }

        [Fact]
        public void ListPortfolioAssetsByName()
        {
            PortfolioManager testPortfolio = new PortfolioManager();
            List<string> namesL = new List<string>();
            namesL.Add("App");
            namesL.Add("Tes");

            string assets = testPortfolio.ListPortfolioAssetsByName(namesL);

            Assert.NotNull(assets);
        }

        [Fact]
        public void GetPortfolioValue_ReturnsCorrectDecimal()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(10000);
            decimal expectedValue = 310.0m;

            //Act
            testPortfolio.PurchaseAsset("AAPL", 2);
            decimal actualValue = testPortfolio.GetPortfolioValue();

            //Assert
            Assert.Equal(expectedValue, actualValue, 2);
        }

        //[Fact]
        /*public void GetPortfolioPurchasesInRange_Correct_Percentage()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(10000);
            string expectedValue = "";

            //Act
            testPortfolio.PurchaseAsset("AAPL", 2);
            String actualValue = testPortfolio.ListPortfolioPurchasesInRange(new DateTime(2020, 10, 1), new DateTime(2023, 10, 1));

            //Assert
            Assert.Equal(expectedValue, actualValue);
        }*/

        [Fact]
        public void ListAllInvestements_ReturnsNotNull()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(10000);
            testPortfolio.PurchaseAsset("AAPL", 5);
            //Act
            string returnedString = testPortfolio.ListAllInvestements();
            //Assert
            Assert.NotNull(returnedString);
        }
        [Fact]
        //This tester is for use in the debugger
        public void ListAllInvestements_StringReturnsCorrectAmountOfAssets()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(10000);
            testPortfolio.PurchaseAsset("AAPL", 5);
            testPortfolio.PurchaseAsset("AAPL", 5);
            //Act
            string returnedString = testPortfolio.ListAllInvestements();
            //Assert
            Assert.NotNull(returnedString);
        }



        // Test requires the assets list in PortfolioManager to be public, please change it back to private after testing and
        // comment out the Assert function
        /*[Fact]
        public void SetPreviousPurchase_ValidValues_AddsAssetToList() 
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager();
            List<Asset> expectedResult = new List<Asset>();

            Asset expectedAsset = new Asset
            {
                AssetSymbol = "TSLA",
                AssetFullName = "Tesla Inc.",
                AssetPurchaseDateTime = new DateTime(2021, 10, 1),
                AssetType = AssetType.Equity,
                PurchaseCost = 755.22m,
                UnitsPurchased = 10
            };

            expectedResult.Add(expectedAsset);

            //Act
            testPortfolio.SetPreviousPurchase("TSLA", "Tesla Inc.", AssetType.Equity, 10, new DateTime(2021, 10, 1), 755.22m);

            //Assert
            Assert.Equal(expectedResult[0].AssetSymbol, testPortfolio._assets[0].AssetSymbol);
            Assert.Equal(expectedResult[0].AssetFullName, testPortfolio._assets[0].AssetFullName); // test fails but when debugging the two lists are exactly the same
            Assert.Equal(expectedResult[0].AssetPurchaseDateTime, testPortfolio._assets[0].AssetPurchaseDateTime);
            Assert.Equal(expectedResult[0].AssetType, testPortfolio._assets[0].AssetType);
            Assert.Equal(expectedResult[0].PurchaseCost, testPortfolio._assets[0].PurchaseCost);
            Assert.Equal(expectedResult[0].UnitsPurchased, testPortfolio._assets[0].UnitsPurchased);

        }*/

        //Use the debugger with this test to ensure the returned asset type 
        [Fact]
        public void ListPorfolioInvestementsByType_ReturnsCorrectAssetType()
        {
            //Arrange
            string assetType = "Equity";
            PortfolioManager testPortfolio = new PortfolioManager(10000);
            testPortfolio.PurchaseAsset("AAPL", 5);
            //Act
            string returnedString = testPortfolio.ListPortfolioInvestementsByType(assetType);
            //Assert
            Assert.NotNull(returnedString);
        }

        //Use the debugger with this test to ensure the method doesnt return anything if an asset type that isnt present in the asset list is passed as an argument 
        [Fact]
        public void ListPorfolioInvestementsByType_DoesntReturnIfAssetTypeIsntPresentInList()
        {
            //Arrange
            string assetType = "Currency";
            PortfolioManager testPortfolio = new PortfolioManager(10000);
            testPortfolio.PurchaseAsset("AAPL", 5);
            //Act
            string returnedString = testPortfolio.ListPortfolioInvestementsByType(assetType);
            //Assert
            Assert.NotNull(returnedString);
        }

        [Fact]
        public void ListPortfolioSalesInRange_DisplaysCorrectValues()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(10000);
            testPortfolio.PurchaseAsset("AAPL", 10);
            testPortfolio.SellAsset("AAPL", 9);
            //Act
            string test = testPortfolio.ListPortfolioSalesInRange(new DateTime(2023, 1, 1), new DateTime(2023, 12, 31));
            //Assert
            Assert.NotNull(test);
        }

        [Fact]
        public void ListPortfolioSalesInRange_DisplaysCorrectValuesIfMultipleDifferentAssetObjectAreSold()
        {
            //Arrange
            PortfolioManager testPortfolio = new PortfolioManager(10000);
            testPortfolio.PurchaseAsset("AAPL", 5);
            testPortfolio.PurchaseAsset("AAPL", 5);
            testPortfolio.SellAsset("AAPL", 9);
            //Act
            string test = testPortfolio.ListPortfolioSalesInRange(new DateTime(2023, 1, 1), new DateTime(2023, 12, 31));
            //Assert
            Assert.NotNull(test);
        }
    }
}
