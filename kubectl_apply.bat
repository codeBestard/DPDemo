@echo off
kubectl apply -f .\AKS\nats-deployment.yml

timeout 10

kubectl apply -f .\AKS\handler-deployment.yml
::ping 127.0.0.1 -n 20 > nul

kubectl apply -f .\AKS\handler-hpa.yml


timeout 25

kubectl apply -f .\AKS\publisher-job.yml