# Use the official .NET SDK 8 image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy everything and restore the project
COPY . ./
RUN dotnet restore SeleafAPI.csproj

# Build and publish the project
RUN dotnet publish SeleafAPI.csproj -c Release -o out

# Use the official ASP.NET Core runtime 8 image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install system fonts (necessary for PDF generation)
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    fonts-dejavu-core \
    fontconfig \
    && rm -rf /var/lib/apt/lists/*

# Copy the output from the build stage
COPY --from=build-env /app/out .

# Expose the port
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "SeleafAPI.dll"]
