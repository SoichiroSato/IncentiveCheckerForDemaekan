FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

RUN apt-get update &&\
    apt-get install -y wget gnupg curl
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add - &&\
    wget -q https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb &&\
    apt-get install -y ./google-chrome-stable_current_amd64.deb &&\
    rm google-chrome-stable_current_amd64.deb

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IncentiveCheckerforDemaekan/IncentiveCheckerforDemaekan.csproj", "IncentiveCheckerforDemaekan/"]
RUN dotnet restore "IncentiveCheckerforDemaekan/IncentiveCheckerforDemaekan.csproj"
COPY . .
WORKDIR "/src/IncentiveCheckerforDemaekan"
RUN dotnet build "IncentiveCheckerforDemaekan.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "IncentiveCheckerforDemaekan.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf && \
    sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "IncentiveCheckerforDemaekan.dll"]

