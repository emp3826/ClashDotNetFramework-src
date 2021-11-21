using ClashDotNetFramework.Models.Share;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Models.Responses
{
    public class AdvertisementResponse
    {
        [JsonProperty("airports")]
        public List<AirportData> Airports { get; set; }
    }
}
