## Практическое задание ООО "Омега"

### Эндпоинты

Примеры корректных и некорректных запросов ко всем эндпоинтам есть
в [postman-коллекции](https://github.com/isthatkirill/omega-cats-practice/blob/main/postman/requests.json). 

Эндпоинты, доступные всем пользователям, в том числе неавторизованным:

- GET `/api/positions/{id}` - получение позиции по идентификатору
- GET `/api/positions` - получение всех позиций

Эндпоинты, доступные пользователям с ролью ADMIN:

- POST `/api/positions` - добавление позиции
- PUT `/api/positions/{id}` - редактирование позиции по идентификатору
- DELETE `/api/positions/{id}` - удаление позиции по идентификатору

> Для удобства тестирования пользователь с ролью ADMIN добавляется в базу данных до старта приложения с помощью
> миграций. Тажке добавляется несколько тестовых позиций.


### ER-диаграмма используемой БД

<p align="left">
 <img width="400px" src="https://github.com/isthatkirill/omega-cats-practice/blob/main/resources/er.png" alt="er"/>
</p>

### Инструкция по запуску

1. Склонируйте репозиторий `git clone https://github.com/isthatkirill/omega-cats-practice.git`
2. Перейдите в директорию с проектом `cd omega-cats-practice`
3. Запустите PostgreSQL локально на компьютере или воспользуйтесь заранее подготовленным в `docker-compose.yaml` контейнером, 
запустив его с помощью команды `docker compose up`.
4. Примените миграции к бд: `dotnet ef migrations add InitialCreate`, `dotnet ef database update`.
5. Запутстите приложение с помощью `dotnet run` или через среду разработки.


