# 🎮 FCG Catalog API – Fase 2

API REST desenvolvida em **.NET 8** como parte do **Desafio da Fase 2** da disciplina **Arquitetura de Sistemas .NET – FIAP**.

O projeto representa o microsserviço de **Catálogo** da plataforma de games educacionais (**FCG – FIAP Game Center**). Nesta fase, o monolito foi decomposto, e este serviço passa a ter total autonomia sobre o gerenciamento de jogos e atua como ponto de partida para o fluxo de compras via mensageria.

---

## 📌 Objetivo do Projeto

Refatorar a aplicação para uma arquitetura de microsserviços orientada a eventos, garantindo:

- Autonomia de código e ciclo de vida (repositório isolado)
- Comunicação assíncrona utilizando Mensageria (**RabbitMQ**)
- Persistência de dados isolada
- Containerização com **Docker**
- Base escalável para orquestração em Kubernetes

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQLite**
- **MassTransit** (Abstração de Mensageria)
- **RabbitMQ** (Message Broker)
- **Docker**
- **JWT Bearer Authentication**
- **Swagger / OpenAPI**
- **ILogger** para logs estruturados

---

## 🧱 Arquitetura

O projeto segue uma separação clara de responsabilidades, inspirada em princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, promovendo manutenibilidade, clareza e escalabilidade.

### Camadas Principais

- **Api**  
  Controllers, middlewares, configuração da aplicação, autenticação e autorização.

- **Application**  
  Serviços de negócio, interfaces, validações, DTOs e publicação de eventos via MassTransit.

- **Domain**  
  Entidades (ex: Game), Value Objects e exceções de domínio.

- **Infrastructure**  
  Repositórios, acesso a dados, persistência utilizando Entity Framework Core e configuração do RabbitMQ.

---

## 📁 Estrutura de Pastas

```text
Fcg.Catalog
│
├── Fcg.Catalog.Api
│   ├── Controllers
│   ├── Middlewares
│   ├── Program.cs
│   └── appsettings.json
│
├── Fcg.Catalog.Application
│   ├── Services
│   ├── Interfaces
│   ├── DTOs
│   └── Messages
│
├── Fcg.Catalog.Domain
│   ├── Entities
│   ├── ValueObjects
│   └── Exceptions
│
└── Fcg.Catalog.Infrastructure
    ├── Persistence
    ├── Repositories
    └── Messaging

```

---

## 🔐 Segurança

* Autenticação via **JWT Bearer**
* Controle de acesso utilizando:
* `[Authorize]`
* `[Authorize(Roles = "Admin")]`


* Middleware global para tratamento de exceções
* Logs estruturados utilizando `ILogger`

---

## 🔑 Níveis de Acesso

### 👤 User

* Pode se autenticar na plataforma (via UserAPI)
* Pode consultar o catálogo de jogos disponíveis

### 🛡️ Admin

* Pode listar todos os jogos
* Pode cadastrar, atualizar e remover jogos do catálogo

---

## 🔐 Autenticação com JWT

Este microsserviço não gera tokens (responsabilidade da UserAPI), mas valida os tokens recebidos. O token JWT deve ser enviado nas requisições protegidas no header:

```http
Authorization: Bearer {token}

```

As roles são incluídas como claims no token e utilizadas pelo ASP.NET Core para controle de acesso aos endpoints.

---

## 📨 Mensageria e Eventos (RabbitMQ)

A comunicação com outros microsserviços ocorre de forma assíncrona.

* **Produtor:** Publica eventos de domínio (ex: `OrderPlacedEvent`) no RabbitMQ quando um fluxo de compra é iniciado.
* **Consumidor:** Escuta eventos (ex: `PaymentProcessedEvent`) para atualizar o status dos jogos/pedidos.

---

## 🔗 Endpoints Principais

### ✅ Catálogo de Jogos

* Listar jogos: `GET /games`
* Buscar jogo por ID: `GET /games/{id}`
* Criar jogo (Admin): `POST /games`
* Atualizar jogo (Admin): `PUT /games/{id}`
* Remover jogo (Admin): `DELETE /games/{id}`

---

## 📘 Documentação e Testes

### Swagger / OpenAPI

A API possui documentação automática gerada com Swagger, disponível em:
`/swagger`

Por meio do Swagger é possível:

* Visualizar todos os endpoints disponíveis
* Ver parâmetros e respostas esperadas
* Testar requisições diretamente pela interface
* Autenticar usando JWT pelo botão Authorize

---

## ⚠️ Tratamento de Erros

A API utiliza um middleware global de exceções, responsável por:

* Capturar exceções de domínio (ex: NotFoundException)
* Traduzir exceções para os códigos HTTP adequados (400, 404, 500)
* Retornar respostas JSON padronizadas
* Registrar logs estruturados

---

## 🪵 Logs Estruturados

Os logs são registrados com ILogger, permitindo:

* Logs de erro (LogError)
* Logs de aviso (LogWarning)
* Logs informativos (LogInformation)
* Identificação de requisições através de traceId

---

## 🗄️ Banco de Dados

* **SQLite**
* **Entity Framework Core**
* Criação e versionamento do banco via Migrations
* Arquivo do banco gerado automaticamente em ambiente de desenvolvimento

---

## ▶️ Como Executar o Projeto

### Pré-requisitos

* .NET SDK 8 ou superior
* Docker e Docker Compose (para o RabbitMQ)
* SQLite

### Variáveis de Ambiente (appsettings.json)

* `ConnectionStrings:DefaultConnection` (Data Source=catalog.db)
* `JwtSettings:SecretKey` (A mesma chave usada na UserAPI)
* `RabbitMQ:Host`, `RabbitMQ:Username`, `RabbitMQ:Password`

### Execução via Docker

```bash
docker-compose up -d

```

### Execução Local

Acesse:
https://localhost:5001/swagger

---

## ✅ Considerações Finais

Este projeto foi refatorado com foco em:

* Autonomia de microsserviços
* Comunicação assíncrona (RabbitMQ / MassTransit)
* Containerização (Docker)
* Escalabilidade para futuras fases do desafio

---

## 👥 Squad 8 – Turma 12NETT

**Integrantes**

* Yan Santos Wendt
* Ronnam de Lima da Silva

```

```
