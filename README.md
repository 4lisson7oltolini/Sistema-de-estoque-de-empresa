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

### Deploy em VPS com Docker Compose

O deploy de referência usa uma VPS Linux com Docker Engine e Docker Compose, um domínio apontado para o IP público e as portas TCP 80 e 443 liberadas. O Caddy termina HTTPS automaticamente; somente o site fica exposto à internet. A API permanece na rede interna do Compose, e o SQLite e as chaves de proteção do Blazor ficam em volumes persistentes.

Clone o repositório na VPS e crie o arquivo local de ambiente:

```bash
cp .env.example .env
openssl rand -base64 48
openssl rand -base64 48
```

Edite `.env`: informe seu domínio em `DOMAIN` e cole os dois resultados, sem reutilizá-los, em `JWT_SIGNING_KEY` e `BOOTSTRAP_SETUP_KEY`, respectivamente. Não reutilize as chaves de desenvolvimento nem compartilhe o arquivo `.env`.

Com o registro DNS apontando para a VPS, inicie os serviços:

```bash
docker compose up --build -d
docker compose ps
docker compose logs -f api web caddy
```

O serviço `migrate` aplica as migrations pendentes no volume SQLite antes de iniciar a API. Acesse `https://<seu-dominio>/primeiro-acesso` para criar o administrador. Depois do primeiro acesso, remova o valor de `BOOTSTRAP_SETUP_KEY` de `.env` e recrie a API:

```bash
docker compose up -d --force-recreate api
```

Faça backup regular do volume `pdv_data` e mantenha os volumes `web_keys`, `caddy_data` e `caddy_config` ao atualizar os containers. **Não use `docker compose down -v`** para atualizar: a opção `-v` apaga os volumes, incluindo o banco. Se a rede `172.30.0.0/24` já estiver ocupada na VPS, altere a sub-rede e o IP fixo do Caddy em `compose.yaml` e atualize `ReverseProxy__KnownProxy` para o mesmo IP.

### Deploy da API no Render

O erro `Configure uma chave JWT com pelo menos 32 bytes` ocorre quando a API inicia sem a configuração da chave. No serviço da API, abra **Environment** no painel do Render e crie estas variáveis:

| Variável | Valor |
| --- | --- |
| `Jwt__SigningKey` | Uma chave aleatória própria para produção, com pelo menos 32 bytes |
| `Bootstrap__SetupKey` | Outra chave aleatória, diferente da chave JWT, para criar o primeiro administrador |

Gere cada valor separadamente, por exemplo no PowerShell, e guarde-os em um cofre de senhas:

```powershell
[Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(64))
```

Não use o mesmo valor para as duas variáveis, não reutilize uma chave de desenvolvimento e não coloque os valores no repositório. O nome `JWT_SIGNING_KEY` é usado pelo `compose.yaml` para preencher `Jwt__SigningKey` no Docker Compose; no Render, configure diretamente `Jwt__SigningKey`, como na tabela.

Antes de implantar, configure `ConnectionStrings__DefaultConnection` no Render. Sem essa variável, `appsettings.json` usa `Data Source=pdvsistema.db`, que no container do Render fica relativo ao diretório `/app` e não é persistente. O repositório não contém uma configuração de disco do Render. Para persistir o SQLite, crie/associe um **Persistent Disk** ao serviço, escolha `/var/data` como mount path e configure:

```text
ConnectionStrings__DefaultConnection=Data Source=/var/data/pdvsistema.db
```

Faça backup do banco existente e aponte para o caminho correto antes do deploy; não presuma que o banco atualmente aberto pelo serviço seja o mesmo arquivo de um deploy anterior. A API aplica as migrations pendentes no startup usando a mesma configuração de conexão que atende as requisições. Se a migração falhar, o processo não inicia e o erro é registrado. O deploy deve usar uma única instância da API ao compartilhar um arquivo SQLite; para múltiplas instâncias, use um processo de migração único e controlado e uma estratégia de banco compatível com esse cenário.

Para conferir no shell do serviço que a tabela e o histórico de migrations estão no arquivo persistente configurado (requer o utilitário `sqlite3`):

```bash
sqlite3 /var/data/pdvsistema.db "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'Usuarios';"
sqlite3 /var/data/pdvsistema.db "SELECT MigrationId FROM __EFMigrationsHistory;"
```

Salve as variáveis e faça um novo deploy. Depois que o primeiro administrador for criado em `/primeiro-acesso`, remova `Bootstrap__SetupKey` do Render para desativar essa configuração inicial. Mantenha `Jwt__SigningKey` estável entre deploys; trocá-la invalida os tokens já emitidos.

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

No desenvolvimento local, o banco SQLite é criado em `PdvSistema.API/pdvsistema.db` e não deve ser versionado. No deploy Docker, ele fica no volume persistente `pdv_data`.

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
