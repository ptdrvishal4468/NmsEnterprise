# Backup & Disaster Recovery Runbook

## Targets
* **Recovery Point Objective (RPO):** < 1 Hour (Target via hourly scheduled differential / full daily backups).
* **Recovery Time Objective (RTO):** < 15 Minutes (Target automated scripted restore).

## Backup Procedures
* Automated backups are executed using `deploy/scripts/backup-db.sh`.
* Backups write full database archives with native page checksum verification directly to the persistent `nms_sql_backups` volume.

## Restore & DR Procedures
1. Run `./deploy/scripts/restore-db.sh <backup-filename.bak>`.
2. Execute `./deploy/scripts/dr-test.sh` to run the standardized validation drill.