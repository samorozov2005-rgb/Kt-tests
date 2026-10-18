# Budget Calculator

Учебный проект по модульному тестированию метода ASP.NET Core REST API с помощью xUnit. Тесты вызывают `BudgetController.Calculate()` напрямую, без запуска HTTP-сервера.

## Структура

- `BudgetCalculator.Api` — минимальный ASP.NET Core Web API с моделью `BudgetRequest` и контроллером `BudgetController`.
- `BudgetCalculator.Tests` — xUnit-тесты контроллера и `coverlet.collector` для сбора coverage.

## Проверяемые сценарии

- обычный расчёт без страховки;
- расчёт со страховкой 5%;
- отрицательное количество дней;
- нулевое количество дней;
- нулевая стоимость дня;
- расчёт с `decimal`-значением `100.50m`.

Для успешных ответов проверяются `OkObjectResult`, статус 200 и `TotalAmount`. Для ошибок количества дней — `BadRequestObjectResult`, статус 400 и текст сообщения.

## Обнаруженный баг

Исходный код содержал:

```csharp
total *= 5;
```

Такой коэффициент давал 500% исходной суммы вместо добавления 5%. До исправления тест со страховкой ожидал `525`, но получал `2500`. После того как тест выявил ошибку, код был исправлен:

```csharp
total *= 1.05m;
```

## Запуск

Из корня solution:

```bash
dotnet restore
dotnet build
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

Отчёт coverage в формате Cobertura будет создан в папке `BudgetCalculator.Tests/TestResults`.
