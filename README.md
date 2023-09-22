# .NET Modern API
An template API to provide a fast startup for .NET Core 7 APIs with the best practicies.
## Running
### Local
- In the path "src/DotNETModernAPI.Presentation/appsettings.Development.json", edit the "Email" Section with your email configuration:
    ```
    "UserName" // Your Email
    "From" // How the email will be named when an email is send 
    "Password" // Email password
    "Host" // Email SMTP Host
    "Port" // Email SMTP Port
    ```
- In the path "src/" execute in cmd:
    ```
    docker-compose up
    ```
- Now the application is running at:
    ```
    http://localhost:5000
    ```
- Done.
### Azure
To execute this application in Microsoft Azure you will need the following resources:
- Container Registry;
- App Service Plan;
- App Service;
    - Must be configured to run a Docker Container for Linux;
    - The Image Source must be a Docker Container Registry;
    - The Registry must be the created Container Registry.
#### Steps
- Create a Variable Group called "production";
- The Variable Group must have the following variables:
    - AppServiceName = *Name of created AppService*;
    - ContainerRegistryName = *Name of created Container Registry*;
    - ContainerRepositoryName = *Name of Docker Image*;
    - EmailFrom = *How the Email must be named*;
    - EmailHost = *Email Host*;
    - EmailPassword = *Email Password*;
    - EmailPort = *Email SMTP Port*;
    - EmailUserName = *Email*;
    - JWTSecret = *Secret used in JWTs*;
    - JWTValidAudience = *App Service Domain*;
    - JWTValidIssuer = *App Service Domain*;
    - SQLDBConnectionString = *PostgreSQL ConnectionString*;
    - SQLDBName = *PostgreSQL Database Name*;
    - SQLDBServer = *PostgreSQL Database Server*;
    - SQLDBUserName = *PostgreSQL UserName*;
    - SQLDBUserPassword = *PostgreSQL Password*;
    - Subscription = *Azure Subscription*;
- Create a new pipeline using an existing pipeline in the main branch with the path to the azure-pipelines.yml, in this repository is "pipelines/azure-pipelines.yml";
- Done.
# Stay in touch
- [Github](https://github.com/pedroo-csproj);
- [LinkedIn](https://www.linkedin.com/in/pedroo-csproj/);
- [X](https://twitter.com/pedro_csproj).
