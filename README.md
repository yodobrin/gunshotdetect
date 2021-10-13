# Gunshoot Detection 
There are multiple areas that the civilian majority is suffering from a violent groups, which use guns to intimidate civilians. 
Gun fires are not logged, reported (due to many reasons) or handled by the police.
We came to realize the most fit solution for the hack is using IoT Central and building our own simulated devices. As we wish to address the lack of gathered data from the field, and act upon data, the actual devices play a role, but not a critical one for the hack.

## The Israeli View
While we live in Israel and the current personal safety of Arab civilians is in decline, so far over one hundred people murdered and the Police is clueless regarding finding the killers. This is a situation that might be in other areas. While we are not aiming to solve the underline reasons for this situation, we do hope to address and assist the Police or other authorities in finding people who uses guns in an urban and other areas. One of the areas that has limited to no visability is the actual data. How many shots are fired? Where? Are there any patterns that we can seek.
Data would start the conversation, it will allow authorities to develop plans and measure their plans progression.

## Future Looking 
Visual elements with dispatched drones and gathering surveillance camera feeds.
Enhance Sound Model to detect type of gus (9mm, 5.56mm etc.)

 
## Abstract

## Concept solution
- Each unit emmits telemetry of 3 sound models (values from 0-1) per ~30 sec
- devices are located 300 meters apart or less
- All telemtry data funnel to Cosmos via data export utility of IoT Central. Data is enriched with device data.

On the db side:
- if 3 models avg is over a predefined threshold - a gunshot is suspected on a specific unit
- every 30 seconds get all suspected gunshot locations from the last minute, order by timestamp asc, 

** what about multiple gunshots?
# Build and Test
Using vs code to develop iot modules - see [documentation](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-vs-code-develop-module?view=iotedge-2020-11)

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 


# Prep your device
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

docker login -u gunshotregistry -p P=4JbSdJ0IOowcygYeCwlPJwu37Sguac gunshotregistry.azurecr.io

docker tag heartbeat gunshotregistry.azurecr.io/heartbeat
docker push gunshotregistry.azurecr.io/heartbeat

from device
docker pull gunshotregistry.azurecr.io/heartbeat