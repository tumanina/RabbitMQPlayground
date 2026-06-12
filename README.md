# RabbitMQPlayground

RabbitMQ is a message broker that enables asynchronous communication between services. Messages are published to exchanges, which route them to queues; for example, a fanout exchange broadcasts a message to all bound queues.

This project also contain examples of using MassTransit.

MassTransit is a .NET service bus framework built on top of RabbitMQ. It simplifies message publishing and consumption by handling serialization, routing, retries, error queues, and consumer registration, allowing developers to focus on business logic instead of messaging infrastructure.


## Event-driven messaging

Implemented an event-driven architecture using RabbitMQ and MassTransit, where the API publishes domain events and consumers process them asynchronously. This approach improves scalability, fault tolerance, and service decoupling.

<img width="710" height="275" alt="image" src="https://github.com/user-attachments/assets/072d76f2-7f01-4e4d-812e-668dc5b2c22d" />

### How to test

Run Api project and open `https://localhost:7245/swagger`and send Post `/users` with text in body.

<img width="936" height="470" alt="image" src="https://github.com/user-attachments/assets/7f869eb3-6c10-4786-b63e-ffdecdf465fd" />


### Exchange (MassTransit)

Open UI and tab Exchanges, find `RabbitMQPlayground.Contracts:User`

<img width="927" height="450" alt="image" src="https://github.com/user-attachments/assets/c4ae3821-be56-4c00-b5ee-9f746e1a2aa9" />

This exchange does not have binding, it can be done manually or it will be done automatically after consumer starts (queue User will be created automatically).

<img width="924" height="342" alt="image" src="https://github.com/user-attachments/assets/01cc9b8e-e507-42bd-a432-1607732e10cd" />

<img width="927" height="431" alt="image" src="https://github.com/user-attachments/assets/50afb2d2-9b20-455e-a6a2-fb864e2c3eeb" />


### Queue

Open UI and open queue `test`, then Get messages and see text that was sent.

<img width="945" height="420" alt="image" src="https://github.com/user-attachments/assets/7170fed0-a13d-434e-b4f5-bdb97768337a" />

<img width="932" height="520" alt="image" src="https://github.com/user-attachments/assets/73337ab1-bfc5-4524-b8fa-6cd9e7ae6e5c" />

## Request/Response Messaging

Implemented the Request/Response pattern over RabbitMQ using MassTransit request clients. Services communicate through correlated request and response messages, providing reliable inter-service communication without direct HTTP dependencies.

<img width="700" height="370" alt="image" src="https://github.com/user-attachments/assets/6e6a36d3-ddc7-4a53-bc32-3f762742cec2" />

### How to test



## Infrastructure

### How to run RabbitMQ locally from docker image

`docker run -d --name playground -p 5672:5672 -p 15672:15672 rabbitmq:3-management`

Management UI: http://localhost:15672/

Defult username/password: guest/guest

<img width="926" height="520" alt="image" src="https://github.com/user-attachments/assets/18f26338-30ee-4ef7-9b14-c821b9532308" />

More information here: https://www.rabbitmq.com/docs/management

### Run in one-step with docker compose

Run `docker-compose up`

<img width="659" height="360" alt="image" src="https://github.com/user-attachments/assets/7d1715bf-db8a-4777-93c1-2f85b6c7c043" />

API: https://localhost:64671/swagger/index.html

RabbitMQ UI: http://localhost:15672/#/queues

Clean-up

Run `docker-compose down`

### Configure mongodb to store messages

Run Compass from image
`docker run -d -p 8080:8080 -e CW_MONGO_URI="mongodb://host.docker.internal:27017" haohanyang/compass-web`

Open ui http://localhost:8080/

Set database naem and collection name:
<img width="863" height="531" alt="image" src="https://github.com/user-attachments/assets/fb84da66-caaa-4af2-b95c-131f3fe4f32e" />

