# Message for Chubb Insurance

Thank you for the opportunity to work on the Claims Platform assessment.

I decided to stop at what I consider a solid Minimum Viable Product. While there are many enhancements that could be added—authentication, messaging, distributed workflows, observability, cloud deployment, advanced reporting, CI/CD, and many more—the current implementation demonstrates how I approach software architecture, backend design, and building enterprise applications.

I want to be candid about why I stopped here.

Over the past several years, approximately 90% of the public repositories on my GitHub have originated from technical assessments for companies. Many of those assessments evolved into substantial projects requiring days or even weeks of work. Unfortunately, in most cases, I never heard back from the company afterwards.

As a software developer, I'm simply looking for honest work and an opportunity to contribute. Like everyone else, I have bills to pay and a career I want to continue building.

Building software of this nature is rarely the work of one individual. Throughout my career, I've enjoyed working alongside business analysts, claims officers, QA engineers, database specialists, DevOps engineers, architects, and fellow developers. The collaboration, discussions, shared ownership, and solving problems together are what make software engineering rewarding.

This assessment reflects only a minimum iteration. The architecture was intentionally designed to support future enhancements that would naturally be developed by a team.

If Chubb is looking for someone who enjoys building enterprise systems, collaborating with others, and contributing over the long term, I would genuinely welcome the opportunity to work together.

However, if the intention is simply to have candidates produce increasingly sophisticated software without meaningful engagement or feedback afterwards, then I respectfully ask that you do not contact me for similar assessments in the future.

I believe actions speak louder than words. The strongest signal of whether a company values engineering talent is not the assessment itself, but what happens after the assessment is submitted.

Thank you again for taking the time to review my work. Regardless of the outcome, I appreciate the opportunity and wish Chubb continued success.

Kind regards,

Retnalogan Thirujanasambanthan

# Claims Platform

An enterprise-style Claims Management Platform built using a microservices architecture.

The project demonstrates modern backend architecture using ASP.NET Core, Clean Architecture, Domain Driven Design, Entity Framework Core, SQL Server, Transactional Outbox and Angular.

The solution was intentionally built as a Minimum Viable Product (MVP) to establish a solid foundation for future enterprise enhancements.

---

## Current Features

### Backend

- ASP.NET Core (.NET 10)
- Clean Architecture
- Domain Driven Design
- CQRS-ready application layer
- SQL Server
- Entity Framework Core
- Transactional Outbox Pattern
- Integration Events
- Unit Tests
- Architecture Tests

### Frontend

- Angular
- Standalone Components
- Reactive Forms
- Responsive UI
- Officer Workspace
- Claim Tracking
- Claim Submission

---

## Architecture

ClaimsPlatform

├── Gateway

├── BuildingBlocks

│ ├── Contracts

│ ├── EventBus

│ └── Observability

├── Services

│ ├── Claims

│ ├── Notifications

│ ├── Reporting

│ └── WorkManagement

└── Web

    └── Angular Portal

---

## Current Claim Workflow

Claim Submitted

↓

Officer Review

↓

Request Information (optional)

↓

Claimant Provides Information

↓

Officer Review

↓

Approve / Reject

↓

Settlement

---

## Technologies

Backend

- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server
- REST APIs

Frontend

- Angular
- SCSS
- TypeScript

Architecture

- Clean Architecture
- DDD
- Repository Pattern
- Outbox Pattern
- Event Driven Architecture

Testing

- xUnit

---

## Running

### Backend

dotnet restore

dotnet build

dotnet run --project src/Services/Claims/Claims.Api

Backend runs on

http://localhost:5142

### Angular

cd src/Web/claims-portal

npm install

ng serve

Angular runs on

http://localhost:4200

---

## Database

SQL Server

Database

ClaimsDb

Migration

dotnet ef database update

---

## Current REST APIs

POST /api/claims

GET /api/claims/reference/{claimNumber}

POST /api/claims/{id}/start-review

POST /api/claims/{id}/request-information

POST /api/claims/{id}/information-requests/{requestId}/response

POST /api/claims/{id}/approve

POST /api/claims/{id}/reject

POST /api/claims/{id}/settle

---

## Current MVP Scope

✔ Submit Claim

✔ Track Claim

✔ Officer Workspace

✔ Review Workflow

✔ Information Requests

✔ Approval

✔ Rejection

✔ Settlement

✔ SQL Persistence

✔ Transactional Outbox

---

## Application Walkthrough

![Backend_Postman_API_calls](docs/Backend_showcase.gif)
![UI_frontend](docs/Frontend_showcase.gif)

## Future Enhancements

Authentication & Authorization

- JWT Authentication
- Refresh Tokens
- Identity Server
- Azure AD / Entra ID
- Role Based Access Control

Workflow

- Multi-level Approval
- SLA Management
- Escalation Rules
- Claim Assignment
- Fraud Review

Messaging

- Apache Kafka
- RabbitMQ
- Dead Letter Queue
- Retry Policies

Notifications

- Email
- SMS
- Push Notifications

Reporting

- Power BI
- Operational Dashboards
- Claim Analytics
- KPIs

Observability

- Serilog
- OpenTelemetry
- Prometheus
- Grafana
- Jaeger

Cloud

- Docker
- Kubernetes
- Helm
- Azure AKS
- AWS EKS

Security

- API Gateway
- Rate Limiting
- Secrets Management
- Encryption
- Audit Trail

CI/CD

- GitHub Actions
- Azure DevOps
- Jenkins

Testing

- Integration Tests
- Contract Tests
- Load Testing
- Playwright
- End-to-End Tests