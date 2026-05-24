# Zabbix para monitoramento de infraestrutura + pre-montagem de dashboard no Grafana

Este documento descreve **como encaixar o Zabbix na stack atual do SolidarityHub** para monitoramento de infraestrutura, sem substituir o que ja existe hoje com Prometheus, Loki, Jaeger e Grafana.

O foco aqui e:

- monitorar **Docker Compose** no ambiente local;
- monitorar **AKS/Kubernetes** no ambiente de cluster;
- manter o **Prometheus** como fonte principal para metricas de aplicacao;
- usar o **Zabbix** como camada de disponibilidade, health e monitoramento de infraestrutura;
- preparar um **dashboard de Grafana** que combine a stack atual com dados oriundos do Zabbix.

---

## 1. Papel do Zabbix na arquitetura

### O que o Zabbix monitora melhor

O Zabbix se encaixa muito bem para:

- disponibilidade de servicos;
- status de containers e pods;
- uso de CPU e memoria;
- disponibilidade de portas;
- health checks HTTP;
- alertas simples de indisponibilidade;
- monitoramento de host/infraestrutura.

### O que o Prometheus continua fazendo

O Prometheus deve continuar sendo usado para:

- metricas da aplicacao;
- metricas customizadas dos servicos;
- contagem de requisicoes;
- timers e contadores instrumentados no codigo;
- scraping de `/metrics`.

### Recomendacao de desenho final

- **Prometheus**: metricas da aplicacao e, se desejado, metricas de cluster
- **Zabbix**: monitoramento de disponibilidade e infraestrutura
- **Grafana**: dashboards consolidados
- **Loki**: logs
- **Jaeger**: tracing

Em outras palavras:

- **nao substituir Prometheus**
- **nao substituir Loki**
- **nao substituir Jaeger**
- **adicionar Zabbix como complemento**

---

## 2. Escopo minimo recomendado para entrega

Se a meta for uma implementacao pragmatica, o minimo recomendado e:

1. Subir **Zabbix Server + Zabbix Web + banco do Zabbix** no ambiente local
2. Monitorar:
   - API Gateway
   - PostgreSQL
   - RabbitMQ
   - Grafana
   - Prometheus
   - Jaeger
3. Expor no Grafana:
   - disponibilidade dos servicos
   - saude do ambiente
   - consumo basico de recursos

---

## 3. Estrategia recomendada

### Estrategia A - mais segura para a entrega

Usar o Zabbix no **Docker Compose**, monitorando:

- os containers locais;
- os endpoints HTTP da stack;
- o ambiente local usado na demonstracao.

### Estrategia B - mais completa

Usar o Zabbix para monitorar tambem o **AKS/Kubernetes**, com:

- checks HTTP para servicos expostos;
- monitoramento de nodes/pods;
- opcionalmente `zabbix-agent2` ou `zabbix-proxy` no cluster.

### Recomendacao pratica

Para a primeira versao:

- implementar a **Estrategia A**
- deixar a **Estrategia B** preparada no documento e no backlog

---

## 4. Instrumentacao do Zabbix no Docker Compose

### 4.1 Componentes recomendados

Para o ambiente local, adicionar:

- `zabbix-postgres`
- `zabbix-server`
- `zabbix-web`
- `zabbix-agent2`

### 4.2 Servicos sugeridos para adicionar ao `docker-compose.yml`

> Este bloco e um **modelo base**. Ajuste versoes, senhas e portas conforme o ambiente.

```yaml
  zabbix-postgres:
    image: postgres:18-alpine
    container_name: sh-zabbix-postgres
    environment:
      POSTGRES_DB: zabbix
      POSTGRES_USER: zabbix
      POSTGRES_PASSWORD: change-me-zabbix-db-password
    volumes:
      - zabbix-postgres-data:/var/lib/postgresql/data
    networks:
      - sh-network

  zabbix-server:
    image: zabbix/zabbix-server-pgsql:alpine-7.0-latest
    container_name: sh-zabbix-server
    environment:
      DB_SERVER_HOST: zabbix-postgres
      POSTGRES_DB: zabbix
      POSTGRES_USER: zabbix
      POSTGRES_PASSWORD: change-me-zabbix-db-password
    ports:
      - "10051:10051"
    depends_on:
      - zabbix-postgres
    networks:
      - sh-network

  zabbix-web:
    image: zabbix/zabbix-web-nginx-pgsql:alpine-7.0-latest
    container_name: sh-zabbix-web
    environment:
      DB_SERVER_HOST: zabbix-postgres
      POSTGRES_DB: zabbix
      POSTGRES_USER: zabbix
      POSTGRES_PASSWORD: change-me-zabbix-db-password
      ZBX_SERVER_HOST: zabbix-server
      PHP_TZ: America/Sao_Paulo
    ports:
      - "8081:8080"
    depends_on:
      - zabbix-server
    networks:
      - sh-network

  zabbix-agent2:
    image: zabbix/zabbix-agent2:alpine-7.0-latest
    container_name: sh-zabbix-agent2
    environment:
      ZBX_SERVER_HOST: zabbix-server
      ZBX_HOSTNAME: docker-host
      ZBX_ACTIVE_ALLOW: "true"
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
    depends_on:
      - zabbix-server
    networks:
      - sh-network
```

