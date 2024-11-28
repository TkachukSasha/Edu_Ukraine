using System.Globalization;
using System.Text;

using Newtonsoft.Json.Linq;

namespace Web.App.Models
{
    public static class UniversityMappingExtensions
    {
        private static readonly string[] CultureFormats = {
           "yyyy-MM-dd",          // ISO format (e.g., 2024-11-28)
            "dd/MM/yyyy",          // Day/Month/Year format (e.g., 28/11/2024)
            "yyyy/MM/dd",          // Year/Month/Day format (e.g., 2024/11/28)
            "dd/MM/yyyy HH:mm:ss", // Day/Month/Year with time (e.g., 28/11/2024 15:30:00)
            "yyyy-MM-dd HH:mm:ss", // ISO format with time (e.g., 2024-11-28 15:30:00)
            "dd.MM.yyyy"           // Day.Month.Year format (e.g., 01.01.1930)
        };

        public static List<University> Map(JObject? response)
        {
            if (response is null)
            {
                return new List<University>();
            }

            var universities = new List<University>();

            var bindings = response["results"]?["bindings"];

            if (bindings is null)
            {
                return universities;
            }

            foreach (var item in bindings)
            {
                var institution = item["institutionLabel"]?["value"]?.ToString() ?? string.Empty;
                var city = item["cityLabel"]?["value"]?.ToString() ?? string.Empty;
                string? foundationDate = item["foundationDate"]?["value"]?.ToString();
                string? website = item["website"]?["value"]?.ToString();
                string? logo = item["logo"]?["value"]?.ToString();

                string parsedFoundationDate = ParseFoundationDate(foundationDate);

                string logoBase64 = ConvertToBase64(logo);

                var university = new University
                {
                    Name = institution, 
                    FoundationDate = parsedFoundationDate, 
                    City = city, 
                    CityLabel = city, 
                    Website = website ?? string.Empty, 
                    LogoUrl = logoBase64 
                };

                universities.Add(university);
            }

            return universities;
        }

        private static string ParseFoundationDate(string? foundationDate)
        {
            if (string.IsNullOrEmpty(foundationDate))
            {
                return "Unknown";
            }

            string datePart = foundationDate.Split(' ')[0]; 

            if (DateTime.TryParseExact(datePart, CultureFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.ToString("yyyy-MM-dd"); 
            }

            return "Unknown";
        }


        private static string ConvertToBase64(string? logo)
        {
            if (string.IsNullOrWhiteSpace(logo))
            {
                return string.Empty; 
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(logo)); 
        }
    }
}
