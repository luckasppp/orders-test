# OrdersTest

API REST para gestão de pedidos com arquitetura assíncrona via mensageria e frontend web integrado.

O fluxo principal: o cliente envia um pedido à API, que publica uma mensagem no RabbitMQ e responde `202 Accepted` imediatamente. Um consumer processa a mensagem em background e persiste o pedido no banco. As consultas são atendidas diretamente do banco.

---

## Stack Técnica

| Camada | Tecnologia |
|---|---|
| Runtime | .NET 10 |
| API | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Banco de dados | SQL Server Express |
| Mensageria | RabbitMQ + MassTransit 8.5.2 |
| Frontend | Vue 3 + Vite |
| Documentação API | Scalar.AspNetCore |
| Logging | Serilog |
| Cache | MongoDB |
| Observabilidade | OpenTelemetry |
| Testes | xUnit, Moq, FluentAssertions |
| Container | Docker + Docker Compose |
| Pipeline CI/CD | Azure DevOps |

---

## Pré-requisitos (para rodar containerizado)

- **Docker Desktop** com WSL2 habilitado
- **SQL Server Express 2022+** instalado nativamente no Windows
- **.NET 10 SDK** — necessário apenas se for executar os testes ou a aplicação fora do container (https://dotnet.microsoft.com/download/dotnet/10.0)
- **Imagens base Docker** disponíveis localmente ou acesso ao `mcr.microsoft.com`:
  - `mcr.microsoft.com/dotnet/sdk:10.0`
  - `mcr.microsoft.com/dotnet/aspnet:10.0`
  - `rabbitmq:3-management`
  - `mongo:7`

---

## Configuração inicial do SQL Server Express

O SQL Server precisa aceitar conexões TCP e SQL Authentication, já que a API executa dentro de um container e acessa o banco via `host.docker.internal`.

### 1. Habilitar TCP/IP

Abra o **SQL Server Configuration Manager**:

- **SQL Server Network Configuration** → **Protocols for SQLEXPRESS**
- Habilite **TCP/IP**
- Em **IPAll**, defina **TCP Port** como `1433`
- Reinicie o serviço `SQL Server (SQLEXPRESS)`

### 2. Habilitar Mixed Mode e criar senha do `sa`

No PowerShell como administrador (ajuste `MSSQL17` conforme a versão instalada):

```powershell
$path = "HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server\MSSQL17.SQLEXPRESS\MSSQLServer"
Set-ItemProperty -Path $path -Name LoginMode -Value 2
Restart-Service "MSSQL`$SQLEXPRESS"

Start-Sleep -Seconds 15

sqlcmd -S localhost\SQLEXPRESS -E -C -Q "ALTER LOGIN sa ENABLE; ALTER LOGIN sa WITH PASSWORD = 'Dev@Password123';"
```

### 3. Liberar a porta 1433 no firewall do Windows

Em ambientes onde o Docker Desktop usa WSL2, o firewall do Windows pode bloquear conexões vindas de containers. PowerShell como administrador:

```powershell
New-NetFirewallRule -DisplayName "SQL Server 1433" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
```

### 4. Validar

```powershell
sqlcmd -S localhost\SQLEXPRESS -U sa -P "Dev@Password123" -C -Q "SELECT @@VERSION"
```

A versão do SQL Server deve ser retornada.

---

## Como rodar

A stack (API, frontend, RabbitMQ, MongoDB) é orquestrada via Docker Compose. O SQL Server permanece nativo no host e é acessado pelos containers através de `host.docker.internal`. O schema do banco é criado automaticamente no primeiro startup da API via EF Core migrations — não é necessário executar nenhum comando manual de migração.

```bash
git clone https://github.com/damascenos-dev/orders-test.git
cd orders-test
docker compose up -d --build
```

A primeira execução leva aproximadamente 3 minutos (build das imagens da API e do frontend). Execuções subsequentes usam cache e sobem em segundos.

Para encerrar:

```bash
docker compose down
```

Para resetar volumes (limpa dados do MongoDB):

```bash
docker compose down -v
```

### Logs

```bash
docker compose logs -f              # todos os serviços
docker compose logs -f orders-api   # apenas a API
docker compose logs -f orders-web   # apenas o frontend
```

### Executando os testes

```bash
dotnet test
```

A suíte tem 7 testes unitários cobrindo a lógica de negócio do `OrderService`. Os testes usam Moq para substituir as dependências. Nenhuma infraestrutura externa é necessária para rodar.

---

## URLs após subir

| Serviço | URL |
|---|---|
| Frontend | http://localhost:3000 |
| API | http://localhost:5000 |
| Documentação da API (Scalar) | http://localhost:5000/scalar/v1 |
| RabbitMQ Management | http://localhost:15672 |

Credenciais padrão do RabbitMQ: `guest` / `guest`.

> A documentação completa dos endpoints, incluindo exemplos de request e response, está disponível no Scalar no link da tabela acima.

---

## Estrutura do Projeto

```
orders-test/
├── azure-pipelines.yml
├── docker-compose.yml
├── Orders.slnx
│
├── OrdersApi/
│   ├── Api/
│   │   └── Controllers/
│   ├── Application/
│   │   └── UseCases/
│   ├── Domain/
│   │   ├── Entities/
│   │   └── Repositories/
│   ├── Infrastructure/
│   │   ├── Cache/
│   │   ├── Messaging/
│   │   └── Persistence/
│   ├── Shared/
│   │   └── Dtos/
│   ├── Migrations/
│   ├── Program.cs
│   └── Dockerfile
│
├── OrdersWeb/
│   ├── src/
│   │   ├── views/
│   │   │   ├── HomeView.vue
│   │   │   ├── NewOrderView.vue
│   │   │   └── OrderDetailView.vue
│   │   ├── App.vue
│   │   └── main.js
│   ├── index.html
│   ├── nginx.conf
│   ├── vite.config.js
│   └── Dockerfile
│
└── OrdersApi.Tests/
    └── Unit/
```

---

## Notas

- **SQL Server nativo, não containerizado.** Banco de dados em container tem limitações de I/O, durabilidade de volume e backup que o tornam inadequado para produção. Em ambientes reais, o banco vem de serviço gerenciado (Azure SQL Database, RDS). Localmente, o SQL Express nativo é acessado pelos containers via `host.docker.internal` (configurado com `extra_hosts: ["host.docker.internal:host-gateway"]` no compose para garantir resolução em qualquer setup Docker Desktop).

- **Migrations automáticas no startup.** A API executa `Database.Migrate()` ao inicializar. O schema é criado e atualizado automaticamente, sem necessidade de intervenção manual. Em ambientes de produção com múltiplas instâncias, a migração deve ser desacoplada do startup e executada via pipeline de deploy, evitando race conditions.

- **Configuração via variáveis de ambiente.** Connection strings, hosts e URLs são lidos via `IConfiguration` com fallback. Isso permite que o mesmo binário funcione em desenvolvimento nativo (lendo do `appsettings.json`) e em container (sobrescrito pelo `docker-compose.yml`), sem rebuild.

- **`restart: unless-stopped` na API.** O `depends_on` do Docker Compose garante apenas ordem de início, não readiness. Como RabbitMQ e SQL Server podem levar ~20-40s para aceitar conexões, a API pode falhar nas primeiras tentativas. O restart automático lida com isso. Em ambiente de produção, o mecanismo apropriado seriam healthchecks com `condition: service_healthy`.

- **MassTransit v8.** A versão 9 migrou para licença comercial em Q1 2026. A v8 (Apache 2.0) permanece recebendo patches e cobre os casos de uso necessários.

- **Documentação Scalar.** Disponível apenas em ambiente `Development`. A variável `ASPNETCORE_ENVIRONMENT=Development` já está configurada no serviço `orders-api` do compose para expor `/scalar/v1`.

- **Imagens do Microsoft Container Registry.** Em algumas redes residenciais no Brasil, o download de imagens de `mcr.microsoft.com` pode sofrer interrupções (erros `EOF` no meio do pull). Se ocorrer, tente através de outra rede (4G, outro provedor) — uma vez que as imagens estão no cache local do Docker, o build funciona em qualquer rede. Esse é um problema específico de roteamento entre ISPs brasileiros e a CDN da Azure, não da imagem em si.

- **Cache MongoDB com padrão cache-aside.** O endpoint `GET /orders/{id}` consulta o MongoDB antes de ir ao SQL Server. Em caso de cache miss, o dado é buscado no SQL, armazenado no Mongo e retornado. Em caso de cache hit, o SQL não é consultado. O `GET /orders` lê diretamente do SQL — listas com ordenação têm invalidação complexa e o ganho não justifica o custo nesse escopo.

- **OpenTelemetry.** Traces de cada requisição HTTP são exportados para o console via `OpenTelemetry.Exporter.Console`. Visível em `docker compose logs orders-api`. Cada trace inclui endpoint, duração, status code e identificação do serviço.

---

## Autor

Lucas Damasceno
