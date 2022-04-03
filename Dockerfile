#######################################################
# Step 1: Build the application in a container        #
#######################################################

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

ARG CONFIGURATION=Release

# Copy .sln and .csproj files for NuGet restore
COPY src/*.sln src/
COPY src/*.props src/
COPY src/CosmosDbSetup/CosmosDbSetup.csproj src/CosmosDbSetup/

# Restore NuGet packages
RUN dotnet restore src/CosmosDbSetup/CosmosDbSetup.csproj

# Copy the rest of the files over
COPY src/CosmosDbSetup/ src/CosmosDbSetup/

# Build the application
WORKDIR /src/CosmosDbSetup/
RUN dotnet publish --output /out/ --configuration $CONFIGURATION

#######################################################
# Step 2: Run the build outcome in a container        #
#######################################################

FROM mcr.microsoft.com/dotnet/runtime:6.0

COPY --from=build /out .

# Start the application
ENTRYPOINT ["dotnet", "CosmosDbSetup.dll"]
