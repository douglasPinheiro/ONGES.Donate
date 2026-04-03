# ONGES.Donate

Microservico responsavel pelo recebimento e processamento de doacoes da plataforma ONGES.

O projeto foi estruturado no mesmo padrao dos repositorios `ONGES` e `ONGES.Users.Api`, com separacao por camadas e um `Consumer` dedicado para processamento assincrono.

## Visao Geral

Fluxo principal:

1. um usuario autenticado envia uma doacao para a API
2. a API valida os dados recebidos
3. a API verifica se a campanha existe
4. a API verifica se a campanha esta ativa
5. a API publica uma mensagem interna para processamento
6. o `Consumer` consome essa mensagem
7. o `Consumer` persiste a doacao no banco
8. o `Consumer` publica uma mensagem para o `ONGES (Campaign)` atualizar o valor arrecadado

Mensagem publicada para o `ONGES (Campaign)`:

```json
{
  "CampaignId": "550e8400-e29b-41d4-a716-446655440000",
  "Amount": 500.00,
  "DonatedAt": "2026-04-03T10:30:00Z"
}
```

## Estrutura da Solucao

O repositorio contem os seguintes projetos:

- `ONGES.Donate.Api`
- `ONGES.Donate.Application`
- `ONGES.Donate.Domain`
- `ONGES.Donate.Infrastructure`
- `ONGES.Donate.Consumer`
- `ONGES.Donate.Test`

Responsabilidade de cada camada:

- `Api`: endpoint HTTP, autenticacao JWT e exposicao da API
- `Application`: DTOs, interfaces e implementacao dos services/casos de uso
- `Domain`: entidades, enums, regras de dominio e resultados
- `Infrastructure`: persistencia, repositorios, mensageria e integracoes tecnicas
- `Consumer`: processamento assincrono das mensagens internas
- `Test`: testes unitarios da solucao

## Tecnologias

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core
- SQL Server
- Azure Service Bus
- FluentValidation
- xUnit

## Endpoint Principal

Rota:

```text
POST /v1/donations
```

Autenticacao:

- requer JWT valido
- qualquer usuario autenticado pode doar

Payload:

```json
{
  "idCampanha": "550e8400-e29b-41d4-a716-446655440000",
  "valorDoado": 150.00
}
```

Comportamento esperado:

- retorna erro se a campanha nao existir
- retorna erro se a campanha nao estiver ativa
- retorna `202 Accepted` quando a doacao for recebida para processamento assincrono

## Configuracao

### Banco de dados local

O projeto esta configurado para desenvolvimento local com SQL Server em Docker.

Connection string usada atualmente:

```text
Server=localhost,1433;Database=db-onges-donate-dev;User Id=sa;Password=SenhaForte123!;TrustServerCertificate=True;
```

### JWT

A API espera os seguintes itens de configuracao:

- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`

### Integracao com campanhas

A API e o consumer utilizam:

- `CampaignApi:BaseUrl`

Essa configuracao e usada para consultar se a campanha existe e se esta ativa.

### Service Bus

Configuracoes esperadas:

- `ServiceBus:ConnectionString`
- `ServiceBus:DonationsTopic`
- `ServiceBus:DonationsSubscription`
- `ServiceBus:CampaignUpdatesEntity`

Valores padrao definidos no projeto:

- topico interno: `donates-topic`
- subscription: `donate-api-sub`
- destino da mensagem de atualizacao de campanha: `campaign-donations`

## Executando Localmente

### 1. Restaurar dependencias

```powershell
dotnet restore .\ONGES.Donate.slnx
```

### 2. Executar a API

```powershell
dotnet run --project .\ONGES.Donate.Api\ONGES.Donate.Api.csproj
```

A API foi configurada para abrir o Swagger automaticamente em ambiente de desenvolvimento.

### 3. Executar o consumer

```powershell
dotnet run --project .\ONGES.Donate.Consumer\ONGES.Donate.Consumer.csproj
```

## Executando com Docker Compose

O repositorio possui um `docker-compose.yml` para subir:

- `ONGES.Donate.Api`
- `ONGES.Donate.Consumer`
- `SQL Server`

Comando:

```powershell
docker compose up --build
```

API exposta em:

```text
http://localhost:8080
```

## Dockerfiles

Arquivos disponiveis:

- [Dockerfile](ONGES.Donate.Api/Dockerfile)
- [Dockerfile](ONGES.Donate.Consumer/Dockerfile)

Compose:

- [docker-compose.yml]

## Testes

Para executar os testes:

```powershell
dotnet test .\ONGES.Donate.Test\ONGES.Donate.Test.csproj
```

## Observacoes Importantes

- o projeto foi desenhado para manter `services` na camada `Application`
- a `Infrastructure` concentra persistencia, mensageria e integracoes tecnicas
- a doacao nao e persistida diretamente pela API
- a persistencia final acontece no `Consumer`

## Proximos Passos Recomendados

- criar migrations do `DonateDbContext`
- definir contrato final do destino `campaign-donations` com o servico `ONGES`
- adicionar tratamento mais robusto para falhas e retry de mensagens
- adicionar testes de integracao da API e do consumer
