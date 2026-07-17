#!/usr/bin/env bash
set -euo pipefail

# Restore the dev Postgres database from a backup in ~/dev/backupdb.
# Falls back to the db container if pg_restore is not installed locally.
# Usage: ./scripts/restore_db.sh [path/to/backup.dump]

COMPOSE_FILE="docker-compose.dev.yaml"
BACKUP_DIR="${BACKUP_DIR:-$HOME/dev/backupdb}"
HOST="${PGHOST:-localhost}"
PORT="${PGPORT:-5433}"
DB="${PGDATABASE:-chroniclesdb_dev}"
USER="${PGUSER:-chronicles_dev}"
PASSWORD="${PGPASSWORD:-devpassword}"

FILE="${1:-}"
if [ -z "$FILE" ]; then
  if ls "$BACKUP_DIR"/*.dump >/dev/null 2>&1; then
    FILE=$(ls -t "$BACKUP_DIR"/*.dump | head -n1)
  fi
fi

if [ -z "$FILE" ] || [ ! -f "$FILE" ]; then
  echo "No backup file specified and none found in $BACKUP_DIR"
  exit 1
fi

echo "About to restore '$FILE' into database '$DB' on $HOST:$PORT as $USER"
read -r -p "This will overwrite data in the target database. Continue? [y/N]: " CONFIRM
if [ "$CONFIRM" != "y" ]; then
  echo "Restore aborted."
  exit 1
fi

export PGPASSWORD="$PASSWORD"

if command -v pg_restore >/dev/null 2>&1; then
  echo "Restoring using local pg_restore..."
  pg_restore --host="$HOST" --port="$PORT" --username="$USER" --dbname="$DB" --verbose --clean --no-owner "$FILE"
  echo "Restore complete."
  exit 0
fi

if ! command -v docker >/dev/null 2>&1; then
  echo "Error: pg_restore not found locally and docker is not installed."
  exit 2
fi

if ! docker compose -f "$COMPOSE_FILE" ps -q db >/dev/null 2>&1; then
  echo "Error: db service is not running under $COMPOSE_FILE. Start the stack first."
  exit 2
fi

REMOTE_FILE="/tmp/$(basename "$FILE")"

docker compose -f "$COMPOSE_FILE" cp "$FILE" "db:$REMOTE_FILE"
docker compose -f "$COMPOSE_FILE" exec -T -e PGPASSWORD="$PASSWORD" db \
  pg_restore --username="$USER" --dbname="$DB" --verbose --clean --no-owner "$REMOTE_FILE"

docker compose -f "$COMPOSE_FILE" exec -T db rm -f "$REMOTE_FILE" >/dev/null 2>&1 || true

echo "Restore complete (performed via db container)."
