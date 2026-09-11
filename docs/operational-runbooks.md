# Enterprise Network Management System (NMS)
# Operational Runbooks

> **Document Version:** 1.0  
> **Status:** Active Operational Guide  
> **Target Audience:** Site Reliability Engineers (SRE), DevOps Engineers, System Administrators

---

## 1. System Overview & Architecture Topology

The Enterprise Network Management System (NMS) runs as a multi-container Docker Compose stack configured in `docker-compose.prod.yml`:

* **Ingress / Reverse Proxy:** `nms-reverse-proxy` (`nginx:1.27-alpine`) on host ports 80/443
* **Backend API:** `nms-api` (`nms-enterprise-api:latest`, ASP.NET Core on .NET 10) on internal port 8080
* **Primary Relational Store:** `nms-sqlserver` (`mcr.microsoft.com/mssql/server:2022-latest`) on internal port 1433
* **Distributed Caching:** `nms-redis` (`redis:7-alpine`) on internal port 6379
* **Network Isolation:** All containers communicate across bridge network `nms-production-network`
* **Persistent Volumes:** `nms_sql_data`, `nms_sql_backups`, `nms_redis_data`

---

## 2. Standard Operational Runbooks (Runbooks 1–3)

### Runbook 01: Application Startup
* **Objective:** Start all NMS platform services in the correct dependency order.
* **Procedure:**
  1. Verify `.env.production` is present and populated with required secrets.
  2. Launch stack:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml up -d
     ```
  3. Verify container state:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml ps
     ```

### Runbook 02: Application Graceful Shutdown
* **Objective:** Stop the platform cleanly without corrupting active polling queues or database transactions.
* **Procedure:**
  1. Issue graceful termination (allowing background hosted services to complete inflight tasks):
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml down
     ```

### Runbook 03: Health Check & System Verification
* **Objective:** Validate liveness and dependency readiness.
* **Procedure:**
  1. Check process liveness:
     ```bash
     curl -k -i https://localhost/health/live
     ```
     *Expected Output:* `HTTP/1.1 200 OK`, body: `Healthy`
  2. Check SQL Server and Redis readiness:
     ```bash
     curl -k -i https://localhost/health/ready
     ```
     *Expected Output:* `HTTP/1.1 200 OK`, body: `Healthy`

---

## 3. Infrastructure & Component Outage Runbooks (Runbooks 4–8)

### Runbook 04: API Service Failure / Crash Recovery
* **Symptom:** Nginx returns `502 Bad Gateway`; `nms-api` container exited or restarting.
* **Procedure:**
  1. Inspect API logs:
     ```bash
     docker logs nms-api --tail 200
     ```
  2. Restart API container:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml restart nms.api
     ```
  3. Validate `/health/live`.

### Runbook 05: Primary Database (SQL Server) Failure
* **Symptom:** `/health/ready` returns `HTTP 503 Service Unavailable`; logs indicate SQL connection failure.
* **Procedure:**
  1. Inspect SQL Server container status:
     ```bash
     docker logs nms-sqlserver --tail 100
     ```
  2. Verify database process responsiveness:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT @@VERSION;"
     ```
  3. Restart SQL Server container:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml restart sqlserver
     ```

### Runbook 06: Distributed Cache (Redis) Failure
* **Symptom:** Cache misses spike; `/health/ready` reports Redis health check failure.
* **Procedure:**
  1. Ping Redis container:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml exec redis redis-cli -a "$REDIS_PASSWORD" ping
     ```
  2. If unresponsive, restart Redis container:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml restart redis
     ```

### Runbook 07: Reverse Proxy (Nginx) Failure
* **Symptom:** Connection refused on port 80/443; ingress traffic blocked.
* **Procedure:**
  1. Check Nginx configuration syntax:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml exec reverse-proxy nginx -t
     ```
  2. Restart reverse proxy container:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml restart reverse-proxy
     ```

### Runbook 08: TLS Certificate Renewal & Ingress Update
* **Objective:** Renew expired TLS certificates without restarting the entire stack.
* **Procedure:**
  1. Place new `server.crt` and `server.key` into `deploy/certs/`.
  2. Test configuration reload:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml exec reverse-proxy nginx -s reload
     ```
  3. Verify handshake using OpenSSL:
     ```bash
     openssl s_client -connect localhost:443 -servername localhost
     ```

---

## 4. Backup, Restore & Disaster Recovery Runbooks (Runbooks 9–11)

### Runbook 09: Database Backup Execution
* **Objective:** Create an authenticated full database backup with checksum verification.
* **Procedure:**
  1. Execute the automated backup script:
     ```bash
     bash deploy/scripts/backup-db.sh
     ```
  2. Confirm backup file created under volume `/var/opt/mssql/backups`.

### Runbook 10: Database Restore Execution
* **Objective:** Restore the database from an existing `.bak` archive.
* **Procedure:**
  1. Execute restore script:
     ```bash
     bash deploy/scripts/restore-db.sh
     ```
  2. Verify schema and table integrity:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "USE NmsEnterpriseDb; SELECT COUNT(*) FROM Tenants;"
     ```

