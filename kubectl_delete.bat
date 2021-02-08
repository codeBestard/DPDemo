@echo off
kubectl delete -f .\AKS\nats-deployment.yml
kubectl delete -f .\AKS\nats-svc.yml

kubectl delete -f .\AKS\handler-hpa.yml
kubectl delete -f .\AKS\handler-deployment.yml


kubectl delete -f .\AKS\publisher-job.yml