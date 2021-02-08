FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["DP.Common/DP.Common.csproj", "DP.Common/"]
COPY ["DP.MessageHandler/DP.MessageHandler.csproj", "DP.MessageHandler/"]
COPY ["DP.MessageQueue/DP.MessageQueue.csproj", "DP.MessageQueue/"]

RUN dotnet restore "DP.MessageHandler/DP.MessageHandler.csproj"
COPY . .
WORKDIR "/src/DP.MessageHandler"
RUN dotnet build "DP.MessageHandler.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DP.MessageHandler.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DP.MessageHandler.dll"]