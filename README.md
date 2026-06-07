# RabbitMQPlayground

RabbitMQ is a message broker that enables asynchronous communication between services. Messages are published to exchanges, which route them to queues; for example, a fanout exchange broadcasts a message to all bound queues.

This project also contain examples of using MassTransit.

MassTransit is a .NET service bus framework built on top of RabbitMQ. It simplifies message publishing and consumption by handling serialization, routing, retries, error queues, and consumer registration, allowing developers to focus on business logic instead of messaging infrastructure.

### How to run RabbitMQ locally from docker image

`docker run -d --name playground -p 5672:5672 -p 15672:15672 rabbitmq:3-management`

Management UI: http://localhost:15672/

Defult username/password: guest/guest

<img width="1851" height="1039" alt="image" src="https://github.com/user-attachments/assets/18f26338-30ee-4ef7-9b14-c821b9532308" />

More information here: https://www.rabbitmq.com/docs/management

### How to send first message

Run Api project and open `https://localhost:7245/swagger`and send Post `/messages` with text in body.

Open UI and open queue `test`, then Get messages and see text that was sent.

<img width="1910" height="839" alt="image" src="https://github.com/user-attachments/assets/7170fed0-a13d-434e-b4f5-bdb97768337a" />

<img width="1873" height="1039" alt="image" src="https://github.com/user-attachments/assets/73337ab1-bfc5-4524-b8fa-6cd9e7ae6e5c" />

### Run in one-step with docker compose

Run `docker-compose up`

<img width="1318" height="719" alt="image" src="https://github.com/user-attachments/assets/7d1715bf-db8a-4777-93c1-2f85b6c7c043" />

API: https://localhost:64671/swagger/index.html

RabbitMQ UI: http://localhost:15672/#/queues


Clean-up

Run `docker-compose down`

### Configure mongodb to store messages

Run Compass from image
`docker run -d -p 8080:8080 -e CW_MONGO_URI="mongodb://host.docker.internal:27017" haohanyang/compass-web`

Open ui http://localhost:8080/

Set database naem and collection name:
<img width="1726" height="1063" alt="image" src="https://github.com/user-attachments/assets/fb84da66-caaa-4af2-b95c-131f3fe4f32e" />

