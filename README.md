# RabbitMQPlayground

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


### Run in one-step with docker compose

Run `docker-compose up`

<img width="1873" height="1039" alt="image" src="https://github.com/user-attachments/assets/73337ab1-bfc5-4524-b8fa-6cd9e7ae6e5c" />

