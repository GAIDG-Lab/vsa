# vsa
Visual Steering Agent 


#Version and Installation

Neccesary tools to install and run mlagents package

- Unity version: 2020.3.12f1
- Python version: 3.9.9 (Python 3.9.10+ seems to be unstable with ml-agents packages)
- Upgrade pip using command `python -m pip install --upgrade pip`
- pip version: 22.0.3
- Install torch using command `pip install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html`
- numpy version: 1.22.2

To install the mlagents Python package, activate your virtual environment and run from the command line:

`python -m pip install mlagents==0.28.0`

Install Unity ML agent package in Unity:

- Open Window -> Package Manager
- Choose `Packages: Unity Registry` in the toolbar in Pakage Manager window
- Find `ML Agents` in list of packages, install preview version: 1.9.1-preview 
- To access preview packages, follow this instruction: https://medium.com/@jeffreymlynch/where-are-the-missing-preview-packages-in-unity-2020-


To run training:

- Make sure agents script is in Default Mode with no input brain.
- Create virtual enviroment inside project folder using command: `python -m venv venv`
- Activate virtual enviroment: `venv\Scripts\activate`
- Start training by running command: `mlagents-learn` when you are inside your virutal enviroment
- If you pause/stop and rerun the command again, you will run into this error: "Previous data from this run ID was found". 
You can use `mlagents-learn --resume` to resume, `mlagents-learn --force` to override previous data or `mlagents-learn --run-id={new id}` to create a new training brain.
