# launch dapr sidecar to interact with the default state store
dapr run --app-id hello-dapr --dapr-http-port 3600
Write-Host "Dapr sidecar is running on port 3600, invoking http call"
# run the HTTP call to interact with sidecar
Invoke-RestMethod -Method Post -ContentType 'application/json' -Body '[{ "key": "name", "value": "Hello ATD"}]' -Uri 'http://localhost:3600/v1.0/state/statestore'
Write-Host "Reading the state from the state store in redis"
docker exec -it dapr_redis redis-cli
keys *
hgetall "hello-dapr||name"
Write-Host "Show how Dapr is configured to use Redis as the default state store"
Get-Content "$Env:USERPROFILE\.dapr\components\statestore.yaml"