using System;
using System.Collections.Generic;
using Sendwithus;

namespace LinkGreen.Email
{
    public class Mail
    {
        private const string testApiKey = "test_b003bd3edf41de5571e10a243c77375256f55b74";
        private const string liveApiKey = "live_4b220eb979cc52006c356d13df6f205738e7a0c6";

        public class EmailTemplate
        {
            public string Value { get; set; }

            private EmailTemplate(string value)
            {
                Value = value;
            }

            public static EmailTemplate ProcessCompleteTemplate => new EmailTemplate("tem_SwFt7M8ttPDvmt7tdR8kdV6c");
        }


        public static async void SendMessage(string destination, EmailTemplate template, Dictionary<string, object> mailParameters, Action<string> callback)
        {
            SendwithusClient.ApiKey = liveApiKey;

            var link = new Dictionary<string, string> { { "url", "https://www.sendiwthus.com" } };

            mailParameters.Add("link", link);

            foreach (var address in destination.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    var recipient = new EmailRecipient(address.Trim());

                    var email = new Sendwithus.Email(template.Value, mailParameters, recipient);
                    var response = await email.Send();
                    callback?.Invoke(response.status + ":" + response.receipt_id);
                }
                catch
                {
                    // ignored - no email
                }
            }
        }

        public static void SendProcessCompleteEmail(string destination, List<string> details, string processName, Action<string> callback = null)
        {
            var data = new Dictionary<string, object> {
                {"processType", processName},
                {"processEndTime", DateTime.Now},
                {"details", details.ToArray()}
            };
            SendMessage(destination, EmailTemplate.ProcessCompleteTemplate, data, callback);
        }

        public static void SendProcessCompleteEmail(string destination, string message, string processName, Action<string> callback) =>
            SendProcessCompleteEmail(destination, new List<string> { message }, processName, callback);

    }
}