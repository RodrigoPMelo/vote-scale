# Diagramas de Arquitetura do Projeto (definições em andamento, não é versão final)  

### Arquitetura em Camadas e Dependências: Este diagrama ilustra a aplicação estrita do princípio de inversão de dependência. A camada Web (Interface) e o Worker (Serviço) dependem apenas das abstrações definidas no Domain e das implementações concretas da Infrastructure. O Domain permanece puro, sem conhecimento de banco de dados ou frameworks externos, facilitando a testabilidade e a manutenção.  

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
### Fluxo de Votação Assíncrono (Event-Driven): Demonstração do padrão Fire-and-Forget. Para garantir alta disponibilidade sob tráfego massivo, a API não grava no banco diretamente. Ela delega a responsabilidade para o Message Broker (RabbitMQ) e responde imediatamente ao eleitor. O processamento pesado (persixtência SQL) ocorre em segundo plano, desacoplado da experiência do usuário.  

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
### Topologia de Implantação Containerizada: Visão física da infraestrutura isolada via Docker. Cada responsabilidade (Web, Worker, Banco e Mensageria) reside em seu próprio container, garantindo isolamento de recursos, facilidade de escalabilidade horizontal (podemos subir mais réplicas do Worker se necessário) e paridade entre os ambientes de desenvolvimento e produção.  

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
### Estratégia de Testes Automatizados (Visualização)  

O diagrama abaixo ilustra como garantimos a qualidade do componente de dados (EF Core) isolando o ambiente via Testcontainers, sem sujar o banco de produção.  

```mermaid
graph LR
    subgraph TestEnvironment [Ambiente de Testes Automatizados]
        TestRunner[xUnit Test Runner]
        
        subgraph EphemeralInfra [Infraestrutura Descartável]
            PgTest[(PostgreSQL - Testcontainers)]
        end
        
        TestRunner -->|Instancia| Repo[VoteRepository]
        Repo -->|Grava/Lê| PgTest
    end
    
    style TestEnvironment fill:#f0f4c3,stroke:#827717,stroke-width:2px,stroke-dasharray: 5 5
```
