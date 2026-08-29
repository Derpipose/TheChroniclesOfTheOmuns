#!/usr/bin/env bash
set -euo pipefail

# Backup the dev Postgres database into ~/dev/backupdb.
# Falls back to the db container if pg_dump is not installed locally.
# Usage: ./scripts/backup_db.sh

COMPOSE_FILE="../docker-compose.dev.yaml"
BACKUP_DIR="${BACKUP_DIR:-$HOME/dev/backupdb}"
HOST="${PGHOST:-localhost}"
PORT="${PGPORT:-5433}"
DB="${PGDATABASE:-chroniclesdb_dev}"
USER="${PGUSER:-chronicles_dev}"
PASSWORD="${PGPASSWORD:-devpassword}"

mkdir -p "$BACKUP_DIR"
TIMESTAMP=$(date +"%Y%m%d%H%M%S")
FILE="$BACKUP_DIR/${DB}_backup_${TIMESTAMP}.dump"

export PGPASSWORD="$PASSWORD"

echo "Backing up database '$DB' on $HOST:$PORT as $USER to: $FILE"

if command -v pg_dump >/dev/null 2>&1; then
  pg_dump -h "$HOST" -p "$PORT" -U "$USER" -F c -b -v -f "$FILE" "$DB"
  echo "Done. Backup saved to: $FILE"
  exit 0
fi

if ! command -v docker >/dev/null 2>&1; then
  echo "Error: pg_dump not found locally and docker is not installed."
  exit 2
fi

if ! docker compose -f "$COMPOSE_FILE" ps -q db >/dev/null 2>&1; then
  echo "Error: db service is not running under $COMPOSE_FILE. Start the stack first."
  exit 2
fi

docker compose -f "$COMPOSE_FILE" exec -T -e PGPASSWORD="$PASSWORD" db \
  pg_dump -h localhost -p 5432 -U "$USER" -F c -b "$DB" > "$FILE"

echo "Done. Backup saved to: $FILE (created via db container)"
