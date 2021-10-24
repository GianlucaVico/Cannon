FROM mono:latest
RUN apt-get update && apt-get install gtk-sharp2 -y
COPY . .
# WORKDIR .
# CMD [ "msbuild",  "Cannon_GUI.csproj", "-t:Rebuild", "-p:Configuration=Release"]
WORKDIR /bin/Release
CMD [ "mono", "Cannon_GUI.exe" ]
