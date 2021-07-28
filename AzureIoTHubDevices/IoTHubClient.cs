using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace AzureIoTHubDevices
{
    public class IoTHubClient
    {
        public static async Task<IMqttClient> CreateFromConnectionString(string connectionString)
        {
            DateTimeOffset expiry = DateTimeOffset.UtcNow.AddMinutes(60);
            var expiryString = expiry.ToUnixTimeMilliseconds().ToString();

            DeviceConnectionString dcs = new DeviceConnectionString(connectionString);

            var mqttFactory = new MqttFactory();
            IMqttClient mqttClient = mqttFactory.CreateMqttClient();

            mqttClient.UseConnectedHandler(HandleConnected);
            mqttClient.UseDisconnectedHandler(HandleDisconnect);
            mqttClient.UseApplicationMessageReceivedHandler(HandleApplicationMessageReceivedAsync);

            var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
             .WithClientId(dcs.DeviceId)
             .WithTcpServer(dcs.HostName, 8883)
             .WithCredentials(dcs.GetUserName(expiryString), dcs.BuildSasToken(expiryString))
             .WithTls(new MqttClientOptionsBuilderTlsParameters
             {
                 UseTls = true,
                 IgnoreCertificateChainErrors = true,
                 IgnoreCertificateRevocationErrors = true,
                 AllowUntrustedCertificates = true,
                 CertificateValidationHandler = (x) => { return true; },
                 SslProtocol = SslProtocols.Tls12
             })
             .WithCleanSession(true)
             .Build();

            var connAck = await mqttClient.ConnectAsync(mqttClientOptionsBuilder, CancellationToken.None);
            Console.WriteLine(connAck.ResultCode);
            return mqttClient;
        }

        private static void HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            Console.WriteLine("HandleApplicationMessageReceivedAsync " + arg.ApplicationMessage.Payload);
        }

        private static void HandleDisconnect(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine("Disonnected: " + arg.Reason);
        }

        private static void HandleConnected(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine("Connected: " + arg.AuthenticateResult.ReasonString);

        }

    }
}
