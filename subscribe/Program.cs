using AzureIoTHubDevices;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Subscribing;
using System;
using System.Text;
using System.Threading.Tasks;

namespace subscribe
{
    class Program
    {
        static string cs = Environment.GetEnvironmentVariable("CS");
        static async Task Main(string[] args)
        {
            IMqttClient mqttClient = await IoTHubClient.CreateFromConnectionString(cs);
            MqttClientSubscribeResult subRes = await mqttClient.SubscribeAsync("rido/one", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            mqttClient.UseApplicationMessageReceivedHandler(MessageReceived);
            Console.WriteLine("Subscribed. " + subRes.Items.Count);
            Console.ReadLine();
        }

        private static void MessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            string msg = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            Console.WriteLine(msg);
        }
    }
}