### Runbook 11: Disaster Recovery (DR) Simulation Drill
* **Objective:** Test end-to-end failover, database recreation, and restore readiness.
* **Procedure:**
  1. Run the DR simulation script:
     ```bash
     bash deploy/scripts/dr-test.sh
     ```
  2. Confirm RPO (< 1 Hour) and RTO (< 15 Minutes) targets are met.

---

## 5. Observability, Metrics & Performance Triage (Runbooks 12–17)

### Runbook 12: Log Investigation & Correlation
* **Objective:** Trace application errors across controllers, handlers, and background services.
* **Procedure:**
  1. Follow real-time structured logs:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml logs -f nms.api
     ```
  2. Query logs by correlation ID:
     ```bash
     docker logs nms-api 2>&1 | grep "CORR-"
     ```

### Runbook 13: Monitoring & OpenTelemetry Validation
* **Objective:** Verify telemetry export and dashboard metrics.
* **Procedure:**
  1. Query Performance Dashboard endpoint:
     ```bash
     curl -k -H "Authorization: Bearer <TOKEN>" https://localhost/api/v1/dashboard/performance
     ```

### Runbook 14: High CPU Utilization Remediation
* **Symptom:** Host or container CPU exceeds 85% sustained threshold.
* **Procedure:**
  1. Check per-container CPU usage:
     ```bash
     docker stats --no-stream
     ```
  2. Identify high-frequency poll jobs in `Nms.Api` logs.
  3. Increase poll intervals for intensive device profiles via API.

### Runbook 15: High Memory Utilization & Leak Investigation
* **Symptom:** Container memory continuously increases towards limit.
* **Procedure:**
  1. Capture memory diagnostics inside `nms-api`:
     ```bash
     docker stats nms-api --no-stream
     ```
  2. Review Redis memory usage:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml exec redis redis-cli -a "$REDIS_PASSWORD" info memory
     ```

### Runbook 16: High Database Load & Query Bottlenecks
* **Symptom:** SQL Server response times degrade; connection pool exhaustion.
* **Procedure:**
  1. Execute active query DMV investigation:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT TOP 10 total_worker_time/execution_count AS Avg_CPU, execution_count, query_hash FROM sys.dm_exec_query_stats ORDER BY Avg_CPU DESC;"
     ```

### Runbook 17: Polling Queue Backlog Remediation
* **Symptom:** SNMP/ICMP polling results delayed; queue reaches bounded capacity.
* **Procedure:**
  1. Check worker logs:
     ```bash
     docker logs nms-api --tail 200 | grep -i "PollingQueue"
     ```
  2. If saturated, increase consumer worker count or buffer size in `appsettings.Production.json` (`PollingQueueOptions:QueueCapacity`).

---

## 6. Protocols & Security Incident Runbooks (Runbooks 18–20)

### Runbook 18: Device Polling & Listener Failure (SNMP / Syslog)
* **Symptom:** Device metrics not updating; Syslog or SNMP traps not captured.
* **Procedure:**
  1. Inspect listener background services:
     ```bash
     docker logs nms-api --tail 100 | grep -E "SyslogListener|SnmpTrapListener"
     ```
  2. Verify UDP port bindings and firewall rules for Syslog (UDP 514) and SNMP Traps (UDP 162).

### Runbook 19: Authentication & JWT Validation Failure
* **Symptom:** Legitimate users receiving `HTTP 401 Unauthorized`.
* **Procedure:**
  1. Check token expiration configuration in `.env.production` (`JwtSettings__ExpiryMinutes`).
  2. Verify system clock synchronization across containers (`date -u`).

### Runbook 20: Security Incident & Unauthorized Access Remediation
* **Symptom:** Threat detection rules report brute force or port scan anomalies.
* **Procedure:**
  1. Fetch active threat summary:
     ```bash
     curl -k -H "Authorization: Bearer <ADMIN_TOKEN>" https://localhost/api/v1/threat-detection/summary
     ```
  2. Block offending IP addresses at Nginx reverse proxy level in `deploy/nginx/conf.d/default.conf`.
  3. Reload Nginx configuration:
     ```bash
     docker compose --env-file .env.production -f docker-compose.prod.yml exec reverse-proxy nginx -s reload
     ```