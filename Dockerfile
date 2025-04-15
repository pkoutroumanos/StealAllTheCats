# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and main project files
COPY ["StealAllTheCats.sln", "./"]
COPY ["StealAllTheCats/StealAllTheCats.csproj", "StealAllTheCats/"]

# Restore dependencies for the main project
RUN dotnet restore "StealAllTheCats/StealAllTheCats.csproj"

# Copy the rest of the source code
COPY . .

# Set working directory to the main project folder and publish the project
WORKDIR "/src/StealAllTheCats"
RUN dotnet publish "StealAllTheCats.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "StealAllTheCats.dll"]