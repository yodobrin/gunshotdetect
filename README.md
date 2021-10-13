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

 
## Abstract
![image](https://user-images.githubusercontent.com/37622785/137107471-072e14ec-fbb6-4dda-808a-c03f3403f7c0.png)


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

# Contribute
Contact one of the repo collaborators to gain access. 

# This is not confirmed content 
## Prep your device
https://ubuntu.com/tutorials/how-to-install-ubuntu-on-your-raspberry-pi#2-prepare-the-sd-card

```sudo apt  install network-manager```
turn wifi on
```nmcli r wifi on```
verify if there is a connection
```nmcli d wifi list```
```nmcli d wifi connect tofu password 22102003```

iot edge
https://docs.microsoft.com/en-us/azure/iot-edge/how-to-install-iot-edge?view=iotedge-2020-11

need to remember to install python3.8 not 3.9!
use the 20.04 os for the device, it comes with python3.8
https://linuxize.com/post/how-to-install-python-3-8-on-ubuntu-18-04/

sudo apt install python3-pip
sudo apt-get install libsndfile1
docker:
curl -sSL https://get.docker.com/ | sh
az cli
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash


for pyaudio
sudo apt-get install portaudio19-dev

dockers

docker tag heartbeat gunshotregistry.azurecr.io/heartbeat
docker push gunshotregistry.azurecr.io/heartbeat

from device
docker pull gunshotregistry.azurecr.io/heartbeat
