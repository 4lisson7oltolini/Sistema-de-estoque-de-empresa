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
- `PdvSistema.Web`: interface Blazor para painel, estoque, vendas, clientes e relatórios.
- `PdvSistema.Tests`: testes automatizados.

## Funcionalidades

- API REST de cadastro, consulta, atualização e remoção de produtos.
- API de categorias com proteção contra remoção com produtos associados.
- API de movimentação de estoque com validação de saldo e histórico.
- API de vendas com cálculo de total, itemização e cancelamento com devolução de estoque.
- API de clientes e usuários.
- Saldo inicial registrado como entrada de estoque ao cadastrar um produto.
- Histórico de movimentações com filtros por produto, tipo e período; vendas e cancelamentos também geram movimentações.
- Interface de vendas com carrinho, cliente opcional, forma de pagamento, validação de disponibilidade e cancelamento.
- Cadastro de clientes com busca, criação, edição e remoção.
- Relatórios de faturamento, ticket médio, produtos mais vendidos e produtos no mínimo ou abaixo dele, com filtro de período para vendas.
- Interface visual responsiva para painel, produtos, categorias, movimentações, vendas, clientes e relatórios.
- Login com token curto, autorização por perfil e configuração inicial de administrador.

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

Após entrar no sistema, as telas ficam disponíveis no menu lateral:

- `/produtos`: catálogo, saldo inicial e movimentações de entrada/saída.
- `/movimentacoes`: histórico de estoque filtrável por produto, tipo e período.
- `/vendas`: caixa, carrinho e vendas recentes; cancelar uma venda devolve os itens ao estoque.
- `/clientes`: cadastro e busca de clientes.
- `/relatorios`: indicadores de vendas e estoque, com filtro de período.
- `/categorias`: organização dos produtos.

### Primeiro login

Antes de iniciar a API pela primeira vez, configure segredos locais. Não use uma chave fixa em produção:

```powershell
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$jwtBytes = [byte[]]::new(64)
$setupBytes = [byte[]]::new(48)
$rng.GetBytes($jwtBytes)
$rng.GetBytes($setupBytes)
$jwtKey = [Convert]::ToBase64String($jwtBytes)
$setupKey = [Convert]::ToBase64String($setupBytes)
dotnet user-secrets set "Jwt:SigningKey" $jwtKey --project .\PdvSistema.API\PdvSistema.API.csproj
dotnet user-secrets set "Bootstrap:SetupKey" $setupKey --project .\PdvSistema.API\PdvSistema.API.csproj
$rng.Dispose()
Remove-Variable jwtKey, setupKey, jwtBytes, setupBytes, rng
```

Abra `http://localhost:5209/primeiro-acesso`, informe a chave salva em `Bootstrap:SetupKey` e crie o administrador. Para copiá-la sem exibir os segredos no terminal, execute:

```powershell
$secrets = Get-Content "$env:APPDATA\Microsoft\UserSecrets\PdvSistema-API-Local-Secrets\secrets.json" -Raw | ConvertFrom-Json
$secrets.'Bootstrap:SetupKey' | Set-Clipboard
Remove-Variable secrets
```

Essa rota só funciona enquanto não houver nenhum usuário cadastrado; depois, entre em `http://localhost:5209/login`. Hashes SHA-256 de senhas antigas são atualizados para o formato adaptativo no próximo login bem-sucedido.

Em produção, configure `Jwt__SigningKey` e `Bootstrap__SetupKey` no cofre de segredos do ambiente, publique somente sob HTTPS e remova/desative a chave de bootstrap após criar o administrador.

### Proteções incluídas e limites

- O acesso à API exige JWT por padrão; usuários só podem ser administrados por perfil Administrador.
- Login e bootstrap são limitados a 5 tentativas por minuto/IP; a API também limita tráfego geral e concorrência, tamanho do corpo e tempo de headers.
- O acesso a dados usa LINQ/EF Core parametrizado; não foram encontradas consultas SQL raw montadas por concatenação.
- Senhas usam `PasswordHasher` e não são devolvidas pela API. O Blazor guarda a sessão em armazenamento protegido de sessão, não em `localStorage`.
- Rate limit no processo não impede DDoS volumétrico distribuído. Para produção pública, use WAF/CDN, firewall, proxy confiável, monitoramento e limites no host/banco.

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
