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

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)

# Prep your device
https://ubuntu.com/tutorials/how-to-install-ubuntu-on-your-raspberry-pi#2-prepare-the-sd-card

need to remember to install python3.8 not 3.9!
use the 20.04 os for the device, it comes with python3.8
https://linuxize.com/post/how-to-install-python-3-8-on-ubuntu-18-04/

sudo apt install python3-pip



for pyaudio
sudo apt-get install portaudio19-dev