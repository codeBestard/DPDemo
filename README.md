# DPDemo
A simple distributed system with messaging queues

The project demostrates how we use message queues to achieve workload distribution and how the systems will scale in order to accommodate the incoming workloads.

We can run this demo in the local and cloud (Kubernets) environnments.

### Requirements
- [.Net Core](https://dotnet.microsoft.com/)
- [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview) or  [Nats](https://nats.io/) 
- [Kubernetes](https://kubernetes.io/) 
- [Docker](https://www.docker.com/)

### Instructions:
##### Run locally without Kubernets
1. Setup the Messaging Queues System ( Azure service bus or NATS ) with Docker
2. Execute 
   > .\local_start.ps1 -mq "nats" -worker 5 -batches 3 -batchSize 30
3. Cleanup
   > .\local_stop.ps1

##### Run in Kubernetes
1. Create the docker images for different components and upload the images to repository
2. Steup the Messaging Queue system  ( Azure service bus or NATS)
4. Execute
   > kubectl_apply.bat
5. Cleanup
   > Kubectl_delete.bat
