# Этап 1: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["ServerInstancingService.csproj", "./"]
RUN dotnet restore "ServerInstancingService.csproj"

# Копируем весь остальной исходный код и собираем проект
COPY . .
RUN dotnet build "ServerInstancingService.csproj" -c Release -o /app/build

# Этап 2: Публикация
FROM build AS publish
RUN dotnet publish "ServerInstancingService.csproj" -c Release -o /app/publish

# Этап 3: Финальный образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
# Открываем стандартный порт внутри контейнера
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerInstancingService.dll"]