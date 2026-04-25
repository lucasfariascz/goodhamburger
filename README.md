# GoodHamburger

Sistema de gerenciamento de pedidos de hamburgueria, construído com ASP.NET Core 9 e Blazor Server. A aplicação permite criar, visualizar e excluir pedidos com cálculo automático de descontos conforme a combinação de itens escolhida.

---

## Stack

- **Backend:** ASP.NET Core 9 (Web API)
- **Frontend:** Blazor Server (.NET 9)
- **Banco de dados:** SQLite com Entity Framework Core 9
- **Testes:** xUnit + Moq + WebApplicationFactory
- **Deploy:** Azure App Service via GitHub Actions

---

## Arquitetura

O projeto segue Clean Architecture, separado em camadas:

```
GoodHamburger/
├── src/
│   ├── GoodHamburger.Domain/          # Entidades, regras de negócio, exceções, interfaces
│   ├── GoodHamburger.Application/     # Casos de uso, DTOs, serviços
│   ├── GoodHamburger.Infrastructure/  # EF Core, repositórios, migrations
│   ├── GoodHamburger.Api/             # Controllers, middleware, Program.cs
│   └── GoodHamburger.Web/             # Blazor Server, páginas, cliente HTTP
└── tests/
    ├── GoodHamburger.UnitTests/
    └── GoodHamburger.IntegrationTests/
```

### Decisões de arquitetura

**Clean Architecture:** A dependência entre camadas flui para dentro — Domain não conhece nada além de si mesmo, Application depende só de Domain, Infrastructure implementa as interfaces definidas em Domain. Isso mantém a lógica de negócio isolada e testável sem tocar banco de dados.

**Domínio rico:** As regras de negócio (validação de combinação de itens, cálculo de desconto) vivem nas entidades de domínio e lançam exceções próprias (`DomainException`, `DuplicateItemException`, `NotFoundException`). O middleware de exceções captura essas exceções e retorna respostas HTTP adequadas com Problem Details (RFC 7807).

**Cardápio estático:** Os itens do menu são hardcoded no domínio via `StaticMenuCatalog`. Não há tabela de produtos no banco — o cardápio é fixo e as regras de desconto dependem das categorias dos itens, não de configuração externa. Isso simplificou bastante o modelo sem perder a expressividade do domínio.

**SQLite:** Escolhido pela simplicidade de setup — sem instalar banco externo para rodar localmente. O arquivo `.db` é criado automaticamente na primeira execução. Para produção no Azure, o path é configurado via variável de ambiente.

**Blazor Server:** O frontend consome a API via `HttpClient` (classe `ApiClient`). Não há compartilhamento de código entre backend e frontend além dos DTOs, que são replicados em `Models/ApiModels.cs` no projeto Web.

---

## Regras de negócio

### Itens do cardápio

| Item | Preço |
|------|-------|
| X-Burguer | R$ 5,00 |
| X-Egg | R$ 4,50 |
| X-Bacon | R$ 7,00 |
| Fritas | R$ 2,00 |
| Refrigerante | R$ 2,50 |

### Restrições por pedido

- Máximo de **1 sanduíche** por pedido (X-Burguer, X-Egg ou X-Bacon são mutuamente exclusivos)
- Máximo de **1 porção de fritas**
- Máximo de **1 refrigerante**

### Descontos automáticos

| Combinação | Desconto |
|------------|----------|
| Sanduíche + Fritas + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Fritas | 10% |
| Outros | Sem desconto |

---

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

---

## Rodando localmente

### 1. Clone o repositório

```bash
git clone https://github.com/lucasfariascz/GoodHamburger.git
cd GoodHamburger
```

### 2. Restaure as dependências

```bash
dotnet restore
```

### 3. Rode as migrations e suba a API

As migrations são aplicadas automaticamente na inicialização da API. Para subir:

```bash
dotnet run --project src/GoodHamburger.Api
```

A API estará disponível em `http://localhost:5293`. O arquivo `goodhamburguer.db` será criado automaticamente na pasta do projeto API na primeira execução.

O Swagger estará acessível em `http://localhost:5293/swagger`.

### 4. Suba o frontend

Em outro terminal:

```bash
dotnet run --project src/GoodHamburger.Web
```

O frontend estará disponível em `http://localhost:5000`.

> A URL da API consumida pelo frontend é configurada em `src/GoodHamburger.Web/appsettings.json` via `ApiBaseUrl`.

---

## Rodando as migrations manualmente (opcional)

As migrations já são aplicadas automaticamente no startup da API. Mas caso queira aplicar manualmente:

```bash
dotnet ef database update --project src/GoodHamburger.Infrastructure --startup-project src/GoodHamburger.Api
```

Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project src/GoodHamburger.Infrastructure --startup-project src/GoodHamburger.Api
```

---

## Testes

```bash
# Todos os testes
dotnet test

# Apenas unitários
dotnet test tests/GoodHamburger.UnitTests

# Apenas integração
dotnet test tests/GoodHamburger.IntegrationTests
```

Os testes de integração usam `WebApplicationFactory` com SQLite em memória — não precisam da API rodando.

---

## API

### Pedidos

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/orders` | Lista todos os pedidos |
| `GET` | `/api/orders/{id}` | Retorna um pedido pelo ID |
| `POST` | `/api/orders` | Cria um novo pedido |
| `PUT` | `/api/orders/{id}` | Atualiza um pedido existente |
| `DELETE` | `/api/orders/{id}` | Remove um pedido |

### Cardápio

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/menuItem` | Lista todos os itens do cardápio com preços |

Erros retornam no formato Problem Details com `traceId` para rastreamento.

---

## Deploy

O deploy é feito automaticamente via GitHub Actions ao mergear para `main`. O workflow:

1. Builda e executa todos os testes
2. Publica e faz deploy da API para o Azure App Service `goodhamburger-api-prod`
3. Publica e faz deploy do frontend para `goodhamburger-web-prod`

Os secrets necessários no repositório são `AZURE_WEBAPP_PUBLISH_PROFILE_API` e `AZURE_WEBAPP_PUBLISH_PROFILE_WEB`.

---

## O que foi deixado de fora

O projeto foi construído como um exercício focado em clean architecture e regras de negócio. Diversas funcionalidades foram intencionalmente omitidas para manter o escopo controlado:

- **Gestão de clientes** — pedidos não estão vinculados a nenhum usuário/cliente
- **Status de pedido** — não há fluxo de estados (em preparo, entregue, cancelado)
- **Filtragem e paginação** — a listagem de pedidos retorna tudo sem filtros
- **Docker** — sem Dockerfile ou docker-compose
- **Logging estruturado** — sem Serilog ou similar, apenas o logging padrão do .NET
- **Cache** — sem cache de respostas

## Ambientes publicados

A aplicação foi publicada no Azure App Service com dois serviços separados: um para a API e outro para o frontend Blazor Server.

| Serviço | URL |
|---|---|
| Frontend Web | https://goodhamburger-web-prod.azurewebsites.net/ |
| API | https://goodhamburger-api-prod.azurewebsites.net/ |
| Swagger | https://goodhamburger-api-prod.azurewebsites.net/swagger/ |
