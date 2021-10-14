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
