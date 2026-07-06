
# MiniMES

> 一个基于 **.NET 8 + Clean Architecture + CQRS + DDD** 构建的工业级 Manufacturing Execution System（MES）学习项目。

## 项目目标

MiniMES 并不是一个简单的 CRUD 示例，而是按照真实企业项目的开发方式，逐步实现一个可扩展、可维护、可测试的工业级 MES 系统。

整个项目采用 Sprint 的方式迭代开发，每一个 Sprint 都会完成一组独立的业务能力，同时持续优化整体架构。

---

# 技术栈

* .NET 8
* ASP.NET Core
* Entity Framework Core
* SQL Server
* MediatR（CQRS）
* FastEndpoints
* FluentValidation
* JWT Authentication
* Serilog
* Clean Architecture
* Domain Driven Design（DDD）

---

# 项目架构

```
MiniMES
│
├── MiniMES.Api              // API 层
├── MiniMES.Application      // 应用层（CQRS、业务逻辑）
├── MiniMES.Domain           // 领域层
├── MiniMES.Infrastructure   // 基础设施层
└── MiniMES.SharedKernel     // 公共组件
```

遵循 Clean Architecture：

```
Api
    ↓
Application
    ↓
Domain
    ↑
Infrastructure
```

依赖方向始终保持向内。

---

# 当前已完成（Sprint 2）

## 基础架构

* Clean Architecture
* CQRS（MediatR）
* PipelineBehavior
* 全局异常处理
* Result<T> 返回模型
* Error 常量体系
* Serilog 日志
* CorrelationId

---

## 身份认证（Authentication）

* JWT 登录
* Refresh Token
* Password Hash（BCrypt）
* 登录失败限制
* 用户锁定
* Token Rotation
* CurrentUser
* AuthenticationService

---

## 权限系统（Authorization）

* Role
* Permission
* UserRole
* RolePermission
* JWT Claims
* Policy 授权
* Permission Authorization Handler

---

## 数据访问

* Entity Framework Core
* SQL Server
* Migration
* DbContext
* Repository-Free（直接 DbContext）

---

# 当前目录结构

```
Application
│
├── Authentication
├── Common
└── Features

Infrastructure
│
├── Authentication
├── Authorization
└── Persistence
```

---

# 后续开发计划

## Sprint 3

* 用户管理
* 角色管理
* 权限管理
* 审计日志
* 操作日志
* 登录日志

---

## Sprint 4

MES 核心业务：

* 工厂
* 车间
* 产线
* 工位
* 产品
* BOM
* 工单

---

## Sprint 5

生产执行：

* 工单派工
* 开工
* 报工
* 完工
* 生产追溯

---

## Sprint 6

系统优化：

* Redis
* 缓存
* 分布式锁
* 性能优化
* Docker
* CI/CD

---

# 开发原则

整个项目坚持以下原则：

* Clean Architecture
* SOLID
* DDD
* CQRS
* 高内聚、低耦合
* 面向接口编程
* Repository-Free（EF Core）
* Result Pattern
* Pipeline Pattern
* 统一异常处理
* 统一日志
* 统一权限控制

---

# 项目定位

本项目旨在模拟真实企业 MES 系统的开发过程，重点关注：

* 架构设计
* 代码规范
* 可维护性
* 可扩展性
* 工业级开发实践

项目会持续迭代完善，并逐步引入更多企业级能力，如缓存、消息队列、审计日志、多租户、任务调度等。
