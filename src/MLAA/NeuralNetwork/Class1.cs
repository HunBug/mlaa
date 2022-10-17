using System.Collections.Generic;
using System.Diagnostics;
using Server.Model.Api;
using Server.Model.Client;
using Server.Model.Model;

namespace NeuralNetwork
{
    public class Class1
    {
        public static void Test()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost:8000";
            var apiInstance = new DefaultApi(config);
            try
            {
                // Home
                object result = apiInstance.HomeGet();
                Console.WriteLine(result);
                FileStream fileStream = new FileStream(@"C:\Users\akoss\work\hunbug\repos\mlaa\src\MLAA\Python\chunks_0\killfeed_000000.jpg", FileMode.Open);
                result = apiInstance.PredictPredictPost(fileStream);
                Console.WriteLine(result);
            }
            catch (ApiException e)
            {
                Console.WriteLine("Exception when calling DefaultApi.HomeGet: " + e.Message);
                Console.WriteLine("Status Code: " + e.ErrorCode);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}