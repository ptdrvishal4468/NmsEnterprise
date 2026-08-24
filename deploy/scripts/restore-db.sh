#!/usr/bin/env bash
set -eo pipefail

if [ -z "$1" ]; then
  echo "Usage: $0 <backup-filename.bak>"
  exit 1
fi

BACKUP_FILE="/var/opt/mssql/backups/$1"
CONTAINER_NAME="nms-sqlserver"

echo "==> [NMS Restore] Restoring Database from ${BACKUP_FILE}..."

docker exec "${CONTAINER_NAME}" /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C \
  -Q "
  ALTER DATABASE [NmsEnterpriseDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
  RESTORE DATABASE [NmsEnterpriseDb] FROM DISK = N'${BACKUP_FILE}' WITH REPLACE;
  ALTER DATABASE [NmsEnterpriseDb] SET MULTI_USER;
  "

echo "==> [NMS Restore] Database restore completed successfully."