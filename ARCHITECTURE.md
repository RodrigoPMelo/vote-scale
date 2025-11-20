# Diagramas de Arquitetura do Projeto (definições em andamento, não é versão final)  

``` mermaid
  graph TD
      %% Estilos para diferenciar as camadas
      classDef entry fill:#e1f5fe,stroke:#01579b,stroke-width:2px;
      classDef infra fill:#fff9c4,stroke:#fbc02d,stroke-width:2px;
      classDef core fill:#e8f5e9,stroke:#2e7d32,stroke-width:2px,color:black;
  
      subgraph EntryPoints [Entry Points]
          direction TB
          Web[<b>VoteScale.Web</b><br/>Blazor & API<br/><i>Depende de Infra e Domain</i>]:::entry
          Worker[<b>VoteScale.Worker</b><br/>Background Service<br/><i>Depende de Infra e Domain</i>]:::entry
      end
  
      subgraph Infrastructure [Camada de Infraestrutura]
          Infra[<b>VoteScale.Infrastructure</b><br/>Implementação do EF Core<br/>Implementação do RabbitMQ<br/>Migrations]:::infra
      end
  
      subgraph Domain [Camada de Domínio Core]
          Core[<b>VoteScale.Domain</b><br/>Entidades Vote, Survey<br/>Interfaces IVoteRepository<br/>Interfaces IMessageBus]:::core
      end
  
      %% Fluxo de Dependências (Quem referencia quem)
      Web -->|Usa Classes Concretas via DI| Infra
      Worker -->|Usa Classes Concretas via DI| Infra
      
      Web -->|Usa Modelos e Interfaces| Core
      Worker -->|Usa Modelos| Core
      
      Infra -->|Implementa Interfaces| Core
  
      %% Nota explicativa
      linkStyle 0,1,2,3,4 stroke-width:2px,fill:none,stroke:gray;
```

``` mermaid
sequenceDiagram
    autonumber
    actor User as Eleitor
    participant Browser as Blazor UI
    participant API as Vote API (.NET)
    participant Bus as RabbitMQ
    participant Worker as Background Service
    participant DB as PostgreSQL

    Note over User, API: Fluxo Síncrono (Rápido)
    User->>Browser: Clica em "Confirmar Voto"
    Browser->>API: POST /api/votes (JSON)
    
    activate API
    API->>API: Valida dados básicos (Schema)
    API->>Bus: Publica Mensagem (Fire & Forget)
    API-->>Browser: 202 Accepted ("Voto Recebido")
    deactivate API
    
    Browser-->>User: Exibe "Sucesso!"
    
    Note over Bus, DB: Fluxo Assíncrono (Processamento)
    loop Processamento em Background
        Bus->>Worker: Entrega mensagem (Push)
        activate Worker
        Worker->>Worker: Deserializa JSON
        Worker->>DB: INSERT INTO Votes (Transação)
        DB-->>Worker: Commit OK
        Worker-->>Bus: Ack (Confirmação de Leitura)
        deactivate Worker
    end
```  


``` mermaid
graph TD
    subgraph DockerHost [Servidor Docker Host]
        style DockerHost fill:#f9f9f9,stroke:#333,stroke-width:2px
        
        subgraph C_Web [Container: VoteScale.Web]
            WebApp[VoteScale.Web.dll]
        end

        subgraph C_MQ [Container: RabbitMQ]
            RabbitMQ(Message Broker)
        end

        subgraph C_Worker [Container: VoteScale.Worker]
            WorkerApp[VoteScale.Worker.dll]
        end

        subgraph C_DB [Container: PostgreSQL]
            DB[(Banco de Dados)]
        end
    end

    %% Conexões de Rede
    WebApp -- "Publica Evento (AMQP)" --> RabbitMQ
    RabbitMQ -- "Consome Evento (AMQP)" --> WorkerApp
    WorkerApp -- "INSERT (TCP/5432)" --> DB
    WebApp -. "SELECT (TCP/5432)" .-> DB
    
    %% Acesso Externo
    Usuario((Usuário)) -- "HTTPS / JSON" --> WebApp
```

