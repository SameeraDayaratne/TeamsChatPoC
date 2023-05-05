using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Web;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Data.SqlClient;
using static ChatTeams.Pages.Chat.AuthenticateModel;
using System.Text.RegularExpressions;

namespace ChatTeams.Pages.Chat
{



    public class AuthenticateModel : PageModel 
    {
        public List<ChatMessage> Messages { get; set; }
        public ChatMessage NewMessage { get; set; }

        public AuthenticateModel()
        {
            Messages = new List<ChatMessage>();
            NewMessage = new ChatMessage();
        }

        public List<MessageInfo> messageList = new List<MessageInfo>();




    public void OnGet()
        {
            var accessTokenResponse  = Authenticate();

            List<ChatMessageValue> mes = GetMessages(accessTokenResponse);
            
            

            foreach (var item in mes)
            {
               if(item.From != null)
                {
                    if (item.From.User.DisplayName == "Vithushan Sylvestor")
                    {

                        var incomingMessage = item.Body.Content;

                        if (incomingMessage.StartsWith("<p>") && incomingMessage.EndsWith("</p>"))
                        {
                            incomingMessage = incomingMessage.Substring(3, incomingMessage.Length - 7);
                        }
                        MessageInfo newMessage = new MessageInfo { Content = incomingMessage, Sender = "Incoming" };
                        messageList.Add(newMessage);

                    }
                    else
                    {
                        var outgoingMessage = item.Body.Content;

                        if (outgoingMessage.StartsWith("<p>") && outgoingMessage.EndsWith("</p>"))
                        {
                            outgoingMessage = outgoingMessage.Substring(3, outgoingMessage.Length - 7);
                        }
                        MessageInfo newMessage = new MessageInfo { Content = outgoingMessage, Sender = "Outgoing" };
                        messageList.Add(newMessage);

                    }
                }
                else
                {
                    continue;
                }
              
            }

            var s = 1;




            /*string url = "https://www.google.com/";
           



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
            }*/

        }
        public AccessTokenResponse Authenticate()
        {
            string MicrosoftGraphApiBaseUrl = "https://graph.microsoft.com";
            string MicrosoftLoginBaseUrl = "https://login.microsoftonline.com";
            string TokenEndpoint = "oauth2/v2.0/token";
            var tenantId = "1b41edc3-3eef-4a0d-b9fe-bb09d452bf85";
            var currentUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            Uri uri = new Uri(currentUrl);
            string code = HttpUtility.ParseQueryString(uri.Query).Get("code");


            try
            {
                Console.WriteLine("Entered AccessToken");
                var @params = new Dictionary<string, string>
            {
                {"client_id", "f9e599a6-6e85-49f8-9b98-561e6680f264"},
                {"client_secret", "M-M8Q~RvXA4kAriNjIcCmdmLUi13BYqI8tPKkdlt"},
                {"redirect_uri", "http://localhost:5017/Chat/Authenticate"},
                {"grant_type", "authorization_code"},
                {"scope", $"{MicrosoftGraphApiBaseUrl}/.default offline_access"},
                {"code", code}
            };

                var requestUrl = string.Format($"{MicrosoftLoginBaseUrl}/{tenantId}/{TokenEndpoint}");
                var accessTokenRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                accessTokenRequest.Method = "POST";
                accessTokenRequest.ContentType = "application/x-www-form-urlencoded;";
                accessTokenRequest.Accept = "application/json;";

                var postData = @params.Keys.Aggregate("", (current, key) => current + (HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(@params[key]) + "&"));
                var data = Encoding.ASCII.GetBytes(postData);

                var requestStream = accessTokenRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                string jsonString;
                var response = (HttpWebResponse)accessTokenRequest.GetResponse();
                using (var responseStreamReader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), true))
                {
                    jsonString = responseStreamReader.ReadToEnd();
                }
                var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(jsonString);


                string connectionString = "Data Source=SL-SDAYARATNE;Initial Catalog=mystore;Integrated Security=True";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO OauthCredentials " +
                                    "(refreshToken, accessToken) VALUES " +
                                    "(@refreshToken, @accessToken);";
                    using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@refreshToken", accessTokenResponse.RefreshToken);
                        command.Parameters.AddWithValue("@accessToken", accessTokenResponse.AccessToken);

                        command.ExecuteNonQuery();
                    }

