# Transaction
Gateway level transaction management micro services


## Amorphie.Transaction

To Run
```
dapr run --app-id amorphie-transaction  --app-port 5001  --dapr-http-port 50001 --components-path Components dotnet run -- urls=http://localhost:5001/
```

## Amorphie.Transaction.Hub

To Run
```
dapr run --app-id amorphie-transaction-hub  --app-port 5009  --dapr-http-port 50009 --components-path Components dotnet run -- urls=http://localhost:5009/ 
```


## Amorphie.Transaction.Worker

To Run
```
dapr run --app-id amorphie-transaction-worker  --app-port 5019  --dapr-http-port 50019 --components-path Components dotnet run -- urls=http://localhost:5019/ 
```


