# RabbitMQPlayground

### How to run RabbitMQ locally from docker image

`docker run -d --name playground -p 5672:5672 -p 15672:15672 rabbitmq:3-management`

Management UI: http://localhost:15672/

Defult username/password: guest/guest

<img width="1851" height="1039" alt="image" src="https://github.com/user-attachments/assets/18f26338-30ee-4ef7-9b14-c821b9532308" />

More information here: https://www.rabbitmq.com/docs/management

### How to run Jaeger locally from docker image

`docker run --name jaeger -p 13133:13133 -p 16686:16686 -p 4317:4317 -d --restart=unless-stopped jaegertracing/opentelemetry-all-in-one`

Jaeger UI: http://localhost:16686/search

### How to send first message

Run Api project and open `https://localhost:7245/swagger`and send Post `/messages` with text in body.

Open UI and open queue `test`, then Get messages and see text that was sent.

<img width="1910" height="839" alt="image" src="https://github.com/user-attachments/assets/7170fed0-a13d-434e-b4f5-bdb97768337a" />

<img width="1873" height="1039" alt="image" src="https://github.com/user-attachments/assets/73337ab1-bfc5-4524-b8fa-6cd9e7ae6e5c" />

### Tracing (Jaeger UI)

<img width="1834" height="647" alt="image" src="https://github.com/user-attachments/assets/09c64780-7d75-4811-a723-bc90e68ea92a" />


<img width="1916" height="643" alt="image" src="https://github.com/user-attachments/assets/c6b97742-0e10-4370-8eff-86fa2cc3ce8c" />


### Run in one-step with docker compose

Run `docker-compose up`

or `docker compose up --build` if something in code was changed and images have to be recreated.

<img width="1318" height="719" alt="image" src="https://github.com/user-attachments/assets/7d1715bf-db8a-4777-93c1-2f85b6c7c043" />

API: https://localhost:64671/swagger/index.html

RabbitMQ UI: http://localhost:15672/#/queues

Jaeger UI: http://localhost:16686/search

Clean-up

Run `docker-compose down`
