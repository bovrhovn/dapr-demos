using System.Text;
using ATD.SM.Models;
using ATD.SM.Web.Client.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace ATD.SM.Web.Client.Services
{
    public class StateApiClientCall(HttpClient client, IOptions<WebOptions> webSettingsValue,
        ILogger<StateApiClientCall> logger)
    {
        public async Task<Person> GetPersonAsync(string email)
        {
            logger.LogInformation("Reading person by email {Email}", email);
            try
            {
                var uri = new Uri($"{webSettingsValue.Value.WebApiLink}/byemail/{email}", UriKind.RelativeOrAbsolute);
                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogInformation("No data was found in the system");
                    return new Person();
                }
                logger.LogInformation("Http Call to create person created successfully.");
                var responseStream = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Data received: {ResponseStream}", responseStream);
                var person = JsonConvert.DeserializeObject<Person>(responseStream);
                return person ?? new Person();
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
                logger.LogInformation("Saving person {FullName}", person.FullName);
                var uri = new Uri($"{webSettingsValue.Value.WebApiLink}/add", UriKind.RelativeOrAbsolute);
                var currentPerson = JsonConvert.SerializeObject(person);
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(currentPerson, Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                    logger.LogInformation("Save of person {FullName} with {Email} executed successfully.",
                        person.FullName,
                        person.Email);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return false;
            }

            return true;
        }
    }
}