// Written by Dr. Shane Wilson.
// The author licenses this file to you under the MIT license.
// See the LICENSE file in the solution root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Service.Live
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Finance
    {
        public object error { get; set; }
        public List<ResultRegion> result { get; set; }
    }

    public class Quote
    {
        public string symbol { get; set; }
    }

    public class ResultRegion
    {
        public int count { get; set; }
        public long jobTimestamp { get; set; }
        public List<Quote> quotes { get; set; }
        public long startInterval { get; set; }
    }

    public class RootRegion
    {
        public Finance finance { get; set; }
    }


}
