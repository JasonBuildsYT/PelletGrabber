# PelletGrabber
My first MLAgents project. There are two agents, one tries to "grab" pellets that spawn around the map and the other hunts the first agent.


##############################################################################################
#################################################
If you're looking to setup this project on your own computer, I recommend you follow the steps 
below to setup a virtual environment or you follow the video I have linked as it shows
the steps I have listed. If you watch the video, only the beginning part covers
the steps listed, no need to watch the entire video (Unless you want to).

How To Use MACHINE LEARNING In Unity MLAgents Setup & Basic Environment - Pellet Grabber Tutorial #1:
    https://youtu.be/D0jTowlMROc

1. Install python 3.9.13 if still available. If not available, I believe 3.10.12 works
2. Open CMD prompt in Unity project folder
3. Create virtual environment in Unity project folder
     python -m venv <venvName>
     you can swap '<venvName>' with whatever you would like
4. Activate virtual environment
     <venvName>\scripts\activate
5. Packages:
     python -m pip install --upgrade pip
     pip install mlagents
     pip install torch torchvision torchaudio
	   pip install protobuf==3.20.3
   Make sure MLagents is properly installed
     mlagents-learn -h
     or
     mlagents-learn --help
       A list of commands should appear in the CMD Prompt window

#################################################
##############################################################################################
