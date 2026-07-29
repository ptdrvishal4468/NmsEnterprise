# Enterprise Network Management & Cybersecurity Platform (NMS)

A high-throughput, multi-tenant Enterprise Network Management System built using **.NET 10**, **Clean Architecture**, and **CQRS**. Designed for low-latency network telemetry monitoring (ICMP, SNMPv3, Syslog), real-time anomaly streaming, and security audit compliance.

---

## 🏗 System Architecture

The solution strictly adheres to Clean Architecture principles:

- **Nms.Domain**: Core enterprise logic, base aggregates, enums, domain exceptions, and specifications.
- **Nms.Application**: Orchestration layer using CQRS commands/queries, validation behaviors, and DTOs.
- **Nms.Infrastructure**: SQL Server persistence, EF Core mappings, Lextm SNMP collector, and background pollers.
- **Nms.Api**: RESTful endpoints, custom authorization middleware, and SignalR real-time hubs.

---

## 🛠 Tech Stack

- **Backend Framework:** .NET 10 (C# 14) / ASP.NET Core Web API
- **Persistence:** SQL Server Express 2022 / Entity Framework Core 9
- **Networking & Polling:** SharpSnmpLib, System.Threading.Channels
- **Validation & Pipeline:** FluentValidation
- **Containerization:** Docker & Docker Compose
- **Testing:** xUnit, FluentAssertions, NetArchTest.eNet

---

## 🚀 Local Setup with Docker

1. Clone the repository:
   ```bash
   git clone [https://github.com/ptdrvishal4468/NmsEnterprise.git](https://github.com/ptdrvishal4468/NmsEnterprise.git)
   cd NmsEnterprise