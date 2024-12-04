# launch dapr sidecar to interact with the default state store
dapr run --app-port 5010 --app-id simplewebapi --app-protocol http --dapr-http-port 6000

