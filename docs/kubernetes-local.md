# Kubernetes local com Docker Desktop

Este guia descreve como validar o ambiente Kubernetes do SolidarityHub localmente usando o Kubernetes do Docker Desktop.

O objetivo deste roteiro e:

- testar o overlay `dev` com imagens locais
- validar a estrutura do `kustomize`
- simular o comportamento do `staging` com imagens publicadas no GHCR
- preparar a base para o deploy futuro no AKS

Este documento nao cobre ainda:

- `overlays/production`
- automacao do deploy via GitHub Actions
- configuracao real do AKS na Azure

## Estrutura usada

Os manifests Kubernetes do projeto estao em:

- `platform/k8s/base`
- `platform/k8s/overlays/dev`
- `platform/k8s/overlays/staging`

O comportamento esperado e:

- `dev`: usa imagens locais `solidarityhub/*:local`
- `staging`: usa imagens no GHCR

## Pre-requisitos

Antes de comecar, confirme:

- Docker Desktop instalado
- Docker Desktop configurado para `Linux containers`
- Kubernetes do Docker Desktop habilitado
- `kubectl` disponivel no terminal
- imagens locais buildadas para o ambiente `dev`

## 1. Habilitar o Kubernetes do Docker Desktop

No Docker Desktop:

1. abra `Settings`
2. acesse `Kubernetes`
3. marque `Enable Kubernetes`
4. aplique as alteracoes
5. aguarde o cluster iniciar

Depois valide no terminal:

```powershell
kubectl config current-context
kubectl get nodes
```

Resultado esperado:

- contexto atual: `docker-desktop`
- ao menos um node com status `Ready`

## 2. Limpar execucoes anteriores

Antes de um novo teste, limpe o ambiente local:

```powershell
kubectl delete namespace solidarityhub --ignore-not-found=true
docker compose -f D:\Projects\SolidarityHub\docker-compose.yml down
```

Isso evita conflito entre:

- containers do `docker-compose`
- pods do Kubernetes
- nomes de servicos
- volumes persistentes em uso

## 3. Validar o `kustomize`

Antes de aplicar qualquer ambiente, renderize os manifests:

```powershell
kubectl kustomize D:\Projects\SolidarityHub\platform\k8s\overlays\dev > $null
kubectl kustomize D:\Projects\SolidarityHub\platform\k8s\overlays\staging > $null
```

Se os dois comandos terminarem sem erro, os overlays estao estruturalmente validos.

## 4. Testar o ambiente `dev`

### 4.1 Buildar as imagens locais

O overlay `dev` usa estas imagens:

- `solidarityhub/api-gateway:local`
- `solidarityhub/identity-api:local`
- `solidarityhub/campaign-api:local`
- `solidarityhub/donation-processor:local`

Execute:

```powershell
docker build -t solidarityhub/api-gateway:local -f D:\Projects\SolidarityHub\src\Gateways\ApiGateway\Dockerfile D:\Projects\SolidarityHub
docker build -t solidarityhub/identity-api:local -f D:\Projects\SolidarityHub\src\Services\Identity\Identity.API\Dockerfile D:\Projects\SolidarityHub
docker build -t solidarityhub/campaign-api:local -f D:\Projects\SolidarityHub\src\Services\Campaign\Campaign.API\Dockerfile D:\Projects\SolidarityHub
docker build -t solidarityhub/donation-processor:local -f D:\Projects\SolidarityHub\src\Workers\DonationProcessor\Dockerfile D:\Projects\SolidarityHub
```

### 4.2 Confirmar as imagens locais

```powershell
docker image ls | Select-String "solidarityhub"
```

Voce deve ver as quatro imagens com tag `local`.

### 4.3 Aplicar o overlay `dev`

```powershell
kubectl apply -k D:\Projects\SolidarityHub\platform\k8s\overlays\dev
```

### 4.4 Acompanhar os rollouts de infraestrutura

```powershell
kubectl rollout status deployment/postgres -n solidarityhub --timeout=180s
kubectl rollout status deployment/rabbitmq -n solidarityhub --timeout=180s
kubectl rollout status deployment/jaeger -n solidarityhub --timeout=180s
kubectl rollout status deployment/loki -n solidarityhub --timeout=180s
kubectl rollout status deployment/prometheus -n solidarityhub --timeout=180s
kubectl rollout status deployment/grafana -n solidarityhub --timeout=180s
```

### 4.5 Acompanhar os rollouts das aplicacoes

```powershell
kubectl rollout status deployment/identity-api -n solidarityhub --timeout=240s
kubectl rollout status deployment/campaign-api -n solidarityhub --timeout=240s
kubectl rollout status deployment/donation-processor -n solidarityhub --timeout=240s
kubectl rollout status deployment/api-gateway -n solidarityhub --timeout=240s
```

### 4.6 Validar recursos criados

```powershell
kubectl get all -n solidarityhub
kubectl get pvc -n solidarityhub
kubectl get configmap -n solidarityhub
kubectl get secret -n solidarityhub
```

Resultado esperado:

- pods das aplicacoes e da infraestrutura em `Running`
- PVCs em `Bound`
- ConfigMaps e Secrets criados no namespace

### 4.7 Validar a API Gateway

Abra um port-forward:

```powershell
kubectl port-forward svc/api-gateway 5000:80 -n solidarityhub
```

Em outro terminal:

```powershell
Invoke-WebRequest http://localhost:5000/health
```

Se o gateway expuser Swagger ou docs, teste tambem:

- `http://localhost:5000/docs`

### 4.8 Validar observabilidade

Abra os port-forwards:

