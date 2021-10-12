/*
Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.
THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code, provided that. 
You agree: 
	(i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded;
    (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and
	(iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code
**/

// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Cosmos;


namespace server
{
    public static class IngestStream
    {
        public static bool IsPropertyExist(dynamic settings, string name, ILogger log)
        {
            bool res = new Newtonsoft.Json.Linq.JObject(settings).ContainsKey(name);
            return res;
        }

        [FunctionName("IngestStream")]
        public static async Task Run([EventHubTrigger("workitems", Connection = "EH_STREAM")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();
            string smodelThreshold = Environment.GetEnvironmentVariable("MODEL_THRESHOLD");
            double modelThreshold = double.Parse(smodelThreshold,System.Globalization.CultureInfo.InvariantCulture);
            // obtain handles to cosmos

            string DatabaseName = Environment.GetEnvironmentVariable("COSMOS_DB_NAME");
            string inventoryCollection = Environment.GetEnvironmentVariable("INVENTORY_COLL");
            string telemetryCollection = Environment.GetEnvironmentVariable("TELEMETRY_COLL");
            string ConnectionStringSetting = Environment.GetEnvironmentVariable("COSMOS_CS");

            CosmosClient cosmosClient = new CosmosClient(ConnectionStringSetting);
            Container inventoryContainer = cosmosClient.GetContainer(DatabaseName,inventoryCollection);
            Container telemetryContainer = cosmosClient.GetContainer(DatabaseName,telemetryCollection);

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    log.LogInformation($"Got event: {messageBody}");                    
                    dynamic message = JsonConvert.DeserializeObject(messageBody);
                    // take the telemetry from the message
                    dynamic telemetry = message.telemetry;
                    dynamic enrichments = message.enrichments;
                    // in case there are no enrichments, skip the message - there is nothing that can be done with it
                    if(message == null 
                        || !IsPropertyExist(enrichments,"deviceId",log)
                        || enrichments==null
                        || !IsPropertyExist(enrichments,"dev_lat",log) 
                        || !IsPropertyExist(enrichments,"dev_long",log) 
                        || !IsPropertyExist(enrichments, "devicename",log)) throw new Exception("where de fuck r the enrichments");
                    
                    string deviceId = (IsPropertyExist(enrichments,"deviceId",log))?enrichments.deviceId:"motherfucker";
                    log.LogInformation($"device id: {deviceId}");

                    dynamic device = new System.Dynamic.ExpandoObject();
                    device.muni = enrichments.muni;
                    string pkey = device.muni;
                    log.LogInformation($"partition key: {pkey}");
                    device.cluster = enrichments.cluster;
                    device.alt = enrichments.alt;                  
                    device.lat = enrichments.dev_lat;
                    device.lon = enrichments.dev_long;
                    device.location = $"{device.lat},{device.lon}";
                    device.heartbeat = telemetry.deviceheartbeat;
                    device.ingesttimestamp = message.enqueuedTime;
                                     
                    device.id = deviceId;

                    double model1,model2,model3;
                    model1 = telemetry.model1;
                    model2 = telemetry.model2;
                    model3 = telemetry.model3;
                    // verify a detection has indeed occured
                    double detectionValue = (model1 + model2 + model3)/3;
                    Boolean detected = detectionValue >= modelThreshold ;
                    // update the telemetry with the calculation and the flag
                    telemetry.detected = detected;
                    telemetry.detectionValue = detectionValue;                    
                    // update telemetry with device info
                    telemetry.muni = device.muni;                     
                    telemetry.cluster = device.cluster;
                    telemetry.alt = device.alt;
                    telemetry.lat = device.lat;
                    telemetry.lon = device.lon;
                    telemetry.location = device.location;
                    telemetry.heartbeat = device.heartbeat;
                    telemetry.ingesttimestamp = device.ingesttimestamp;
                    telemetry.deviceId = deviceId;
                    string id = Guid.NewGuid().ToString();
                    telemetry.id = id;
                    // // save telemetry to the telemetry and inventory to the appropriate collections
                    ItemResponse<object> inventoryResponse = await inventoryContainer.UpsertItemAsync<object>(device, new PartitionKey(pkey));  
                    ItemResponse<object> telemetryResponse = await telemetryContainer.CreateItemAsync<object>(telemetry, new PartitionKey(pkey));  
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
                
            }
            cosmosClient.Dispose();

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
