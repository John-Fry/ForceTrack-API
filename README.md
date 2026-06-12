# Avaliacao Backend

Projeto ASP.NET Core criado para responder ao teste tecnico de backend.

## Stack

- ASP.NET Core Web API
- .NET 10
- MongoDB
- Docker Compose
- HttpClientFactory
- Options Pattern
- Controllers + Services + Interfaces
- OpenAPI em `/openapi/v1.json`

## Como rodar

### Com Docker

```bash
docker compose up --build
```

API:

```txt
http://localhost:8080
```

OpenAPI:

```txt
http://localhost:8080/openapi/v1.json
```

### Localmente

```bash
dotnet restore
dotnet run --project src/AvaliacaoBackend.Api/AvaliacaoBackend.Api.csproj
```

Para rodar localmente com MongoDB:

```bash
docker compose up -d mongo
```

## Endpoints principais

### Questao 1 - Force1

```http
GET /ativos/computadores-sem-comunicacao?dias=60
```

Consome a API do Force1 e retorna computadores com comunicacao mais antiga que o numero de dias informado.

### Questao 2 - Produtos

```http
GET /produtos
POST /produtos
```

Exemplo de POST:

```json
{
  "nome": "Teclado",
  "preco": 250.00
}
```

### Questao 3 - Clientes com MongoDB

```http
GET /clientes
POST /clientes
```

Exemplo de POST:

```json
{
  "nome": "Maria Silva",
  "email": "maria@email.com"
}
```

### Questao 4 - Correcao do codigo

A correcao foi aplicada de forma pratica no `Force1Service`, em vez de manter um metodo isolado chamado `PegaAtivos`.

O codigo original tinha os seguintes problemas:

- criava `HttpClient` diretamente dentro do metodo;
- nao aguardava a leitura do conteudo da resposta;
- usava `ReadAsString()` em vez de `ReadAsStringAsync()`;
- tentava desserializar uma `Task<string>` em vez de uma string;
- nao validava se a API externa retornou sucesso;
- recebia o parametro `cidade`, mas nao o utilizava;
- nao tratava o fluxo real de autenticacao da API Force1.

No projeto, a versao corrigida usa:

- `HttpClient` injetado via `IHttpClientFactory`
- autenticacao previa em `POST /v2/Auth/Login`
- token Bearer na chamada `GET /v2/Force1/GetAssets`
- `await SendAsync(...)`
- `await ReadAsStringAsync(...)`
- tratamento de erros externos com `Force1ApiException`
- parsing do JSON somente depois da resposta ser lida corretamente
- metodo assincrono ponta a ponta

O fluxo corrigido esta nos arquivos:

```txt
src/AvaliacaoBackend.Api/Services/Force1/Force1Service.cs
src/AvaliacaoBackend.Api/Services/Force1/Force1ApiException.cs
src/AvaliacaoBackend.Api/Controllers/AtivosController.cs
```

### Questao 5 - Integracoes

Google Maps:

```http
GET /integracoes/google-maps/geocode?endereco=Av Paulista, Sao Paulo
GET /integracoes/google-maps/reverse-geocode?latitude=-23.561684&longitude=-46.655981
GET /integracoes/google-maps/validar-endereco?endereco=Av Paulista, Sao Paulo
```

DocuSign:

```http
POST /integracoes/docusign/envelopes
POST /integracoes/docusign/envelopes/{envelopeId}/send
GET /integracoes/docusign/envelopes/{envelopeId}
```

Microsoft Graph:

```http
GET /integracoes/graph/me
GET /integracoes/graph/messages?quantidade=10
POST /integracoes/graph/events
```

## Configuracao

As credenciais podem ser informadas por variaveis de ambiente:

```bash
Force1__Login=...
Force1__Senha=...
Force1__Enterprise=...
GoogleMaps__ApiKey=...
DocuSign__AccountId=...
DocuSign__AccessToken=...
MicrosoftGraph__AccessToken=...
```

O arquivo `.env.example` mostra os nomes esperados.
As credenciais recebidas no enunciado devem ser configuradas localmente, por variavel de ambiente, User Secrets ou `appsettings.Development.json`. Nao envie credenciais reais para repositorios publicos.

## Observacoes de implementacao

- A lista de produtos fica em memoria porque a questao pede apenas `GET` e `POST` simples.
- Clientes usam MongoDB Driver com interface `IClienteService`.
- As integracoes externas estao isoladas em services tipados para facilitar testes e troca futura por SDKs oficiais.
- O parser do Force1 foi feito de forma defensiva porque o enunciado nao inclui um exemplo concreto do JSON retornado.
