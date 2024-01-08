# Use the official .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the .csproj and restore as distinct layers
# WORKDIR /src
COPY Processor/Processor.csproj ./
RUN dotnet restore

# Copy the remaining source code and build the application
COPY . ./
#WORKDIR /src/Processor
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app/out .

# Entry point when the container starts
ENTRYPOINT ["dotnet", "Processor.dll"]