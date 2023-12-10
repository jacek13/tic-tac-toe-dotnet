FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

EXPOSE 5000

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore ./TicTacToe.webapi/TicTacToe.webapi.csproj
# Build and publish a release
RUN dotnet publish ./TicTacToe.webapi/TicTacToe.webapi.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "TicTacToe.webapi.dll"]