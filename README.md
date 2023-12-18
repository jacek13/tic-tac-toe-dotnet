# Tic Tac Toe - Multiplayer

The repository contains the source code for a multiplayer game of tic-tac-toe. Backend was written in `C#` using `.Net 8`. The project uses [SignalR](https://github.com/SignalR/SignalR), [Serilog](https://github.com/serilog/serilog), EF Core for Postgresql and Swagger. The frontend was written using Angular and node 20.10.0. 

Backend has been integrated with AWS Cognito using [ASP.NET Core Identity Provider for Amazon Cognito](https://github.com/aws/aws-aspnet-cognito-identity-provider).


The application allows users logged in and not logged in to play tic-tac-toe. Logged-in users are listed on the global scoreboard while the data of non-logged-in users is not accessible from the frontend. The app also has a simple chat feature that allows for communion in the game room.


# Project launch

### **Local environment** (without .Net and angular cli)

**requirements:**
- docker
- docker-compose

Go into folder `backend/TicTacToe.webapi` and modify the file `appsettings.dockerlocal.json`

```json
{
  "db": {
    "Connection": "Host=db;Port=5432;Database=local_db;Username=admin_local;Password=Admin123!;"
  },
  "AWS": {
    "Region": "<your region id goes here>",
    "UserPoolClientId": "<your user pool client id goes here>",
    "UserPoolClientSecret": "<your user pool client secret goes here>",
    "UserPoolId": "<your user pool id goes here>",
    "Authority": "https://cognito-idp.<your region id goes here>.amazonaws.com/<your user pool id goes here>"
  }
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Extensions.Hosting" ],
    "MinimalLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "System": "Information"
      }
    },
    "Enrich": [ "FromLogContext", "WithMachineName", "WithProcessId", "WithThreadId" ],
    "WriteTo": [
      {
        "Name": "Console"
      }
    ],
    "Properties": {
      "Application": "Tic-Tac-Toe"
    }
  }
}

```

After that go to main folder and run:

```bash
docker-compose up
```

After the correct build, you should see on the terminal output

```bash
Creating network "tic-tac-toe-dotnet_internal" with the default driver
Creating tic-tac-toe-dotnet_db_1 ... done
Creating tic-tac-toe-dotnet_webapi_1 ... done
Creating tic-tac-toe-dotnet_frontend_1 ... done
Attaching to tic-tac-toe-dotnet_db_1, tic-tac-toe-dotnet_webapi_1, tic-tac-toe-dotnet_frontend_1
db_1        |
db_1        | PostgreSQL Database directory appears to contain a database; Skipping initialization
db_1        |
db_1        | 2023-12-18 10:11:00.781 UTC [1] LOG:  starting PostgreSQL 16.1 (Debian 16.1-1.pgdg120+1) on x86_64-pc-linux-gnu, compiled by gcc (Debian 12.2.0-14) 12.2.0, 64-bit
db_1        | 2023-12-18 10:11:00.781 UTC [1] LOG:  listening on IPv4 address "0.0.0.0", port 5432
db_1        | 2023-12-18 10:11:00.781 UTC [1] LOG:  listening on IPv6 address "::", port 5432
db_1        | 2023-12-18 10:11:00.791 UTC [1] LOG:  listening on Unix socket "/var/run/postgresql/.s.PGSQL.5432"
db_1        | 2023-12-18 10:11:00.808 UTC [29] LOG:  database system was shut down at 2023-12-17 16:40:15 UTC
db_1        | 2023-12-18 10:11:00.840 UTC [1] LOG:  database system is ready to accept connections
frontend_1  | /docker-entrypoint.sh: /docker-entrypoint.d/ is not empty, will attempt to perform configuration
frontend_1  | /docker-entrypoint.sh: Looking for shell scripts in /docker-entrypoint.d/
frontend_1  | /docker-entrypoint.sh: Launching /docker-entrypoint.d/10-listen-on-ipv6-by-default.sh
frontend_1  | 10-listen-on-ipv6-by-default.sh: info: /etc/nginx/conf.d/default.conf is not a file or does not exist
frontend_1  | /docker-entrypoint.sh: Sourcing /docker-entrypoint.d/15-local-resolvers.envsh
frontend_1  | /docker-entrypoint.sh: Launching /docker-entrypoint.d/20-envsubst-on-templates.sh
frontend_1  | /docker-entrypoint.sh: Launching /docker-entrypoint.d/30-tune-worker-processes.sh
webapi_1    | [10:11:03 WRN] The property 'MatchView.Board' is a collection or enumeration type with a value converter but with no value comparer. Set a value comparer to ensure the collection/enumeration elements are compared correctly.
webapi_1    | [10:11:03 INF] Now listening on: http://0.0.0.0:5000
webapi_1    | [10:11:03 INF] Application started. Press Ctrl+C to shut down.
webapi_1    | [10:11:03 INF] Hosting environment: dockerlocal
webapi_1    | [10:11:03 INF] Content root path: /App
```

### **EC2 environment**

Connect to your EC2 instance via SSH. Run this commands:

```bash
sudo yum update -y
sudo yum install -y docker
sudo service docker start
sudo usermod -a -G docker ec2-user
sudo docker ps
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
sudo docker-compose --version
```

Then download the application's source code (**via git**) or its images (**upload the built images to your private repositories and download**) and run as you would for a local environment. If you have built images before then remember to change the docker-compose.yml file (change section build to image).