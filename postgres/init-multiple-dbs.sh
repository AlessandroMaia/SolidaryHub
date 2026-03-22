#!/bin/bash
set -euo pipefail

ADMIN_DB="${POSTGRES_DB:-postgres}"

log() {
  echo "[postgres-init] $1"
}

role_exists() {
  local role_name="$1"
  psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$ADMIN_DB" -tAc \
    "SELECT 1 FROM pg_roles WHERE rolname='${role_name}'" | grep -q 1
}

database_exists() {
  local db_name="$1"
  psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$ADMIN_DB" -tAc \
    "SELECT 1 FROM pg_database WHERE datname='${db_name}'" | grep -q 1
}

create_role_if_not_exists() {
  local role_name="$1"
  local role_password="$2"
  local password_escaped="${role_password//\'/''}"

  if role_exists "$role_name"; then
    log "Usuário '$role_name' já existe. Pulando criação."
  else
    log "Criando usuário '$role_name'..."
    psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$ADMIN_DB" \
      -c "CREATE ROLE \"$role_name\" WITH LOGIN PASSWORD '$password_escaped';"
    log "Usuário '$role_name' criado com sucesso."
  fi
}

create_database_if_not_exists() {
  local db_name="$1"
  local db_owner="$2"

  if database_exists "$db_name"; then
    log "Banco '$db_name' já existe. Pulando criação."
  else
    log "Criando banco '$db_name' com owner '$db_owner'..."
    psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$ADMIN_DB" \
      -c "CREATE DATABASE \"$db_name\" OWNER \"$db_owner\";"
    log "Banco '$db_name' criado com sucesso."
  fi
}

log "Iniciando configuração dos bancos da aplicação..."

create_role_if_not_exists "$IDENTITY_DB_USER" "$IDENTITY_DB_PASSWORD"
create_role_if_not_exists "$CAMPAIGN_DB_USER" "$CAMPAIGN_DB_PASSWORD"

create_database_if_not_exists "$IDENTITY_DB_NAME" "$IDENTITY_DB_USER"
create_database_if_not_exists "$CAMPAIGN_DB_NAME" "$CAMPAIGN_DB_USER"

log "Configuração concluída com sucesso."
