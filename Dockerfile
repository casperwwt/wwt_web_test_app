FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WWT.NetTest.Blazor/WWT.NetTest.Blazor.csproj", "WWT.NetTest.Blazor/"]
RUN dotnet restore "WWT.NetTest.Blazor/WWT.NetTest.Blazor.csproj"
COPY . .
WORKDIR "/src/WWT.NetTest.Blazor"
RUN dotnet build "WWT.NetTest.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WWT.NetTest.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WWT.NetTest.Blazor.dll"]
