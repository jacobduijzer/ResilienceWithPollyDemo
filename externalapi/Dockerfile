FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /src
COPY . .

FROM build AS publish
RUN dotnet restore
RUN dotnet publish "src/BuggyAnimalDetailsApi.Api/BuggyAnimalDetailsApi.Api.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:latest
WORKDIR /app
COPY --from=publish /out .

ENTRYPOINT ["dotnet", "BuggyAnimalDetailsApi.Api.dll"] 
