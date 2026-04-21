# ONGES.Donate

Microserviço responsável pelo recebimento e processamento de doações da plataforma ONGES.

O projeto foi estruturado no mesmo padrão dos repositórios `ONGES` e `ONGES.Users.Api`, com separação por camadas e um `Consumer` dedicado ao processamento assíncrono.

## Visão Geral

Fluxo principal:

1. um usuário autenticado envia uma doação para a API
2. a API valida os dados recebidos
3. a API verifica se a campanha existe
4. a API verifica se a campanha está ativa
5. a API publica uma mensagem interna para processamento
6. o `Consumer` consome essa mensagem
7. o `Consumer` persiste a doação no banco
8. o `Consumer` publica uma mensagem para o `ONGES (Campaign)` atualizar o valor arrecadado

Mensagem publicada para o `ONGES (Campaign)`:

```json
{
  "CampaignId": "550e8400-e29b-41d4-a716-446655440000",
  "Amount": 500.00,
  "DonatedAt": "2026-04-03T10:30:00Z"
}
```

## Estrutura da Solução

O repositório contém os seguintes projetos:

- `ONGES.Donate.Api`
- `ONGES.Donate.Application`
- `ONGES.Donate.Domain`
- `ONGES.Donate.Infrastructure`
- `ONGES.Donate.Consumer`
- `ONGES.Donate.Test`

Responsabilidade de cada camada:

- `Api`: endpoint HTTP, autenticação JWT e exposição da API
- `Application`: DTOs, interfaces e implementação de serviços/casos de uso
- `Domain`: entidades, enums, regras de domínio e resultados
- `Infrastructure`: persistência, repositórios, mensageria e integrações técnicas
- `Consumer`: processamento assíncrono das mensagens internas
- `Test`: testes unitários da solução

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

Autenticação:

- requer JWT válido
- qualquer usuário autenticado pode doar

Payload:

```json
{
  "idCampanha": "550e8400-e29b-41d4-a716-446655440000",
  "valorDoado": 150.00
}
```

Comportamento esperado:

- retorna erro se a campanha não existir
- retorna erro se a campanha não estiver ativa
- retorna `202 Accepted` quando a doação for recebida para processamento assíncrono

## Configuração

### Banco de dados local

O projeto está configurado para desenvolvimento local com SQL Server em Docker.

Connection string usada atualmente:

```text
Server=localhost,1433;Database=db-onges-donate-dev;User Id=sa;Password=SenhaForte123!;TrustServerCertificate=True;
```

### JWT

A API espera os seguintes itens de configuração:

- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`

### Integração com campanhas

A API e o consumer utilizam:

- `CampaignApi:BaseUrl`

Essa configuração é usada para consultar se a campanha existe e se está ativa.

### Service Bus

Configurações esperadas:

- `ServiceBus:ConnectionString`
- `ServiceBus:DonationsTopic`
- `ServiceBus:DonationsSubscription`
- `ServiceBus:CampaignUpdatesEntity`

Valores padrão definidos no projeto:

- tópico interno: `donates-topic`
- subscription: `donate-api-sub`
- destino da mensagem de atualização de campanha: `campaign-donations`

## Executando Localmente

### 1. Restaurar dependências

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

O repositório possui um `docker-compose.yml` para subir:

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

Arquivos disponíveis:

- [Dockerfile](ONGES.Donate.Api/Dockerfile)
- [Dockerfile](ONGES.Donate.Consumer/Dockerfile)

Compose:

- [docker-compose.yml]

## Testes

Para executar os testes:

```powershell
dotnet test .\ONGES.Donate.Test\ONGES.Donate.Test.csproj
```

## Observações Importantes

- o projeto foi desenhado para manter `services` na camada `Application`
- a `Infrastructure` concentra persistência, mensageria e integrações técnicas
- a doação não é persistida diretamente pela API
- a persistência final acontece no `Consumer`

## Próximos Passos Recomendados

- criar migrations do `DonateDbContext`
- definir contrato final do destino `campaign-donations` com o serviço `ONGES`
- adicionar tratamento mais robusto para falhas e retry de mensagens
- adicionar testes de integração da API e do consumer