```powershell
kubectl port-forward svc/grafana 3000:3000 -n solidarityhub
kubectl port-forward svc/prometheus 9090:9090 -n solidarityhub
kubectl port-forward svc/jaeger 16686:16686 -n solidarityhub
kubectl port-forward svc/rabbitmq 15672:15672 -n solidarityhub
```

Depois valide:

- Grafana: `http://localhost:3000`
- Prometheus: `http://localhost:9090`
- Jaeger: `http://localhost:16686`
- RabbitMQ Management: `http://localhost:15672`

### 4.9 Validar os targets no Prometheus

No Prometheus, confirme que os targets estao `UP`:

- `api-gateway`
- `identity-api`
- `campaign-api`
- `donation-processor`

### 4.10 Validar o dashboard no Grafana

No Grafana, confirme:

- datasources carregadas
- dashboard `SolidarityHub Overview`
- ausencia de erro de datasource

### 4.11 Smoke test funcional

Use o gateway para validar o fluxo principal:

1. autenticar ou registrar usuario
2. criar campanha
3. disparar uma doacao
4. conferir processamento pelo worker

Depois verifique os logs do worker:

```powershell
kubectl logs deployment/donation-processor -n solidarityhub --tail=100
```

## 5. Troubleshooting do ambiente `dev`

Se algum pod falhar:

```powershell
kubectl get pods -n solidarityhub
kubectl describe pod <nome-do-pod> -n solidarityhub
kubectl logs <nome-do-pod> -n solidarityhub
```

Logs uteis por deployment:

```powershell
kubectl logs deployment/identity-api -n solidarityhub
kubectl logs deployment/campaign-api -n solidarityhub
kubectl logs deployment/donation-processor -n solidarityhub
kubectl logs deployment/api-gateway -n solidarityhub
kubectl logs deployment/postgres -n solidarityhub
kubectl logs deployment/rabbitmq -n solidarityhub
```

Para ver eventos mais recentes do namespace:

```powershell
kubectl get events -n solidarityhub --sort-by=.lastTimestamp
```

Para reiniciar um deployment:

```powershell
kubectl rollout restart deployment/identity-api -n solidarityhub
kubectl rollout status deployment/identity-api -n solidarityhub
```

## 6. Simular o ambiente `staging` localmente

O `staging` nao testa o AKS em si. Ele testa localmente:

- imagens vindas do GHCR
- `imagePullSecrets`
- configuracao `Staging`
- comportamento do overlay `staging`

### 6.1 Publicar imagens no GHCR

Antes do teste, confirme que estas imagens existem no GHCR:

- `ghcr.io/alessandromaia/solidarityhub/api-gateway:<versao>`
- `ghcr.io/alessandromaia/solidarityhub/identity-api:<versao>`
- `ghcr.io/alessandromaia/solidarityhub/campaign-api:<versao>`
- `ghcr.io/alessandromaia/solidarityhub/donation-processor:<versao>`

### 6.2 Ajustar a versao no overlay

Edite [platform/k8s/overlays/staging/kustomization.yaml](/D:/Projects/SolidarityHub/platform/k8s/overlays/staging/kustomization.yaml) e troque `1.0.0` pela versao real publicada.

### 6.3 Preencher o secret do GHCR

Edite [platform/k8s/overlays/staging/ghcr-secret.yaml](/D:/Projects/SolidarityHub/platform/k8s/overlays/staging/ghcr-secret.yaml) com as credenciais reais do GHCR.

### 6.4 Limpar o namespace antes do teste

```powershell
kubectl delete namespace solidarityhub
```

### 6.5 Validar o overlay `staging`

```powershell
kubectl kustomize D:\Projects\SolidarityHub\platform\k8s\overlays\staging > $null
```

### 6.6 Aplicar o overlay `staging`

```powershell
kubectl apply -k D:\Projects\SolidarityHub\platform\k8s\overlays\staging
```

### 6.7 Validar image pull

```powershell
kubectl get pods -n solidarityhub
kubectl describe pod <nome-do-pod> -n solidarityhub
```

Se houver erro, os sinais mais comuns sao:

- `ErrImagePull`
- `ImagePullBackOff`

Nesses casos, revise:

- tag da imagem
- nome do repositorio no GHCR
- credenciais do `ghcr-secret`
- permissao do pacote no GHCR

## 7. O que o Docker Desktop valida e o que nao valida

O Docker Desktop valida:

- manifests Kubernetes
- overlays do Kustomize
- inicializacao local da aplicacao
- probes
- volumes locais
- conexao entre os servicos
- consumo de imagens locais ou do GHCR

O Docker Desktop nao valida:

- login no Azure
- permissao no cluster AKS
- `LoadBalancer` real de nuvem
- DNS e rede do ambiente Azure
- storage class real do AKS
- integracao futura com Key Vault ou identidade gerenciada

## 8. Reset rapido do ambiente

Para apagar tudo e recomecar:

```powershell
kubectl delete namespace solidarityhub
```

Depois, reaplique o ambiente desejado:

```powershell
kubectl apply -k D:\Projects\SolidarityHub\platform\k8s\overlays\dev
```

ou:

```powershell
kubectl apply -k D:\Projects\SolidarityHub\platform\k8s\overlays\staging
```

## 9. Ordem recomendada de validacao

A ordem mais segura para evoluir o projeto e:

1. validar `kubectl kustomize` no `dev`
2. buildar e subir o `dev`
3. validar gateway, worker e observabilidade
4. publicar imagens no GHCR
5. validar `staging` localmente com GHCR
6. so depois integrar o workflow de deployment para AKS
