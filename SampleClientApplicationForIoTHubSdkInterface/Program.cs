﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibIoTHubSdkWrapper;

namespace SampleClientApplicationForIoTHubSdkInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Endpoint endpoint = new Endpoint("HostName=afhassan-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=ZqKX5O4OxGf2RNzzZwsIqgV4hYMO9qI3nMJI8SWGcM0=", "AftDeviceId1");

            using (IoTHubSdkWrapper wrapper = new IoTHubSdkWrapper())
            {
                // Use case 1 - Add a device
                Task<DeviceInfo> deviceInfo = wrapper.AddDeviceAsync(endpoint);
                //deviceInfo.Wait();
                Console.WriteLine("printing result : " + deviceInfo.Result.PrimaryKeyConnectionString);

                // Use case 2 - Send message from device to cloud
                List<TelemetryData> data = new List<TelemetryData>();
                Dictionary<string, string> dictionary = new Dictionary<string, string>();//first message
                dictionary.Add("deviceId", "0");
                dictionary.Add("messageId", "1");
                dictionary.Add("temperature", "98.6");
                dictionary.Add("humidity", "99.9");
                Dictionary<string, string> dictionary1 = new Dictionary<string, string>();//second message
                dictionary1.Add("deviceId", "000");
                dictionary1.Add("messageId", "111");
                dictionary1.Add("temperature", "98.666");
                dictionary1.Add("humidity", "99.999");
                Dictionary<string, string> dictionary2 = new Dictionary<string, string>();//third message
                dictionary2.Add("deviceId", "___000");
                dictionary2.Add("messageId", "___111");
                dictionary2.Add("temperature", "___98.666");
                dictionary2.Add("humidity", "___99.999");
                TelemetryData telemetryData = new TelemetryData(dictionary);
                TelemetryData telemetryData1 = new TelemetryData(dictionary1);
                TelemetryData telemetryData2 = new TelemetryData(dictionary2);
                data.Add(telemetryData);
                data.Add(telemetryData1);
                data.Add(telemetryData2);
                Task<Result> sendMessageResult = wrapper.SendMessageD2CAsync(deviceInfo.Result, data, IoTHubSdkWrapper.TransportType.Amqp);
                Console.WriteLine("boolean flag : " + sendMessageResult.Result.IsSuccessful + ", reason : " + sendMessageResult.Result.Reason);

                // Use case 3 - Receive desired property change from cloud to device
                wrapper.ReceiveC2DDesiredPropertyChangeAsync(deviceInfo.Result, OnDesiredPropertyChanged, IoTHubSdkWrapper.TransportType.Mqtt).GetAwaiter().GetResult();

                Console.ReadLine();
                Console.WriteLine("done!");
            }
        }

        private static async Task OnDesiredPropertyChanged(Dictionary<string, string> desiredProperties)
        {
            Console.WriteLine("desired property change");

            foreach(KeyValuePair<string, string> pair in desiredProperties)
            {
                Console.WriteLine("key : " + pair.Key + ", " + pair.Value);
            }
        }
    }
}
