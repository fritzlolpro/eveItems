using CsvHelper;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace eveItems
{
    class Program
    {
        private static string path = @"C:\Users\f.sorokin\Downloads\sde-20160704-TRANQUILITY\sde\fsd\typeIDs.yaml";

        private static string EveCentralURL = @"http://api.eve-central.com/api/marketstat/json?usesystem=30000142&typeid=";

        private static DateTime lastTimeFrame = DateTime.Now;
        private static int requestCount = 0;

        private const int requestsPerSecond = 10;
        public static TextWriter textWriter;

        static void Main(string[] args)
        {
            Process().Wait();
        }

        async static Task Process()
        {
            int counter = 10;

            textWriter = File.CreateText("fedor.txt");
            var csv = new CsvWriter(textWriter);
            using (var input = new StreamReader(path))
            {
                var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention(), ignoreUnmatched: true);

                Dictionary<int, InvType> types = await Task.Run(() => deserializer.Deserialize<Dictionary<int, InvType>>(input));
               
                foreach (var elem in types)
                {

                    if (counter < 0)
                    {
                        break;
                        await Task.Delay(1000);
                        counter = 10;
                    }
                    int typeId = elem.Key;
                    string name = elem.Value.name.en;

                    var data = GetMarketData(typeId).Result;
                    //Console.WriteLine("{0},{1},{2},{3},{4},{5}", typeId, name, data.sell.fivePercent, data.sell.volume, data.buy.fivePercent, data.buy.volume);

                    var stringToWrite = string.Format("{0},{1},{2},{3},{4},{5}", typeId, name, data.sell.fivePercent, data.sell.volume, data.buy.fivePercent, data.buy.volume);
                    csv.WriteRecord(stringToWrite);

                    counter--;

                }


            

        }

       
     }



    async private static Task<EveCentral.MarketType> GetMarketData(int typeId)
    {
        using (var httpClient = new HttpClient())
        {
            var stringData = await httpClient.GetStringAsync(EveCentralURL + typeId);
            return JsonConvert.DeserializeObject<List<EveCentral.MarketType>>(stringData)[0];
        }
    }


        //async private static Task<T> GetJsonFromWebLimited<T>(string url, Func<Task<T>> action)
        //{
        //    if (DateTime.Now - lastTimeFrame > TimeSpan.FromSeconds(1))
        //    {
        //        requestCount = 0;
        //        lastTimeFrame = DateTime.Now;
        //    }
        //    if (requestCount > requestsPerSecond)
        //    {
        //        await Task.Delay(1000);
        //    }
        //    return await action();
        //}
    }
}
