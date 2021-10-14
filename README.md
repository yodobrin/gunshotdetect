# Gunshoot Detection 
There are multiple areas that the civilian majority is suffering from a violent groups, which use guns to intimidate civilians. 
Gun fires are not logged, reported (due to many reasons) or handled by the police.
We came to realize the most fit solution for the hack is using IoT Central and building our own simulated devices. As we wish to address the lack of gathered data from the field, and act upon data, the actual devices play a role, but not a critical one for the hack.

## The Israeli View
While we live in Israel and the current personal safety of Arab civilians is in decline, so far over one hundred people murdered and the Police is clueless regarding finding the killers. This is a situation that might be in other areas. While we are not aiming to solve the underline reasons for this situation, we do hope to address and assist the Police or other authorities in finding people who uses guns in an urban and other areas. One of the areas that has limited to no visability is the actual data. How many shots are fired? Where? Are there any patterns that we can seek.
Data would start the conversation, it will allow authorities to develop plans and measure their plans progression.

## Future Looking 
- Visual elements with dispatched drones and gathering surveillance camera feeds.
- Enhance Sound Model to detect type of gus (9mm, 5.56mm etc.)
- Upload to blob using the provided function

 
## Abstract
![image](https://user-images.githubusercontent.com/37622785/137309050-5118ad48-e75a-4ff8-a683-7607c6c0bc94.png)



## Concept solution
- Each unit emmits telemetry of 3 sound models (values from 0-1) per ~30 sec
- devices are located 300 meters apart or less
- All telemtry data funnel to Cosmos via data export utility of IoT Central. Data is enriched with device data.
- Data flows through eventhub and ingested by a function
- The function validate required data elements are on the message, perform validation of the model scores and decide if it is to be marked as a detection.
- Another functions is triggered once a minute on the 15th second, and performs:
    - Query the inventory collection, finding out what devices are avilable
    - Query the telemetry collection based on the municipality (which acts as the partition key)
- The DB is marked to have the synapse analytics enabled
- Each collection has synapse analytics
- We use the serverless option in Synapse to reduce cost
- Few viewes were created to enable PowerBI query the refreshed data comming in
- PowerBI model was created to support few simple reports

# Build and Test
As part of the hack, and attempt was made to leverage Rasbary Pie 3 b+. However due to multiple HW issues, and the limited time given, we made a calculated decsion to leverage device simulation provided by IoT Central.
When building a device model, it is advised to examine our official  [documentation](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-vs-code-develop-module?view=iotedge-2020-11).
There is also the option to trial and error with the portal (which to be honest is what we did) you can examine the device model in this repo.

## .NET code
2 main functions, one which perform data ingestion from eventhub to cosmos, the second is time triggered and checks the number of events per area, determines the estimated location and saves to addtional collection.
EventHub is used as the sink for data export from the IoT Central
Cosmos with synapse link enabled, having 3 collections, device inventory, raw telemetry, calculated alerts
Synapse serverless, we created few viewed which Power BI, can use as the underline data source.

## IoT Central
### Device Template
This repo contains a reference device model, which we used.
![image](https://user-images.githubusercontent.com/37622785/137309766-51382435-d6f7-46d9-9531-51e92b40b8f5.png)

You can customize the behavior of telemetry values, and also address the initialzation of property values:
![image](https://user-images.githubusercontent.com/37622785/137309911-446df01c-6e22-45df-bf9e-f23937cc4bcf.png)

![image](https://user-images.githubusercontent.com/37622785/137310122-1881398b-78cb-4b6a-9693-d4f73e9a9023.png)

### Data Export
The export tool, allows adding enrichment to the telemetry data, we used it to enrich with the properties of the devices, such as location, name etc.
![image](https://user-images.githubusercontent.com/37622785/137310358-edc567e7-c4dd-49b9-aa5c-db3eb853cb3d.png)

# Contribute
Contact one of the repo collaborators to gain access. 

