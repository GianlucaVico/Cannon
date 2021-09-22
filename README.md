# Cannon Game


xbuild Cannon_GUI.csproj
mkbundle --simple --deps Cannon_GUI.exe -o cannon -L /usr/lib/mono/4.5 --i18n none --no-machine-config --no-config


FROM mono:latest
EXPOSE 4321
ADD . /src
WORKDIR /src
RUN sudo apt-get update && sudo apt-get install libgtk2.0 gtk-sharp2
CMD [ "mono", "/src/Mono-MyFirstNancy/bin/Release/Mono-MyFirstNancy.exe" ]