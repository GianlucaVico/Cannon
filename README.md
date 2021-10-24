# Cannon Game

## Description

This is the game engine for the game of Cannon for the course Intelligent Search & Games (2122-KEN4123) Assignment.
It consist of the GUI, the game engine and an agen able to play the game against a human player.

## Features (working and not)
### ...

## Install & Run

### Docker (recommended)

Build the docker file from the root folder of the project (there is a dot at the end): `docker build -t cannon .`
Run `xhost +` to allow the docker container to use the display (`xhost -` to undo).
Then, the game can be as follow: `docker run -ti --rm -e DISPLAY=$DISPLAY --network host -v /tmp/.X11-unix:/tmp/.X11-unix cannon`

To rebuild the project, uncomment the lines in `Dockerfile` and build & run docker again.


### Linux (Ubuntu, Debian, and similar)

Install Mono: 

```bash
sudo apt install gnupg ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list 
sudo apt update

sudo apt install mono-devel
```

Gtk# should already be included. Gtk2 should be a pre-installed package.

In case they are missing, they can be installed as follow:
`sudo apt-get install libgtk2.0 gtk-sharp2`

The exact version of Mono used is `6.12.0.122 (tarball Mon Feb 22 17:29:18 UTC 2021)`
The .NET framework is .NET 4.7.2 from Mono.

To run the game from the project's root folder: `mono bin/Release/Cannon_GUI.exe`
   
To recompile the project: `msbuild Cannon_GUI.csproj -t:Rebuild -p:Configuration=Release`.


### Windows

It should be possible to run the game on Windows with Docker.

Alternatively, it is possible to find the compiled  project in the folder `bin/WindRelease`.

It needs Gtk# (it can be downloaded here: [https://www.mono-project.com/download/stable/#download-win]([https://www.mono-project.com/download/stable/#download-win])) 
and .NET for Windows ([https://dotnet.microsoft.com/download/dotnet](https://dotnet.microsoft.com/download/dotnet), it should be already included in Visual Studio).

The project can be compiled and run with Visual Studio.

## BUGS & KNOWN ISSUES

* Gtk does not like when you click too many times on the board while the agent is thinking.
* Possible bugs with the UNDO button, use it carefully.
* Click UNDO twice to undo the human player's move.
* When switching players, the agent timer is running for the first move of the human player.
* Some combination of switching players and clicking RESTART break the game and the AI agent is playing the wrong color.
* The agent behaves differently on Linux and on Windows.
* `(Cannon_GUI:1): Gtk-WARNING **: 13:48:21.661: cannot open display: :0` : run `xhost +` before `docker run ...`
