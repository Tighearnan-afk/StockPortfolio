// Written by Dr. Shane Wilson.
// The author licenses this file to you under the MIT license.
// See the LICENSE file in the solution root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portfolio.Application;
using Portfolio.Model;
using Portfolio.Service.TestDouble;

namespace MockClientTest
{
    public class MockClientReturns
    {
        [Fact]
        public void GetTrendingStocksForRegion_ValidRegion_ReturnsListOfAssetSymbols()
        {
            //Arrange
            MockClient testClient = new MockClient();
            List<String> expectedOutput = new List<String>();
            expectedOutput.Add("AAPL");
            expectedOutput.Add("MSFT");

            //Act
            //testClient.GetTrendingStocksForRegion("US");

            //Assert
            Assert.Equal(expectedOutput, testClient.GetTrendingStocksForRegion("US"));
        }
    }
}
