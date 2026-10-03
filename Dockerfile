FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY GymAssistant_API.csproj ./
RUN dotnet restore GymAssistant_API.csproj
COPY . .
RUN dotnet publish GymAssistant_API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
# Writable upload folder for the non-root user
RUN mkdir -p /app/wwwroot/images && chown -R $APP_UID /app/wwwroot
USER $APP_UID
ENTRYPOINT ["dotnet", "GymAssistant_API.dll"]
