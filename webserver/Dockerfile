# Use the .NET SDK 7.0 image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy the project files and restore the dependencies
COPY ./*.csproj ./
RUN dotnet restore

# Copy the rest of the application and build it
COPY . ./
RUN dotnet build -c Release -o out

# Use the ASP.NET Core 7.0 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port the application listens on
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "webserver.dll"]