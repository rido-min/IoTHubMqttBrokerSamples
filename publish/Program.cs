using System;
using System.Text;
using System.Threading.Tasks;
using AzureIoTHubDevices;
using MQTTnet;
using MQTTnet.Client;

namespace publish
{
    class Program
    {
        static string cs = Environment.GetEnvironmentVariable("CS");

        static async Task Main(string[] args)
        {
            var mqttClient = await IoTHubClient.CreateFromConnectionString(cs);

            var mqttMessage = new MqttApplicationMessage()
            {
                Topic = "rido/one",
                Payload = Encoding.UTF8.GetBytes("<message-payload>"),
                QualityOfServiceLevel = 0
            };

            await mqttClient.PublishAsync(mqttMessage);

            Console.WriteLine("Message Published");
            Console.ReadLine();
        }
    }
}
