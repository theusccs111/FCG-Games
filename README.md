# FCG-Games

## 📋 Introdução

**FCG-Games** é um microserviço responsável pela gestão do catálogo de jogos da plataforma FCG. Este serviço permite criar, atualizar, listar e pesquisar jogos, servindo como hub central de informações de games para toda a plataforma.

## 🎯 Objetivos

- Fornecer um catálogo centralizado de jogos
- Gerenciar metadados de jogos (título, descrição, imagens, ratings, etc.)
- Integrar-se com outros microserviços (Bibliotecas, Usuários)
- Oferecer busca e filtragem avançadas de jogos
- Processar eventos de criação e atualização de jogos via mensageria

## 🏗️ Arquitetura

### Padrão Clean Architecture

O projeto segue a arquitetura em camadas com separação clara de responsabilidades:

```
FCG-Games/
├── FCG-Games.Api/              # Camada de Apresentação (Controllers, Endpoints)
├── FCG-Games.Application/       # Camada de Aplicação (UseCases, DTOs, Services)
├── FCG-Games.Domain/            # Camada de Domínio (Entidades, Interfaces)
├── FCG-Games.Infrastructure/    # Camada de Infraestrutura (BD, Externos)
└── FCG-Games.Consumer/          # Processador de Mensagens (Worker Service)
```

### Fluxo de Dados

```
Cliente HTTP
    ↓
Controllers (FCG-Games.Api)
    ↓
Application Services (Lógica)
    ↓
Domain/Repository Pattern (Dados)
    ↓
MongoDB + Azure Service Bus
```

## 🔧 Stack Tecnológico

- **Framework**: ASP.NET Core 8.0
- **Autenticação**: JWT Bearer
- **Banco de Dados**: MongoDB 5.0+
- **Persistência**: Entity Framework Core
- **Mensageria**: Azure Service Bus
- **API Documentation**: Swagger/OpenAPI
- **Docker**: Containerização
- **CI/CD**: Azure Pipelines

## 📨 Microserviços e Mensageria

### Integração com Outros Serviços

**FCG-Games** se comunica com:
- **FCG-Libraries**: Via HTTP REST (consulta dados de bibliotecas do usuário)
- **FCG-Users**: Via HTTP REST (valida autenticação JWT)

### Azure Service Bus - Mensageria Assíncrona

O projeto usa **Azure Service Bus** para comunicação assíncrona baseada em eventos:

#### Consumer Service (FCG-Games.Consumer)
- **Tipo**: Worker Service (Host Service)
- **Responsabilidade**: Processa mensagens da fila
- **Padrão**: Listen & Process
- **Eventos Consumidos**: 
  - Notificações de novos usuários
  - Eventos de pagamentos completados
  - Alterações de bibliotecas

#### Publisher Service
- **Localização**: `FCG.Shared.EventService.Publisher`
- **Função**: Publica eventos para outros microserviços
- **Eventos Publicados**:
  - `GameCreatedEvent`: Quando um novo jogo é criado
  - `GameUpdatedEvent`: Quando um jogo é atualizado
  - `GameDeletedEvent`: Quando um jogo é deletado

### Fluxo de Mensageria

```
FCG-Games.Api
    ↓
[Cria/Atualiza Jogo]
    ↓
EventService.Publisher
    ↓
Azure Service Bus Topic
    ↓
FCG-Libraries.Consumer (recebe notificação)
FCG-Users.Consumer (opcional)
```

## 📁 Estrutura do Projeto

### FCG-Games.Api
- **Program.cs**: Configuração do host e injeção de dependências
- **Controllers/**: Endpoints HTTP
  - `GameController.cs`: CRUD de jogos
- **ApimAuthenticationHandler.cs**: Middleware de autenticação JWT

### FCG-Games.Application
- **Services/**: Lógica de negócios
- **DTOs/**: Data Transfer Objects
- **Validators/**: Validação de dados
- **Interfaces/**: Contratos de serviços
- **Shared/**: Helpers e utilitários compartilhados

### FCG-Games.Domain
- **Entities/**: Modelos de domínio
- **Interfaces/**: Contratos de repositório
- **SearchDocuments/**: Documentos para busca no MongoDB
- **Enums/**: Enumerações

### FCG-Games.Infrastructure
- **Context/**: DbContext do Entity Framework
- **Repositories/**: Implementação de acesso a dados
- **Services/**: Serviços de infraestrutura
- **External/**: Integração com APIs externas

### FCG-Games.Consumer
- **Program.cs**: Configuração do Worker Service
- **Worker.cs**: Lógica principal do processador
- **DependencyInjection.cs**: Setup de DI

## 🚀 Como Executar

### Pré-requisitos
- .NET 8.0 SDK
- MongoDB rodando (local ou cloud)
- Azure Service Bus configurado
- Docker (opcional)

### Desenvolvimento Local

1. **Clonar o repositório**
   ```bash
   git clone https://github.com/theusccs111/FCG-Games.git
   cd FCG-Games
   ```

2. **Configurar appsettings.json**
   ```json
   {
     "ConnectionStrings": {
       "MongoDB": "mongodb://localhost:27017/fcg-games"
     },
     "Services": {
       "LibraryApi": "http://localhost:5001"
     },
     "AzureServiceBus": {
       "ConnectionString": "your-service-bus-connection-string"
     }
   }
   ```

3. **Restaurar dependências e executar**
   ```bash
   dotnet restore
   dotnet run --project FCG-Games.Api
   ```

4. **Executar Consumer**
   ```bash
   dotnet run --project FCG-Games.Consumer
   ```

### Docker

```bash
docker-compose up --build
```

## 🔐 Autenticação

- **Tipo**: JWT Bearer Token
- **Issuer**: Serviço FCG-Users
- **Validação**: ApimAuthenticationHandler
- **Escopo**: Validação por claims de usuário

## 📚 Documentação de API

Acesse o Swagger em: `http://localhost/swagger/index.html`
