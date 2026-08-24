#!/usr/bin/env bash
set -eo pipefail

BACKUP_DIR="/var/opt/mssql/backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="${BACKUP_DIR}/NmsEnterpriseDb_Full_${TIMESTAMP}.bak"
CONTAINER_NAME="nms-sqlserver"

echo "==> [NMS Backup] Initiating Full SQL Server Database Backup: ${BACKUP_FILE}"

docker exec "${CONTAINER_NAME}" /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C \
  -Q "BACKUP DATABASE [NmsEnterpriseDb] TO DISK = N'${BACKUP_FILE}' WITH CHECKSUM, FORMAT, INIT, STATS = 10;"

echo "==> [NMS Backup] Verifying Backup Integrity..."
docker exec "${CONTAINER_NAME}" /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C \
  -Q "RESTORE VERIFYONLY FROM DISK = N'${BACKUP_FILE}' WITH CHECKSUM;"

echo "==> [NMS Backup] Backup successfully created and verified: ${BACKUP_FILE}"