// See https://aka.ms/new-console-template for more information
using IMFS.Web.Models.Application;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;


class Program
{
    public static readonly string WebAPIURL = "https://test-api-au-imfs.ingrammicro.com/";
    static void Main(string[] args)
    {

        // create a logger factory
        var loggerFactory = LoggerFactory.Create(
            builder => builder
                        // add console as logging target
                        .AddConsole()
                        // add debug output as logging target
                        .AddDebug()
                        // set minimum level to log
                        .SetMinimumLevel(LogLevel.Debug)
        );
        ILogger logger = loggerFactory.CreateLogger<Program>();

        try
        {
            string url = WebAPIURL + "Application/GetDealStatus";

            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri(url);
            client.Timeout = Timeout.InfiniteTimeSpan;

            logger.LogInformation("API URL - " + WebAPIURL);

            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            logger.LogInformation(responseBody);
            //var res = JsonConvert.DeserializeObject<List<GetDealStatusResponse>>(responseBody);
            //logger.LogInformation(res?.Count + "");
        }
        catch (WebException ex)
        {
            logger.LogTrace(ex.StackTrace);
            logger.LogError(ex.Message);
            throw ex;
        }
    }
}