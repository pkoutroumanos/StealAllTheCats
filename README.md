# StealAllTheCats

An ASP.NET Core Web API that fetches cat images from an external API, saves them to a SQL Server database, and exposes endpoints with Swagger documentation. Unit tests are also included.

## Prerequisites

- .NET 8 SDK
- Microsoft SQL Server (or SQL Server Express)
- (Optional) Docker

## Getting Started

### 1. Clone the Repository

git clone https://github.com/pkoutroumanos/StealAllTheCats.git

cd StealAllTheCats

### 2. Configure and Set Up the Database

Open the file StealAllTheCats/appsettings.json and update the connection string with your SQL Server details.

Example connection string:

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=CatsDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
}
In a terminal, navigate to the StealAllTheCats project folder and run:
dotnet ef database update --project StealAllTheCats

### 3. Run the Application

Using .NET CLI

cd StealAllTheCats

dotnet run

Then open your browser and visit https://localhost:{port}/swagger to view the API documentation.

## Unit Tests are included-Running Unit Tests

Run the following command from the root folder:

dotnet test

## Docker (Optional)
A Dockerfile is provided in the root folder. To build and run the application using Docker:

Build the Docker image:

docker build -t steal-all-the-cats .

Run the Docker container:

docker run -d -p 80:80 --name steal-all-the-cats-container steal-all-the-cats

Access the application at http://localhost (Swagger UI will be at /swagger).


## Contact
For questions or feedback  email pankoutsd@gmail.com
