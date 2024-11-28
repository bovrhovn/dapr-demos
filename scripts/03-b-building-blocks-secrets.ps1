# set environment variable
[System.Environment]::SetEnvironmentVariable("mysecret","This is secret from my environment variable")
# run dapr on port 3900
dapr run --app-id my-secrets-app --dapr-http-port 3900 --resources-path "D:\Projects\dapr-demos\ATD\ATD.BB.Secrets\Components"