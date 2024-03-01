FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env
WORKDIR /src
COPY CustomContentConnectorExample.csproj .
RUN dotnet restore "CustomContentConnectorExample.csproj"
COPY . .
RUN dotnet publish "CustomContentConnectorExample.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as runtime-env
WORKDIR /app
COPY --from=build-env /publish .

EXPOSE 7225

ENV ASPNETCORE_URLS=http://+:7225
ENTRYPOINT ["dotnet", "CustomContentConnectorExample.dll"]