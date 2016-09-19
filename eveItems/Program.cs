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

        static void Main(string[] args)
        {
            Process().Wait();
        }

        async static Task Process()
        {
            using (var input = new StreamReader(path))
            {
                var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention(), ignoreUnmatched: true);

                Dictionary<int, InvType> types = await Task.Run( () => deserializer.Deserialize<Dictionary<int, InvType>>(input) );

                foreach (var elem in types)
                {
                    int typeId = elem.Key;
                    string name = elem.Value.name.en;
                    Console.WriteLine("{0} - {1}", typeId, name);
                }
            }

            var data = GetMarketData(34).Result;

        }

        async private static Task<EveCentral.MarketType> GetMarketData(int typeId)
        {
            using (var httpClient = new HttpClient())
            {
                var stringData = await httpClient.GetStringAsync(EveCentralURL + typeId);
                return JsonConvert.DeserializeObject<List<EveCentral.MarketType> >(stringData)[0];
            }
        }

        
        async private static Task<T> GetJsonFromWebLimited<T>( string url, Func<Task<T>> action )
        {
            if(DateTime.Now - lastTimeFrame > TimeSpan.FromSeconds(1))
            {
                requestCount = 0;
                lastTimeFrame = DateTime.Now;
            }
            if(requestCount > requestsPerSecond)
            {
                await Task.Delay(1000);
            }
            return await action();
        }
    }
}
