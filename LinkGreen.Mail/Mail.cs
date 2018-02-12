using System;
using System.Collections.Generic;
using Sendwithus;

namespace LinkGreen.Email
{
    public class Mail
    {
        private const string testApiKey = "test_b003bd3edf41de5571e10a243c77375256f55b74";

        public class EmailTemplate
        {
            public string Value { get; set; }

            private EmailTemplate(string value)
            {
                Value = value;
            }

            public static EmailTemplate ProcessCompleteTemplate => new EmailTemplate("tem_SwFt7M8ttPDvmt7tdR8kdV6c");
        }


        public static async void SendMessage(string destination, EmailTemplate template, Dictionary<string, object> mailParameters)
        {
            SendwithusClient.ApiKey = testApiKey;

            var link = new Dictionary<string, string> { { "url", "https://www.sendiwthus.com" } };

            mailParameters.Add("link", link);

            var recipient = new EmailRecipient(destination);

            var email = new Sendwithus.Email(template.Value, mailParameters, recipient);
            var response = await email.Send();
        }

        public static void Test()
        {
            var data = new Dictionary<string, object> {
                {"processType", "Product Import"},
                {"processEndTime", DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)},
                {
                    "details", new[] {
                        "Successfully Imported: 1567",
                        "Unable to import: Product 1 - No Price",
                        "Unable to import: Product 5 - No Price"
                    }
                }
            };

            SendMessage("trevor.watson@linkgreen.ca", EmailTemplate.ProcessCompleteTemplate, data);
        }

    }
}