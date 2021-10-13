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
using Microsoft.Azure.WebJobs;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;



namespace server
{
    public static class TriangulateAlert
    {
        [FunctionName("TriangulateAlert")]
        public static async Task Run([TimerTrigger("15 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string DatabaseName = Environment.GetEnvironmentVariable("COSMOS_DB_NAME");
            string alertsCollection = Environment.GetEnvironmentVariable("ALERTS_COLL");            
            string ConnectionStringSetting = Environment.GetEnvironmentVariable("COSMOS_CS");   
            string telemetryCollection = Environment.GetEnvironmentVariable("TELEMETRY_COLL");
            string inventoryCollection = Environment.GetEnvironmentVariable("INVENTORY_COLL");
                

            CosmosClient cosmosClient = new CosmosClient(ConnectionStringSetting);
            Container alretsContainer = cosmosClient.GetContainer(DatabaseName,alertsCollection);
            Container telemetryContainer = cosmosClient.GetContainer(DatabaseName,telemetryCollection);
            // to be used to query what are the diffrent municipalities
            Container inventoryContainer = cosmosClient.GetContainer(DatabaseName,inventoryCollection);

            QueryDefinition InventoryQuery = new QueryDefinition("SELECT distinct c.muni, c.cluster FROM inventory c");
            List<Object> inventory = await QueryCosmos(telemetryContainer,InventoryQuery,log);
            string muniName,clusterName,detectedLoc;
            
            foreach(dynamic muni in inventory)
            {
                muniName = muni.muni;
                clusterName = muni.cluster;
                PartitionKey pkey = new PartitionKey(muniName);
                // get all detected events from the past 60 sec (or more)
                QueryDefinition TelQuery = new QueryDefinition("SELECT * FROM telemetry t " + 
                                " where t.ingesttimestamp > DateTimeAdd ('minute',-1,GetCurrentDateTime())" +
                                " and t.detected=true" +
                                " and t.cluster=@cluster").WithParameter("@cluster",clusterName);
                List<Object> res = await QueryCosmosWithPartitionKey(telemetryContainer,TelQuery,pkey,log);
                int size = res.Count;
                // log.LogInformation($"result is {size} items");
                double [] lats = new double[size];
                double [] lons = new double[size];
                int pos = 0;
                // we will take timestamp from the items (might need to revisit)
                string ts = "when?";
                foreach (dynamic item in res)
                {
                    lats[pos] = (double)item.lat;
                    lons[pos] = (double)item.lon;                  
                    ts = item.ingesttimestamp;
                    pos++;                    
                }
                // triangulate and save alert to cosmos
                detectedLoc = Triangulate(lats,lons);
                dynamic alert = new System.Dynamic.ExpandoObject();
                alert.location = detectedLoc;
                alert.muni = muniName;
                alert.cluster = clusterName;
                // put in the wieght - how many detections
                alert.weight = pos+1;
                alert.reportedTime = ts; 
                // insert to alert table
                string id = Guid.NewGuid().ToString();
                alert.id = id;
                // if the ts is "when?" it means there is an issue with data
                if( ! ts.Equals("when?"))
                {
                    ItemResponse<object> telemetryResponse = await alretsContainer.CreateItemAsync<object>(alert, pkey);  
                    log.LogInformation($"Saved an alert with location:{detectedLoc}");
                }
                

            }
 
            cosmosClient.Dispose();
        }
        private static async Task<List<Object>> QueryCosmos(Container container,QueryDefinition query, ILogger log)
        {
            List<Object> results = new List<Object>();
            using (FeedIterator<Object> resultSetIterator = container.GetItemQueryIterator<Object>(
                query
                ))
                {
                    while (resultSetIterator.HasMoreResults)
                    {
                        FeedResponse<Object> response = await resultSetIterator.ReadNextAsync();
                        results.AddRange(response);
                    }
                }
            log.LogInformation($"Total number of results from query: {results.Count}");
            return results;    

        }

        private static async Task<List<Object>> QueryCosmosWithPartitionKey(Container container,QueryDefinition query,PartitionKey partitionKey, ILogger log)
        {
            List<Object> results = new List<Object>();
            using (FeedIterator<Object> resultSetIterator = container.GetItemQueryIterator<Object>(
                query,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = partitionKey
                }))
                {
                    while (resultSetIterator.HasMoreResults)
                    {
                        FeedResponse<Object> response = await resultSetIterator.ReadNextAsync();
                        results.AddRange(response);
                    }
                }
            log.LogInformation($"Total number of results from (pk) query: {results.Count}");
            return results;    

        }
        
        /*
            As of initial drop the triangulate does not take into account the ts diffrences, rather 
            performs an average of the reported devices location.
            This will need to be finetuned for a live system.
        
        */
        public static string Triangulate(double [] lat, double[] lon)
        {
            double avg_lat = getAvg(lat);
            double avg_lon = getAvg(lon);
            return $"{avg_lat},{avg_lon}";
        }

        public static double getAvg(double [] vals){
            int size = vals.Length;
            double sum = 0.00;
            foreach(double val in vals)
            {
                sum+=val;
            }
            return sum/(size);

        }
    }
}
