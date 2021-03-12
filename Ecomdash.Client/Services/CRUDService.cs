 
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using EcomdashDTO = Ecomdash.Client.Models;

namespace Ecomdash.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();

        private readonly Logger _logger;

        public CRUDService()
        {
            _logger = LogManager.GetCurrentClassLogger();

            // set up HttpClient instance
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();    // Azzero precedenti configurazione (precauzione) ù
             
            // Request headers
            _httpClient.DefaultRequestHeaders.Add("ecd-subscription-key", "");
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");
                      

            // Indicare il formato Accettato per risposte aumenta Affidabilità e stabilità del client ( in ogni caso --> impostare un default) 
            _httpClient.DefaultRequestHeaders.Accept.Add(
                     new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task Run()
        {
            _logger.Info("CRUDService Running...");  
        }

        public async Task GetOrders()
        {

            // Gestione di eventuali parametri ( paginazione, ordinamento, ecc... )
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            var uri = "https://ecomdash.azure-api.net/api/orders/getOrders?" + queryString;


            // Il risultato è un TASK tipizzato --> quindi contiene l'esito dell'operazione + l'eventuale risultato della risposta
            var response = await _httpClient.GetAsync("api/movies");

            // Se la classe di appartenza dello Status Code non è Successful Operation --> viene generata un'accezione 
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                var orders = JsonConvert.DeserializeObject<List<EcomdashDTO.Order>>(content);
            }
            else
            {
                _logger.Error($"Media Type not Supported: {response.Content.Headers.ContentType.MediaType}");
            }
        }
         
    }
}
