# PaperTendiesBot
 Intial start to a C#/.NetCore trading bot, specifically using the Alpaca API. Right now this is using an example algorithm. Intentions are to continue to grow it out with some handspun trading approaches.
 
 ## Running Locally
 - Ensure you have .Netcore installed locally: `dotnet --info`
 - Create a free, paper trading account over at `https://alpaca.markets/`; Generate a new API Key
 - To run, you will need to put your Alpaca API Key and secret into the `appsettings.json` file.
 - Build with `dotnet build`
 - Run with `dotnet run`
 
## Running in Docker
 - Install docker locally first
 - From root `docker-compose build`
 - Start in detached mode: `docker-compose up -d` 
 - View Logs: `docker-compose ps`  *Shows all logs, but its the only running right now*

## TODO List
 - ~~Dockerize Service locally~~
 - Create Azure deployment; Includes storing/pulling secrets in azure
 - Generate personal trading algo
 - Explore ML approaches to gauge Market Sentiment
 - Print Tendies