                    return accessTokenResponse;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }

            
        }

        public List<ChatMessageValue> GetMessages(AccessTokenResponse accessTokenResponse)
        {
            Dictionary<string, string> messages = new Dictionary<string, string>();

            var accessToken = accessTokenResponse.AccessToken;

            var requestUrl = $"https://graph.microsoft.com/v1.0/me/chats/19:6e6ff219-ac42-4e80-b50f-c0d35b57da84_beb1c85e-5821-4235-b1aa-14bbcf995384@unq.gbl.spaces/messages";
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            var response = (HttpWebResponse)request.GetResponse();

            string jsonString;
            using (var responseStreamReader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), true))
            {
                jsonString = responseStreamReader.ReadToEnd();
            }

            var messageObject = JsonConvert.DeserializeObject<Message>(jsonString);

            return messageObject.Value;
        }

        public class MessageInfo
        {
            public string Content { get; set; }
            public string Sender { get; set; }
            // add other relevant properties as needed
        }
        public class ChatController : Controller
        {
            [HttpPost]
            public IActionResult SendMessage(AuthenticateModel model)
            {
                if (!string.IsNullOrWhiteSpace(model.NewMessage.Text))
                {
                    model.Messages.Add(new ChatMessage { Text = model.NewMessage.Text, IsOutgoing = true });
                }
                model.NewMessage.Text = string.Empty;

                return View(model);
            }

        }


       

        public class ChatMessage
        {
            public string Text { get; set; }
            public bool IsOutgoing { get; set; }
        }
        public class AccessTokenResponse
        {
            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresIn { get; set; }

            [JsonProperty("ext_expires_in")]
            public string ExtExpiresIn { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

        }

        public class Message
        {
            [JsonProperty("@odata.context")]
            public string ODataContext { get; set; }

            [JsonProperty("@odata.count")]
            public int ODataCount { get; set; }

            [JsonProperty("@odata.nextLink")]
            public string ODataNextLink { get; set; }

            [JsonProperty("value")]
            public List<ChatMessageValue> Value { get; set; }
        }

        public class ChatMessageValue
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("replyToId")]
            public object ReplyToId { get; set; }

            [JsonProperty("etag")]
            public string Etag { get; set; }

            [JsonProperty("messageType")]
            public string MessageType { get; set; }

            [JsonProperty("createdDateTime")]
            public string CreatedDateTime { get; set; }

            [JsonProperty("lastModifiedDateTime")]
            public string LastModifiedDateTime { get; set; }

            [JsonProperty("lastEditedDateTime")]
            public object LastEditedDateTime { get; set; }

            [JsonProperty("deletedDateTime")]
            public object DeletedDateTime { get; set; }

            [JsonProperty("subject")]
            public object Subject { get; set; }

            [JsonProperty("summary")]
            public object Summary { get; set; }

            [JsonProperty("chatId")]
            public string ChatId { get; set; }

            [JsonProperty("importance")]
            public string Importance { get; set; }

            [JsonProperty("locale")]
            public string Locale { get; set; }

            [JsonProperty("webUrl")]
            public object WebUrl { get; set; }

            [JsonProperty("channelIdentity")]
            public object ChannelIdentity { get; set; }

            [JsonProperty("policyViolation")]
            public object PolicyViolation { get; set; }

            [JsonProperty("eventDetail")]
            public object EventDetail { get; set; }

            [JsonProperty("from")]
            public ChatMessageFrom From { get; set; }

            [JsonProperty("body")]
            public ChatMessageBody Body { get; set; }

            [JsonProperty("attachments")]
            public List<object> Attachments { get; set; }

            [JsonProperty("mentions")]
            public List<object> Mentions { get; set; }

            [JsonProperty("reactions")]
            public List<object> Reactions { get; set; }
        }

        public class ChatMessageFrom
        {
            [JsonProperty("application")]
            public object Application { get; set; }

            [JsonProperty("device")]
            public object Device { get; set; }

            [JsonProperty("user")]
            public ChatMessageUser User { get; set; }
        }

        public class ChatMessageUser
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("userIdentityType")]
            public string UserIdentityType { get; set; }
        }

        public class ChatMessageBody
        {
            [JsonProperty("contentType")]
            public string ContentType { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }
        }

        public class ChatMessageAttachment
        {
            // properties of attachment object
        }

        public class ChatMessageMention
        {
            // properties of mention object
        }

        public class ChatMessageReaction
        {
            // properties of reaction object
        }
       







    }

}   

