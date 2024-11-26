using System.Text;
using ATD.SM.Models;
using ATD.SM.Web.Client.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ATD.SM.Web.Client.Services
{
    public class StateApiClientCall
    {
        private readonly HttpClient client;
        private readonly ILogger<StateApiClientCall> logger;

        public StateApiClientCall(HttpClient client, IOptions<WebOptions> webSettingsValue,
            ILogger<StateApiClientCall> logger)
        {
            client.BaseAddress = new Uri(webSettingsValue.Value.WebApiLink, UriKind.RelativeOrAbsolute);
            this.client = client;
            this.logger = logger;
        }

        public async Task<Person> GetPersonAsync(string email)
        {
            try
            {
                logger.LogInformation("Reading person by email");
                var response = await client.GetAsync($"byemail/{email}");
                response.EnsureSuccessStatusCode();
                logger.LogInformation("Http Call created successfully.");

                var responseStream = await response.Content.ReadAsStringAsync();
                logger.LogInformation($"Data received: {responseStream}");
                var person = JsonConvert.DeserializeObject<Person>(responseStream);
                //var person = JsonSerializer.Deserialize<Person>(responseStream);
                return person;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }

            return new Person();
        }

        public async Task<bool> SavePersonAsync(Person person)
        {
            try
            {
                logger.LogInformation("Saving person");
                string currentPerson = JsonConvert.SerializeObject(person);
                //string currentPerson = JsonSerializer.Serialize(person);
                var response = await client.PostAsync($"add",
                    new StringContent(currentPerson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                logger.LogInformation("Save executed successfully.");
                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return false;
            }
        }
    }
}