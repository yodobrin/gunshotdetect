# Gunshoot Detection 
put reasoning for this project 
Address the following areas:
- community
- legal implication (e.g privacy)
- ability to report and respond
- short, mid and long term improvments 
## Abstract
address the architectue concepts and guidlines, the following subjects require attention (not in an any order of importance):
- deployment - what is the best way to deploy single and detection units (3 or more autonomic units)
- security - what is stored on the devices? how are devices identify? how do we keep the position ?
- connectivity - what is the best way for communication? (REST / AMNQ ?) do we need a heartbeat from each unit and detection unit?
- new models, improvments - how is it tricled to the deployed units?
- notification and reporting, short mid and long term - what are the alternatives? 

## Concept solution
- Each unit emmits telemetry of 3 sound models (values from 0-1) per ~30 sec
- devices are located 300 meters apart or less
- All telemtry data needs to funnel to a db

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