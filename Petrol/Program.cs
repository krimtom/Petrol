using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Petrol
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RunAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0} ", e.Message);
            }
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://apis.is");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responsePetrol = await client.GetAsync("/petrol");
                HttpResponseMessage responseCurrency = await client.GetAsync("/currency/arion");

                // If response from both api's are a success
                if (responsePetrol.IsSuccessStatusCode && responseCurrency.IsSuccessStatusCode)
                {
                    // Get content
                    Model.PetrolApis petrolApisResults = await responsePetrol.Content.ReadAsAsync<Model.PetrolApis>();
                    Model.CurrencyApis currencyApisResults = await responseCurrency.Content.ReadAsAsync<Model.CurrencyApis>();

                    Model.CurrencyObject currObj = new Model.CurrencyObject();
                    Model.PetrolObject petrObj = new Model.PetrolObject();

                    // Finds value for USD
                    double usdValue = currObj.FindValue(currencyApisResults.results, "USD");
                    // Finds value for EUR
                    double eurValue = currObj.FindValue(currencyApisResults.results, "EUR");

                    // Groups all stations with same company name
                    var companyGroup = petrolApisResults.results.GroupBy(x => x.company);

                    // Finds the lowest price for each company in a companyGroup
                    double lowestPrice;
                    foreach (var company in companyGroup)
                    {
                        // Finds lowest price
                        lowestPrice = company.Min(y => y.diesel);
                        // Convert into USD/EUR
                        double priceInUSD = lowestPrice / usdValue;
                        double priceInEUR = lowestPrice / eurValue;

                        // Find all stations where price equals lowestPrice
                        var lowestStation = company.Where(p => p.diesel == lowestPrice).Select(p => new { p.company, p.name, p.diesel });

                        // Write those stations out by keeping track of if the company name comes up twice or more times
                        bool first = true;
                        foreach (var station in lowestStation)
                        {
                            var prev = station.company;
                            if (first)
                            {
                                first = false;
                                Console.WriteLine('\n');
                                Console.WriteLine("Hjá fyrirtækinu: " + station.company);
                                Console.WriteLine("er diesel ódýrast við: " + station.name);
                                Console.WriteLine("Þar eru verðin:");
                                Console.WriteLine("ISK: {0:0.00}", station.diesel);
                                Console.WriteLine("USD: {0:0.00}", priceInUSD);
                                Console.WriteLine("EUR: {0:0.00}", priceInEUR);
                            }
                            // If more than one station with same company name then write this
                            else if (String.Compare(prev, station.company) == 0)
                            {
                                Console.WriteLine("Sömu verð er að finna við: " + station.name);
                            }
                        }
                    }
                }
                // If responsePetrol status code is not OK
                else if (!responsePetrol.IsSuccessStatusCode)
                {
                    throw new ApplicationException("error code: " + responsePetrol.StatusCode.ToString());
                }
                // If resposeCurrency status code is not OK
                else if (!responseCurrency.IsSuccessStatusCode)
                {
                    throw new ApplicationException("error code: " + responseCurrency.StatusCode.ToString());
                }

            }
        }
    }
}
