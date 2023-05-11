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
using System.Text.RegularExpressions;

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
                var section = ExtractInfoHtml.ExtractSpecifyPartHtml(jujutsuHtml, @"//*[@id='mw-content-text']/div/div/div/div[3]");
                var filterNodes = section[0].ChildNodes.Where(x => x.Name == "table" || x.Name == "div").ToList();

                
                List<Herachy> jujutsu = new List<Herachy>();

                int numTable = 0;
                string nameMain = string.Empty;
                string secondary = string.Empty;

                for (int i = 0; i < filterNodes.Count; i++)
                {
                    if (filterNodes[i].Name == "table" && filterNodes[i+1].Name == "table")
                    {
                        string main = filterNodes[i].InnerText;
                        Console.WriteLine($"main--> {main.Trim()}");
                    }
                    if (filterNodes[i].Name == "table" && filterNodes[i + 1].Name == "div")
                    {
                        string main = filterNodes[i].InnerText;
                        Console.WriteLine($"secondary--> {main.Trim()}");
                    }
                    if (filterNodes[i].Name == "div")
                    {
                        var uno = filterNodes[i].ChildNodes[1].ChildNodes;
                        Console.WriteLine("********************************************************");
                        foreach (var item in uno)
                        {
                            Console.WriteLine($"****{item.InnerText}");
                            var  textoLimpio = item.InnerText.Replace("?", "");

                            var url = $"https://jujutsu-kaisen.fandom.com/es/wiki/{textoLimpio.Replace(" ", "_")}";
                            Console.WriteLine($"****{url}");
                        }
                        Console.WriteLine("********************************************************");
                        //Console.WriteLine($" users--> {filterNodes[i].InnerText}");
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
