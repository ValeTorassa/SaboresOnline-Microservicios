﻿using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "Endpoint = sb://mangowebvalen.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZdIKTCsDTqPgbwDVrAGMoFg+WHiSxaj4P+ASbKdpD9s=";


        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_Name);

            var jsonMessage = JsonConvert.SerializeObject(message);

            ServiceBusMessage busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage)) 
            { 
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(busMessage);
            await client.DisposeAsync();
        }
    }
}
