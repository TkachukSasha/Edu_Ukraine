using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Web.App.Models;

namespace Web.App.Services
{
    public class SPARQLQueryService(HttpClient sparqlClient)
    {
        private readonly HttpClient _sparqlClient = sparqlClient;

        public async Task<List<University>> GetUniversitiesListAsync(string filter = "")
        {
            string sparqlQuery = @"
                   SELECT ?institutionLabel
                          ?foundationDate
                    WHERE {
                      ?institution wdt:P31 wd:Q3918;            # Об'єкти, які є навчальними закладами
                                  wdt:P17 wd:Q212;            # Знаходяться в Україні
                                  wdt:P131 ?city.            # Місто, в якому знаходиться навчальний заклад
                      OPTIONAL { ?institution wdt:P571 ?foundationDate. } # Дата заснування

                      SERVICE wikibase:label { bd:serviceParam wikibase:language ""[AUTO_LANGUAGE],en"". }
                    }";

            string requestUri = $"?query={Uri.EscapeDataString(sparqlQuery)}&format=json";
            string response = await _sparqlClient.GetStringAsync(requestUri);

            JObject? jsonResponse = JsonConvert.DeserializeObject<JObject>(response);

            var universities = UniversityMappingExtensions.Map(jsonResponse);

            return string.IsNullOrWhiteSpace(filter)
                ? universities
                : universities.Where(x => x.Name == filter).ToList();
        }

        public async Task<University> GetUniversityDetailsAsync(string universityName)
        {
            string sparqlQuery = """
                SELECT ?institution ?institutionLabel ?city ?cityLabel
                       ?foundationDate ?website ?logo
                WHERE {
                  ?institution wdt:P31 wd:Q3918;            # Об'єкти, які є навчальними закладами
                              wdt:P17 wd:Q212;            # Знаходяться в Україні
                              wdt:P131 ?city.            # Місто, в якому знаходиться навчальний заклад
                  OPTIONAL { ?institution wdt:P279 ?type. } # Тип навчального закладу (наприклад, університет, технікум)
                  OPTIONAL { ?institution wdt:P571 ?foundationDate. } # Дата заснування
                  OPTIONAL { ?institution wdt:P856 ?website. } # Вебсайт
                  OPTIONAL { ?institution wdt:P154 ?logo. } # Логотип

                  SERVICE wikibase:label { bd:serviceParam wikibase:language "[AUTO_LANGUAGE],en". }
                }
                """;

            string requestUri = $"?query={Uri.EscapeDataString(sparqlQuery)}&format=json";
            string response = await _sparqlClient.GetStringAsync(requestUri);

            JObject? jsonResponse = JsonConvert.DeserializeObject<JObject>(response);

            var universities = UniversityMappingExtensions.Map(jsonResponse);

            return universities.FirstOrDefault(x => x.Name == universityName);
        }
    }
}
