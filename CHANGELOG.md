
# Changelog

All notable changes to this project will be documented in this file.

This project follows a Sprint-based development process.

---

# Sprint 1 - Project Initialization

## Added

* Initial Clean Architecture solution
* Domain / Application / Infrastructure / Api separation
* Entity Framework Core integration
* SQL Server configuration
* Dependency Injection
* Global exception handling
* Result<T> pattern
* Error model
* MediatR integration
* FluentValidation
* Pipeline Behaviors

  * LoggingBehavior
  * ValidationBehavior
  * PerformanceBehavior
* Serilog logging
* CorrelationId middleware

---

# Sprint 2 - Authentication & Authorization

## Authentication

### Added

* JWT Authentication
* Refresh Token
* AuthenticationService
* JwtTokenGenerator
* PasswordHasher (BCrypt)
* CurrentUser
* LoginAttemptService
* UserPermissionService

### Security

Added:

* Failed login protection
* Account lockout
* Refresh Token Rotation
* Token expiration validation

### Authorization

Added:

* Role
* Permission
* UserRole
* RolePermission
* Permission Authorization Handler
* JWT Permission Claims
* Policy-based Authorization

### Persistence

Added:

* RefreshToken Entity
* EF Core Migration
* Authentication related database schema

### Refactor

* Introduced Authentication module
* Unified authentication models
* Decoupled LoginHandler
* Centralized Token generation
* Improved folder structure

---

# Sprint 3 (Planned)

## Users

* User Management
* User CRUD
* User Profile

## Roles

* Role Management
* Assign Roles

## Permissions

* Permission Management
* Dynamic Permission Assignment

## Audit

* Security Audit
* Operation Logs
* Login Logs

---

# Sprint 4 (Planned)

MES Core

* Factory
* Workshop
* Production Line
* Workstation
* Product
* BOM
* Work Order

---

# Sprint 5 (Planned)

Production Execution

* Dispatch
* Start Production
* Report Production
* Finish Production
* Production Traceability

---

# Sprint 6 (Planned)

Infrastructure Enhancement

* Redis
* Distributed Cache
* Docker
* CI/CD
* Performance Optimization
* Monitoring
