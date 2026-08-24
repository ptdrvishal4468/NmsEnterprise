# Enterprise NMS Production Deployment Guide

## Architecture Overview
The production deployment stack isolates all persistent databases and caching instances behind a secure Nginx reverse proxy running inside a dedicated Docker bridge network.

```text
Client
  │ (HTTPS / Port 443)
  ▼
Nginx Reverse Proxy (TLS Termination, Security Headers, Gzip)
  │ (HTTP / Port 8080)
  ▼
Nms.Api (.NET 10 Container, Non-Root Runtime)
  ├── SQL Server 2022 (Internal Network Only, Persistent Named Volume)
  └── Redis 7 (Internal Network Only, Authenticated, Persistent AOF)