# Setting up the Project
## Download Unity
### Download/Install Unity Hub
https://unity3d.com/get-unity/download
### Install Unity
* Open Unity Hub
* Go the [Unity Download Archive](https://unity3d.com/get-unity/download/archive) and install version *2021.3.17f1* via the Unity Hub link
## Download this Project
You can download and install the project either via Github Desktop or by using the git CLI (command-line interface) -- most folks should opt to use Github Desktop unless you're already comfortable with - or want to learn how to use - the CLI
### Using Github Desktop
* If you don't know git very well, the best way to clone this project is to use https://desktop.github.com/ -- the website will have instructions for setting up github and cloning this repository.
* To open the project, you will need to have the [git CLI installed](https://git-scm.com/downloads)
### Using git CLI
* This project uses submodules, so you'll need to clone using `git clone --recurse-submodules https://github.com/mrpropellers/ggj23.git`
* If you already cloned without the above flag, you can do `git submodule update --init --recursive` inside the repo to pull the submodules
* You'll need to manually initialize git LFS as well, once cloned use `git lfs install` and `git lfs pull` to pull any binary files
## Open the Project in Unity
From the Unity Hub | Projects tab, click "Open," navigate to where Git checked out the project, and select the *<PROJECT NAME>* directory

# Ask for help if you need it!
There are several people in the Discord that can help you if you get stuck!

# Acknowledgements
