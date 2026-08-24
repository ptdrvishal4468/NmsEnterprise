# Enterprise Network Management & Cybersecurity Platform (NMS)

A high-throughput, multi-tenant Enterprise Network Management System built using **.NET 10**, **Clean Architecture**, and **CQRS**. Engineered for low-latency network telemetry monitoring (ICMP, SNMPv3, Syslog), real-time anomaly streaming, threat detection, automated configuration backup/restore, and enterprise compliance auditing.

---

## 🏗 System Architecture

The solution strictly adheres to Clean Architecture principles:

- **Nms.Domain**: Core enterprise entities, base aggregates, enums, domain exceptions, and specifications.
- **Nms.Application**: Orchestration layer containing CQRS commands/queries (MediatR), pipeline validation behaviors (FluentValidation), DTOs, and interface contracts.
- **Nms.Infrastructure**: SQL Server persistence (EF Core 10), Redis caching/token store, SNMP collector (SharpSnmpLib), SSH execution, Syslog/SNMP Trap UDP listeners, and background workers.
- **Nms.Api**: RESTful endpoints, dynamic RBAC permission middleware, multi-tenant resolution, and health probes.

---

## 🌐 Production Topology & Network Isolation (Phase 53)

```text
Client (Browser / REST API)
   │
   │ (HTTPS:443 / HTTP:80 Redirect)
   ▼
Nginx Reverse Proxy (`nms-reverse-proxy`)
   │
   │ (Internal HTTP:8080 - `nms-production-network`)
   ▼
NMS API Service (`nms-api`)
   ├── SQL Server 2022 (`nms-sqlserver` - Internal Data & Backup Volumes)
   └── Redis 7 (`nms-redis` - Internal Network, Authenticated, AOF Persistence)