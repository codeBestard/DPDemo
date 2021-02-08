FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY DP.MessagePublisher/DP.MessagePublisher.csproj DP.MessagePublisher/
COPY DP.MessageQueue/DP.MessageQueue.csproj DP.MessageQueue/
COPY DP.Common/DP.Common.csproj DP.Common/

RUN dotnet restore DP.MessagePublisher/DP.MessagePublisher.csproj
COPY . .
WORKDIR /src/DP.MessagePublisher
RUN dotnet build DP.MessagePublisher.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish DP.MessagePublisher.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DP.MessagePublisher.dll"]
