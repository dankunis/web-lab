FROM microsoft/dotnet:2.2.103-sdk

WORKDIR /source
COPY . .

RUN dotnet publish ./Homework1/Homework1.csproj --output /app
COPY ./Homework1/appsettings.json /app/
COPY ./Homework1/appsettings.Development.json /app/
RUN rm -rf /source

ENTRYPOINT ["/usr/bin/dotnet", "/app/Homework1.dll"]