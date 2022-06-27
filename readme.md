# WWT .Net Test Application


## Overiew
Microsoft .Net 6 Core Blazor and API application backed by a REDIS DB to be used for testing purposes only. 

Web App will display current information on server plus numebr of unique hits this server / container has received plus a combined for all other instances.

API allows for the the following endpoints:

            GET /counter/all - Combined Server Count
            GET /counter/server - Single Server Count
            GET /random - Returns Machine name and random string
            POST /load/{seconds} - Generate High CPU For {seconds}
            POST /kill - Stop application host [container will stop]

            

Redis Connection string is stored as a secret or alternatively can be set as a standard environment variable "REDIS_CONNECTION_STRING"


## Dependencies

* Kubernetes [1.22+]
* Ingress Controller
* LoadBalancer

## Installation Instructions

```

kubectl create ns wwttestapp

kubectl apply -f redis_url_secret.yml -n wwttestapp
kubectl apply -f deploy_app.yml -n wwttestapp
kubectl apply -f deploy_app_loadbalancer.yml -n wwttestapp


❯ kubectl get pods -n wwtwebapp
NAME                            READY   STATUS    RESTARTS   AGE
redis-cache-6cc6d55dc7-s4n6x    1/1     Running   0          44h
svclb-wwttestappservice-q7mkw   1/1     Running   0          44h
svclb-wwttestappservice-mxx26   1/1     Running   0          44h
svclb-wwttestappservice-dd49l   1/1     Running   0          44h
wwttestapp-ff9c4d796-cct77      1/1     Running   0          19h
wwttestapp-ff9c4d796-7kth4      1/1     Running   0          19h
wwttestapp-ff9c4d796-npzhz      1/1     Running   0          19h

```
Confirm all pods are running.

```

❯ kubectl  logs wwttestapp-ff9c4d796-7kth4 -n wwtwebapp
[22:52:54 WRN] Storing keys in a directory '/root/.aspnet/DataProtection-Keys' that may not be persisted outside of the container. Protected data will be unavailable when container is destroyed.
[22:52:54 INF] User profile is available. Using '/root/.aspnet/DataProtection-Keys' as key repository; keys will not be encrypted at rest.
[22:52:54 INF] Creating key {bb1d57dd-05f9-4411-8c0e-4db0b6188d73} with creation date 2022-06-26 22:52:54Z, activation date 2022-06-26 22:52:54Z, and expiration date 2022-09-24 22:52:54Z.
[22:52:54 WRN] No XML encryptor configured. Key {bb1d57dd-05f9-4411-8c0e-4db0b6188d73} may be persisted to storage in unencrypted form.
[22:52:54 INF] Writing data to file '/root/.aspnet/DataProtection-Keys/key-bb1d57dd-05f9-4411-8c0e-4db0b6188d73.xml'.
[22:52:54 INF] Now listening on: http://[::]:80
[22:52:54 INF] Application started. Press Ctrl+C to shut down.
[22:52:54 INF] Hosting environment: Production
[22:52:54 INF] Content root path: /app/

```

Confirm that pods are listening on http://[::]:80 - An error will be generated if the redisDb is not accessible.
```
❯ kubectl get svc -n wwtwebapp
NAME                TYPE           CLUSTER-IP      EXTERNAL-IP      PORT(S)        AGE
redis-cache         ClusterIP      10.43.14.213    <none>           6379/TCP       44h
wwttestapp          ClusterIP      10.43.200.253   <none>           80/TCP         3h9m
wwttestappservice   LoadBalancer   10.43.172.144   192.168.32.105   80:30531/TCP   44h
```
Get External IP of wwttestappservice of type **loadbalancer** and test webstie or API calls.

## Known issues
An intermittent websocket timeout might occur when connecting via HTTP. The background will appear but not text will be displayed. Refreshing the page should resovle the issue.
