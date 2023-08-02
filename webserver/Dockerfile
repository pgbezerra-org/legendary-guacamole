# Use the .NET SDK 7.0 image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Set the environment variables to Production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_ENVIRONMENT=Production

WORKDIR /app

# Copy the project files and restore the dependencies
COPY ./*.csproj ./
RUN dotnet restore

# Copy the rest of the application
COPY . ./

# Build the application
RUN dotnet build -c Release -o out

# Use a volume to persist the static files
VOLUME /app/wwwroot

# Set the working directory to the output folder
WORKDIR /app/out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Copy the application build from the previous stage
COPY --from=build-env /app/out ./

EXPOSE 80

ENTRYPOINT ["dotnet", "webserver.dll"]
