FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["QLDA_PCCV.csproj", "./"]
RUN dotnet restore "QLDA_PCCV.csproj"
COPY . .
RUN dotnet publish "QLDA_PCCV.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "QLDA_PCCV.dll"]
