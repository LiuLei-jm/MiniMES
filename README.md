
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
* SQLite
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

# 当前已完成（Sprint 3）

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

## 身份管理与审计

* 用户资料、状态及角色分配
* 角色与动态权限管理
* 安全、操作及登录审计日志
* Vue 3 + Element Plus + Pinia 管理后台

---

## 数据访问

* Entity Framework Core
* SQLite
* 开发环境模型自动建库
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

# 开发数据库

开发环境默认使用 SQLite，配置位于 `src/MiniMES.Api/appsettings.Development.json`：

```json
{
  "DatabaseProvider": "Sqlite",
  "ConnectionStrings": { "Default": "Data Source=minimes-dev.db" }
}
```

首次启动 API 时会自动创建 `minimes-dev.db` 并执行开发种子数据。SQLite 使用模型建库；模型发生变化时，删除该开发库后重新启动即可重建（会清空本地开发数据）。Sprint 3 新增了 `AuditLogs` 表，已有开发库需要按此方式重建。仓库中的旧 SQL Server 迁移仅作为历史文件保留，当前应用配置不会执行它们。

开发和测试环境未配置 `Jwt:Key` 时会在进程启动时生成临时签名密钥，重启后已有访问令牌会失效。生产环境必须通过环境变量 `Jwt__Key` 或其他安全配置源提供至少 32 字节的密钥。

开发数据库首次启动时会创建默认管理员 `admin`，并将一次性密码输出到控制台。若需要固定的本地密码，请在首次启动前设置环境变量 `DataSeed__Admin__Password`；种子逻辑不会覆盖已存在管理员的密码。管理员会自动绑定 `Admin` 角色、加载全部已定义权限，并通过 `system:admin` 权限访问后续新增的权限端点。

开发环境允许 `http://localhost:5173`、`http://127.0.0.1:5173` 以及 Vite Preview 的 `4173` 端口跨域访问 API。其他前端地址可通过 `Cors__AllowedOrigins__0`、`Cors__AllowedOrigins__1` 等环境变量配置。生产环境未配置来源时默认拒绝跨域请求。

---

# 项目定位

本项目旨在模拟真实企业 MES 系统的开发过程，重点关注：

* 架构设计
* 代码规范
* 可维护性
* 可扩展性
* 工业级开发实践

项目会持续迭代完善，并逐步引入更多企业级能力，如缓存、消息队列、审计日志、多租户、任务调度等。
