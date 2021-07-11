FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

RUN apt-get update  && \
	apt-get install curl gnupg -yq && \ 
	curl -sL https://deb.nodesource.com/setup_13.x | bash - && \
	apt-get install -y nodejs && \
	
	npm install -g npm

WORKDIR /build
COPY ./Src/CSharpier.Playground/CSharpier.Playground.csproj Src/CSharpier.Playground/
COPY ./Src/CSharpier/CSharpier.csproj Src/CSharpier/
RUN dotnet restore "Src/CSharpier.Playground/CSharpier.Playground.csproj"

COPY ./Src/CSharpier.Playground/ClientApp/package.json Src/CSharpier.Playground/ClientApp/
COPY ./Src/CSharpier.Playground/ClientApp/package-lock.json Src/CSharpier.Playground/ClientApp/
WORKDIR /build/Src/CSharpier.Playground/ClientApp
RUN npm install

WORKDIR /build
COPY . .

FROM build AS publish
RUN dotnet publish "Src/CSharpier.Playground/CSharpier.Playground.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpier.Playground.dll"]