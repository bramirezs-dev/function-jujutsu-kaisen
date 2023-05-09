using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionJujutsu.Utils;
using System.Linq;
using System.Collections.Generic;

namespace FunctionJujutsu
{
    public static class FunctionJujutsuKaisen
    {
        [FunctionName("JujutsuKaisen")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var jujutsuHtml = await ExternalCallWeb.CallUrl("https://jujutsu-kaisen.fandom.com/es/wiki/Lista_de_Personajes#Manga");
                var section = ExtractInfoHtml.ExtractSpecifyPartHtml(jujutsuHtml, @"//div[@class='wds-tab__content wds-is-current']");
                var filterNodes = section[0].ChildNodes.Where(x => x.Name == "table" || x.Name == "div").ToList();

                filterNodes.Select( x=> x)
                List<Herachy> jujutsu = new List<Herachy>();

                int numTable = 0;
                string nameMain = string.Empty;
                string secondary = string.Empty;
                foreach (var item in filterNodes)
                {
                    if(item.Name == "table" && numTable == 0)
                    {
                        Console.WriteLine(item.InnerText);
                        numTable += 1;
                        nameMain = item.InnerText;

                        continue;
                    }
                    if (item.Name == "table" && numTable == 1)
                    {
                        Console.WriteLine(item.InnerText);
                        numTable =0;
                        secondary = item.InnerText;
                        continue;
                    }
                    if (item.Name == "div")
                    {
                        var uno = item.SelectNodes("/div[@class='wikia-gallery-item']");
                        
                            Console.WriteLine("");
                        
                    }

                }

                

                string name = req.Query["name"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                string responseMessage = string.IsNullOrEmpty(name)
                    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                    : $"Hello, {name}. This HTTP triggered function executed successfully.";

                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
           
        }
    }
}

class Herachy {
    public String MainName { get; set; }
    public List<HP> Content { get; set; }
}

class HP
{
    public String Name { get; set; }

    List<String> NameCharacters { get; set; }
}
