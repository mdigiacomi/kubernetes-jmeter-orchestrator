FROM mcr.microsoft.com/dotnet/core/runtime:2.1.9-alpine3.9 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /src
COPY ["JMeter-Orchestrator/JMeter-Orchestrator.csproj", "JMeter-Orchestrator/"]
RUN dotnet restore "JMeter-Orchestrator/JMeter-Orchestrator.csproj"
COPY . .
WORKDIR "/src/JMeter-Orchestrator/"
RUN dotnet build "JMeter-Orchestrator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "JMeter-Orchestrator.csproj" -c Release -o /app

FROM base AS final
ARG JMETER_VERSION="5.1.1"
ENV JMETER_HOME /opt/apache-jmeter-${JMETER_VERSION}
ENV	JMETER_BIN	${JMETER_HOME}/bin
ENV	JMETER_DOWNLOAD_URL  https://archive.apache.org/dist/jmeter/binaries/apache-jmeter-${JMETER_VERSION}.tgz
WORKDIR /app
COPY --from=publish /app .

RUN    apk update \
	&& apk upgrade \
	&& apk add ca-certificates \
	&& update-ca-certificates \
	&& apk add --update openjdk8-jre tzdata curl unzip bash \
	&& rm -rf /var/cache/apk/* \
	&& mkdir -p /tmp/dependencies  \
	&& curl -L --silent ${JMETER_DOWNLOAD_URL} >  /tmp/dependencies/apache-jmeter-${JMETER_VERSION}.tgz  \
	&& mkdir -p /opt  \
	&& tar -xzf /tmp/dependencies/apache-jmeter-${JMETER_VERSION}.tgz -C /opt  \
	&& rm -rf /tmp/dependencies

RUN mkdir /jmeter

RUN ulimit -n 8172

ENV PATH $PATH:$JMETER_BIN
ENTRYPOINT ["dotnet", "JMeter-Orchestrator.dll"]