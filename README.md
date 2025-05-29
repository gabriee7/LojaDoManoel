# BoxOptimizer Microservice – Guia Rápido

## Pré-requisitos

- Docker  
- Docker Compose

## Execução com Docker Compose

1. **Clonar o repositório**  
   ```bash
   git clone https://github.com/gabriee7/LojaDoManoel.git
   ```

2. **Entrar no diretório do microserviço, raiz da solução (.sln)**  
   ```bash
   cd LojaDoManoel/BoxOptimizerMicroservice
   ```

3. **Construir e iniciar containers**  *(certifique-se que o Docker está rodando antes de executar o seguinte comando)*
   ```bash
   docker-compose up --build -d
   ```
   A configuração do docker compose contém a construção e inicialização da API e do SQLServer, além da execução dos testes unitários (xUnit, Moq e EF in-memory).
## Acessar a API

- **Swagger UI:** `http://localhost:8080/swagger`  
- **Autenticação:** via header `X-API-KEY`  
  ```
  RzW8qYxXq0MwQzV6bXy0Ek_AbnN9pS-CaYm9pDkSn3w
  ```  
  Use o botão **Authorize** no Swagger para colar a chave acima.
  - Para a autenticação na API foi arquitetado e implementado o formato de APIKey através do campo X-API-KEY no header da requisição, este formato é comumente utilizado na autenticação de microsserviço ou comunicação entre APIs. Portanto, utilize a seguinte chave API acima que foi inserida no banco de dados via seed.

## Melhorias Futuras

- Estratégias de empacotamento  
      A implementação dessas estratégias se beneficia dos princípios SOLID e padrões de projeto aplicados para garantir flexibilidade, reutilização e fácil manutenção. Com o uso de abstrações e estratégias, é possível alternar entre estratégias sem alterar a lógica principal, tornando o sistema extensível para novas demandas logísticas sem comprometer a estrutura existente.

- Integração com fila de mensagens (RabbitMQ/Azure Service Bus)  
  A comunicação assíncrona via filas melhora a escalabilidade e a resiliência do sistema, desacoplando o processamento das requisições. Essa abordagem permite processar grandes volumes de dados de forma controlada, com maior tolerância a falhas e suporte a arquiteturas orientadas a eventos.

- Dashboards de métricas (Prometheus + Grafana)  
  A integração com Prometheus e Grafana permite monitorar em tempo real o desempenho e a saúde do microserviço. Isso facilita a identificação de gargalos, tomada de decisões proativas e garante maior confiabilidade na operação, apoiando práticas de observabilidade e melhoria contínua.

