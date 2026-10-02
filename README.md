# Sistema de Estoque e PDV

Sistema de gerenciamento de estoque e vendas desenvolvido em C#/.NET.

## Tecnologias

- C#
- .NET 10
- ASP.NET Core
- Entity Framework Core
- SQLite
- xUnit
- REST API

## Arquitetura

- `PdvSistema.Domain`: entidades e interfaces de repositório.
- `PdvSistema.Application`: serviços e DTOs.
- `PdvSistema.Infrastructure`: Entity Framework Core, SQLite e migrations.
- `PdvSistema.API`: endpoints REST e configuração da aplicação.
- `PdvSistema.Web`: interface Blazor para painel, produtos e categorias.
- `PdvSistema.Tests`: testes automatizados.

## Funcionalidades

- API REST de cadastro, consulta, atualização e remoção de produtos.
- API de categorias com proteção contra remoção com produtos associados.
- API de movimentação de estoque com validação de saldo e histórico.
- API de vendas com cálculo de total, itemização e cancelamento com devolução de estoque.
- API de clientes e usuários.
- Interface visual responsiva para indicadores, produtos e categorias.

## Como executar

Pré-requisitos: .NET SDK 10 e `dotnet-ef` 10.0.12. Instale a ferramenta global se ainda não estiver disponível:

```bash
dotnet tool install --global dotnet-ef --version 10.0.12
```

Na raiz do repositório, aplique as migrations e inicie a API no primeiro terminal:

```bash
dotnet ef database update --project PdvSistema.Infrastructure/PdvSistema.Infrastructure.csproj --startup-project PdvSistema.API/PdvSistema.API.csproj
dotnet run --project PdvSistema.API/PdvSistema.API.csproj
```

Em outro terminal, inicie a interface Blazor:

```bash
dotnet run --project PdvSistema.Web/PdvSistema.Web.csproj
```

A interface abre em `http://localhost:5209` e consome a API em `http://localhost:5177`. Mantenha os dois processos ativos durante o uso. O endereço da API pode ser ajustado em `PdvSistema.Web/appsettings.json` (`ApiBaseUrl`).

O banco SQLite local é criado em `PdvSistema.API/pdvsistema.db` e não deve ser versionado. A interface atual cobre o painel, produtos e categorias; vendas, clientes, usuários e movimentações ainda não têm telas próprias.

Para criar migrations após mudanças no modelo:

```bash
dotnet ef migrations add NomeDaMigration --project PdvSistema.Infrastructure/PdvSistema.Infrastructure.csproj --startup-project PdvSistema.API/PdvSistema.API.csproj --output-dir Data/Migrations
```

## Testes

Execute os testes automatizados com:

```bash
dotnet test PdvSistema.Tests/PdvSistema.Tests.csproj
```

## Estrutura do projeto

- `PdvSistema.slnx`: solução.
- `PdvSistema.API/`: aplicação ASP.NET Core e endpoints.
- `PdvSistema.Web/`: aplicação Blazor Web App.
- `PdvSistema.Application/`: casos de uso e contratos HTTP.
- `PdvSistema.Domain/`: modelo de domínio.
- `PdvSistema.Infrastructure/`: persistência e migrations do EF Core.
- `PdvSistema.Tests/`: testes xUnit.