### 4.3 Volumes recomendados

```yaml
volumes:
  zabbix-postgres-data:
```

### 4.4 Acesso esperado

Depois do `docker compose up -d`, o frontend do Zabbix deve ficar disponivel em:

- [http://localhost:8081](http://localhost:8081)

Credenciais padrao variam por imagem/versao. Ajuste no ambiente final se desejar customizar.

---

## 5. Instrumentacao recomendada no Zabbix

### 5.1 Alvos minimos do SolidarityHub

Crie hosts, grupos ou itens para monitorar pelo menos:

- `api.gateway`
- `identity.api`
- `campaign.api`
- `donation.processor`
- `postgres`
- `rabbitmq`
- `grafana`
- `prometheus`
- `jaeger`

### 5.2 Itens e checks recomendados

#### Disponibilidade por porta TCP

Exemplos:

```text
net.tcp.service[tcp,api.gateway,8080]
net.tcp.service[tcp,postgres,5432]
net.tcp.service[tcp,rabbitmq,5672]
net.tcp.service[tcp,grafana,3000]
net.tcp.service[tcp,prometheus,9090]
net.tcp.service[tcp,jaeger,16686]
```

#### Health checks HTTP

Exemplos de HTTP checks:

```text
http://api.gateway:8080/health
http://identity.api:8080/health
http://campaign.api:8080/health
```

#### Containers executando

Se usar `zabbix-agent2` com suporte a Docker/plugin habilitado, monitore:

- quantidade de containers em execucao
- restarts
- consumo de CPU por container
- memoria por container

#### Recursos do host

Itens classicos:

```text
system.cpu.util[,system]
vm.memory.size[available]
vfs.fs.size[/,pfree]
```

### 5.3 Triggers minimos recomendados

Crie triggers para:

- gateway indisponivel
- banco indisponivel
- broker indisponivel
- Grafana indisponivel
- Prometheus indisponivel
- alto uso de CPU
- memoria livre abaixo de limite

Exemplos conceituais:

```text
last(/docker-host/net.tcp.service[tcp,api.gateway,8080])=0
last(/docker-host/net.tcp.service[tcp,postgres,5432])=0
last(/docker-host/net.tcp.service[tcp,rabbitmq,5672])=0
avg(/docker-host/system.cpu.util[,system],5m)>85
last(/docker-host/vm.memory.size[available])<536870912
```

---

## 6. Zabbix no AKS / Kubernetes

### 6.1 Recomendacao de abordagem

No AKS, o Zabbix pode entrar de duas formas:

#### Opcao 1 - mais simples

Zabbix roda fora do cluster ou no mesmo ambiente do compose e monitora:

- endpoints HTTP expostos;
- disponibilidade dos servicos;
- portas e health checks.

#### Opcao 2 - mais completa

Zabbix roda com componentes no cluster:

- `zabbix-server`
- `zabbix-web`
- `zabbix-agent2` como `DaemonSet`
- opcionalmente `zabbix-proxy`

### 6.2 O que monitorar no AKS

Itens minimos:

- disponibilidade dos pods dos servicos
- disponibilidade do gateway
- disponibilidade do RabbitMQ
- disponibilidade do PostgreSQL
- uso de CPU e memoria do cluster
- disponibilidade do Grafana e Prometheus

### 6.3 Exemplo de `DaemonSet` para agent2 no cluster

> Exemplo base para evolucao. Ajuste permissoes e mounts conforme a estrategia real de monitoramento do cluster.

```yaml
apiVersion: apps/v1
kind: DaemonSet
metadata:
  name: zabbix-agent2
  namespace: solidarityhub
spec:
  selector:
    matchLabels:
      app: zabbix-agent2
  template:
    metadata:
      labels:
        app: zabbix-agent2
    spec:
      hostNetwork: true
      hostPID: true
      containers:
        - name: zabbix-agent2
          image: zabbix/zabbix-agent2:alpine-7.0-latest
          env:
            - name: ZBX_SERVER_HOST
              value: zabbix-server
            - name: ZBX_HOSTNAME_ITEM
              value: system.hostname
          volumeMounts:
            - name: rootfs
              mountPath: /hostfs
              readOnly: true
      volumes:
        - name: rootfs
          hostPath:
            path: /
```

### 6.4 Recomendacao pratica para o AKS

Se o objetivo for demonstracao controlada, priorize:

- health checks HTTP
- disponibilidade dos servicos
- CPU/memoria basicos

Nao tente substituir a telemetria de aplicacao ja coberta por Prometheus.

---

## 7. Integracao do Zabbix com o Grafana

### 7.1 Plugin necessario

Para usar Zabbix como datasource no Grafana, o plugin mais comum e:

```text
alexanderzobnin-zabbix-app
```

### 7.2 Ajuste sugerido no `docker-compose.yml` do Grafana

No servico `grafana`, adicionar:

```yaml
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=${GRAFANA_ADMIN_PASSWORD}
      - GF_USERS_ALLOW_SIGN_UP=false
      - GF_INSTALL_PLUGINS=alexanderzobnin-zabbix-app
```

### 7.3 Ajuste sugerido no Grafana do Kubernetes

No deployment de [`platform/k8s/base/observability/grafana.yaml`](../platform/k8s/base/observability/grafana.yaml), adicionar:

```yaml
            - name: GF_INSTALL_PLUGINS
              value: alexanderzobnin-zabbix-app
```

> Em Kubernetes, o plugin pode aumentar o tempo de bootstrap do Grafana. Planeje isso no readiness do pod.

---

## 8. Datasource Zabbix para o Grafana

### 8.1 Docker Compose

Adicionar ao arquivo [`platform/observability/grafana/provisioning/datasources/datasources.yml`](../platform/observability/grafana/provisioning/datasources/datasources.yml):

```yaml
  - name: Zabbix
    uid: zabbix
    type: alexanderzobnin-zabbix-datasource
    access: proxy
    url: http://zabbix-web:8080/api_jsonrpc.php
    editable: true
    jsonData:
      username: Admin
    secureJsonData:
      password: zabbix
```

> Ajuste `username` e `password` conforme as credenciais reais do frontend Zabbix.

### 8.2 Kubernetes

Adicionar o mesmo bloco em:

- [`platform/k8s/base/observability/files/datasources.yml`](../platform/k8s/base/observability/files/datasources.yml)

Bloco:

```yaml
  - name: Zabbix
    uid: zabbix
    type: alexanderzobnin-zabbix-datasource
    access: proxy
    url: http://zabbix-web:8080/api_jsonrpc.php
    editable: true
    jsonData:
      username: Admin
    secureJsonData:
      password: zabbix
```

---

## 9. Pre-montagem do dashboard no Grafana

### 9.1 Objetivo do dashboard

O dashboard deve responder rapidamente:

- a infraestrutura esta saudavel?
- os servicos principais estao disponiveis?
- o broker e o banco estao acessiveis?
- houve queda de algum componente?
- o ambiente local ou o cluster esta sob pressao?

### 9.2 Layout recomendado

#### Linha 1 - saude geral

- Gateway Online
- PostgreSQL Online
- RabbitMQ Online
- Grafana Online
- Prometheus Online

#### Linha 2 - infraestrutura

- CPU do host ou node
- Memoria disponivel
- Disco livre
- Containers ou pods ativos

#### Linha 3 - servicos da solucao

- identity-api health
- campaign-api health
- donation-processor health
- total de falhas de checks

#### Linha 4 - correlacao com stack atual

- Services Up (Prometheus)
- Donation Intents Created (Prometheus)
- Donation Intents Processed (Prometheus)

### 9.3 Arquivo sugerido para o dashboard

Salvar como:

```text
platform/observability/grafana/provisioning/dashboards/json/solidarityhub-infra-zabbix.json
```

### 9.4 Modelo base de definicao do dashboard

O provider atual ja carrega JSONs em:

```text
/etc/grafana/provisioning/dashboards/json
```

Entao basta adicionar um novo JSON nessa pasta para o compose local.

### 9.5 Especificacao funcional da pre-montagem

Use este modelo como blueprint do dashboard:

```json
{
  "title": "SolidarityHub - Infra + Zabbix",
  "folder": "SolidarityHub",
  "time": {
    "from": "now-6h",
    "to": "now"
  },
  "rows": [
    {
      "title": "Saude Geral",
      "panels": [
        {
          "title": "Gateway Online",
          "datasource": "Zabbix",
          "metric": "HTTP health do api.gateway"
        },
        {
          "title": "PostgreSQL Online",
          "datasource": "Zabbix",
          "metric": "TCP 5432"
        },
        {
          "title": "RabbitMQ Online",
          "datasource": "Zabbix",
          "metric": "TCP 5672"
        },
        {
          "title": "Grafana Online",
          "datasource": "Zabbix",
          "metric": "TCP 3000"
        },
        {
          "title": "Prometheus Online",
          "datasource": "Zabbix",
          "metric": "TCP 9090"
        }
      ]
    },
    {
      "title": "Infraestrutura",
      "panels": [
        {
          "title": "CPU do host/node",
          "datasource": "Zabbix",
          "metric": "system.cpu.util[,system]"
        },
        {
          "title": "Memoria disponivel",
          "datasource": "Zabbix",
          "metric": "vm.memory.size[available]"
        },
        {
          "title": "Disco livre",
          "datasource": "Zabbix",
          "metric": "vfs.fs.size[/,pfree]"
        },
        {
          "title": "Containers/Pods ativos",
          "datasource": "Zabbix",
          "metric": "total de servicos monitorados ativos"
        }
      ]
    },
    {
      "title": "Aplicacao",
      "panels": [
        {
          "title": "Servicos UP",
          "datasource": "Prometheus",
          "metric": "up{job=~\"campaign.api|identity.api|donation.processor\"}"
        },
        {
          "title": "Donation Intents Created",
          "datasource": "Prometheus",
          "metric": "solidarityhub_donation_intents_created_total"
        },
        {
          "title": "Donation Intents Processed",
          "datasource": "Prometheus",
          "metric": "solidarityhub_donation_intents_processed_total"
        }
      ]
    }
  ]
}
```

### 9.6 Paineis concretos recomendados no Grafana

Ao montar esse dashboard no UI do Grafana, use:

- **Stat** para disponibilidade
- **Gauge** para CPU/memoria/disco
- **Time series** para historico de indisponibilidade
- **Table** para ultimos incidentes por host/servico

---

## 10. Ajustes necessarios no repositorio para suportar esse dashboard

### 10.1 Docker Compose

Atualizar:

- [`docker-compose.yml`](../docker-compose.yml)
- [`platform/observability/grafana/provisioning/datasources/datasources.yml`](../platform/observability/grafana/provisioning/datasources/datasources.yml)

Adicionar:

- novo JSON do dashboard em `platform/observability/grafana/provisioning/dashboards/json/`

### 10.2 Kubernetes

Atualizar:

- [`platform/k8s/base/observability/grafana.yaml`](../platform/k8s/base/observability/grafana.yaml)
- [`platform/k8s/base/observability/files/datasources.yml`](../platform/k8s/base/observability/files/datasources.yml)
- [`platform/k8s/base/kustomization.yaml`](../platform/k8s/base/kustomization.yaml)

Para o Kustomize, incluir o novo JSON no `configMapGenerator`:

```yaml
  - name: grafana-dashboards
    files:
      - observability/files/solidarityhub-overview.json
      - observability/files/solidarityhub-infra-zabbix.json
```

E montar o novo arquivo no deployment do Grafana:

```yaml
            - name: grafana-dashboards
              mountPath: /etc/grafana/provisioning/dashboards/json/solidarityhub-infra-zabbix.json
              subPath: solidarityhub-infra-zabbix.json
```

---

## 11. Recomendacao final

Para a implementacao do hackathon e da entrega tecnica:

### Fazer agora

- adicionar Zabbix no ambiente local via Docker Compose
- monitorar disponibilidade da stack
- integrar Zabbix ao Grafana
- criar dashboard inicial de infraestrutura

### Fazer depois

- aprofundar monitoramento do AKS com agent/proxy
- ampliar alertas
- consolidar dashboards de infra + aplicacao em uma visao executiva

---

## 12. Resumo executivo

O Zabbix deve entrar como **monitoramento da infraestrutura e da disponibilidade do ambiente**, e nao como substituto da telemetria da aplicacao.

Melhor divisao de responsabilidades:

- **Prometheus**: metricas da aplicacao
- **Zabbix**: saude e disponibilidade da infraestrutura
- **Grafana**: visualizacao consolidada
- **Loki**: logs
- **Jaeger**: tracing

Essa combinacao e tecnicamente coerente, encaixa bem no que ja existe no SolidarityHub e melhora a historia de operacao da solucao sem exigir reestruturar a stack atual.
