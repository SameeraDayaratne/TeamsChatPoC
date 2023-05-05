using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ChatTeams.Pages.Chat
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            /*   var accessToken = "aaasd";
               var requestUrl = $"https://graph.microsoft.com/v1.0/me";
               var onlineMeetingRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
               onlineMeetingRequest.Method = "POST";
               onlineMeetingRequest.ContentType = "application/json";
               onlineMeetingRequest.Accept = "application/json";
               onlineMeetingRequest.Headers.Add("Authorization", "Bearer " + accessToken);

               var response = (HttpWebResponse)onlineMeetingRequest.GetResponse();
               string jsonString;
               using (var responseStreamReader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), true))
               {
                   jsonString = responseStreamReader.ReadToEnd();
               }

               var onlineMeetingResponse = JsonConvert.DeserializeObject<Users>(jsonString);


   */
            
        }

        
        public void OnPostHandleButtonClick()
        {
            var tenant = "1b41edc3-3eef-4a0d-b9fe-bb09d452bf85";

            string url = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?" +
            "client_id=f9e599a6-6e85-49f8-9b98-561e6680f264" +
            "&response_type=code" +
            "&redirect_uri=http://localhost:5017/Chat/Authenticate" +
            "&response_mode=query" +
            "&scope=offline_access%20user.read%20mail.read" +
            "&state=12345";



            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open URL: {ex.Message}");
            }


        }
        public class Users
        {
            [JsonProperty("businessPhones")]
            public List<string> BusinessPhones { get; set; }

            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("givenName")]
            public string GivenName { get; set; }

            [JsonProperty("jobTitle")]
            public string JobTitle { get; set; }

            [JsonProperty("mail")]
            public string Mail { get; set; }

            [JsonProperty("mobilePhone")]
            public string MobilePhone { get; set; }

            [JsonProperty("officeLocation")]
            public string OfficeLocation { get; set; }

            [JsonProperty("preferredLanguage")]
            public string PreferredLanguage { get; set; }

            [JsonProperty("surname")]
            public string Surname { get; set; }

            [JsonProperty("userPrincipalName")]
            public string UserPrincipalName { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

    }
}
