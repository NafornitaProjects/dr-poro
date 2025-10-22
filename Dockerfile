FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /source

COPY *.sln .
COPY Dr-Poro/ ./Dr-Poro/


RUN dotnet restore

RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS pydeps

RUN apt-get update && apt-get install -y --no-install-recommends python3.11 python3.11-venv python3-pip && rm -rf /var/lib/apt/lists/*

WORKDIR /python

COPY Dr-Poro/requirements.txt .

RUN /usr/bin/python3.11 -m venv /opt/pyenv && /opt/pyenv/bin/pip install --upgrade pip && /opt/pyenv/bin/pip install -r requirements.txt

FROM mcr.microsoft.com/dotnet/aspnet:9.0

RUN apt-get update && apt-get install -y --no-install-recommends python3.11 python3.11-venv python3-pip && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app ./
COPY --from=pydeps /opt/pyenv /opt/pyenv

ENV PYTHONNET_PYDLL=/usr/lib/x86_64-linux-gnu/libpython3.11.so
ENV VIRTUAL_ENV="/opt/pyenv"
ENV PATH="/opt/pyenv/bin:${PATH}"
ENV LD_LIBRARY_PATH="/usr/lib/x86_64-linux-gnu:${LD_LIBRARY_PATH}"

ENTRYPOINT ["dotnet", "Dr-Poro.dll"]
