#!/usr/bin/env bash
set -eo pipefail

echo "=========================================================="
echo " Starting Disaster Recovery Drill Simulation              "
echo "=========================================================="

ENV_FILE=".env.production"
COMPOSE_FILE="docker-compose.prod.yml"

# Step 1: Create fresh baseline backup
echo "[DR-1] Generating Baseline Backup..."
./deploy/scripts/backup-db.sh

LATEST_BACKUP=$(docker exec nms-sqlserver sh -c 'ls -t /var/opt/mssql/backups/*.bak | head -1 | xargs basename')
echo "[DR-1] Latest verified backup identified: ${LATEST_BACKUP}"

# Step 2: Simulate infrastructure loss by stopping application and edge ingress
echo "[DR-2] Simulating System Failure (Stopping Services)..."
docker compose --env-file "${ENV_FILE}" -f "${COMPOSE_FILE}" stop nms.api reverse-proxy

# Step 3: Execute restore
echo "[DR-3] Executing Disaster Recovery Database Restore..."
./deploy/scripts/restore-db.sh "${LATEST_BACKUP}"

# Step 4: Restart application services
echo "[DR-4] Recovering Container Stack..."
docker compose --env-file "${ENV_FILE}" -f "${COMPOSE_FILE}" start nms.api reverse-proxy

# Step 5: Verification
echo "[DR-5] Verifying API Reachability & Health Endpoint..."
sleep 5
STATUS_CODE=$(curl -k -s -o /dev/null -w "%{http_code}" https://localhost/health/ready || true)

if [ "${STATUS_CODE}" -eq 200 ]; then
  echo "==> [DR Drill SUCCESS] System fully recovered and responding (Status: 200)."
else
  echo "==> [DR Drill FAILED] Health check returned HTTP ${STATUS_CODE}."
  exit 1
fi