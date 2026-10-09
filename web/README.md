# MiniMES 管理后台

基于 Vue 3、Element Plus、Pinia 和 Vite 的管理界面。前端已接入登录、令牌自动刷新、用户 CRUD 与资料、角色分配、角色 CRUD、动态权限分配、权限 CRUD 和审计日志接口。

## 项目结构

```text
src/
├─ api/          # HTTP 基础设施及按业务划分的接口模块
├─ composables/  # 会话续期等可复用组合式逻辑
├─ config/       # 菜单等应用配置
├─ layouts/      # 后台整体布局
├─ router/       # 路由表和访问守卫
├─ stores/       # Pinia 状态及认证动作
├─ types/        # 跨模块共享的 TypeScript 类型
├─ utils/        # 无状态工具和持久化适配
└─ views/        # 按 auth、dashboard、system 业务域组织的页面
```

新增功能时，在 `api/modules` 添加接口，在 `views` 下建立对应业务目录，并通过 `router/index.ts` 和 `config/navigation.ts` 接入页面与菜单。受保护请求统一使用 `authorizedRequest`，不要在页面中自行拼接 Authorization 请求头。

## 本地开发

```powershell
cd web
npm install
npm run dev
```

默认代理 `/api` 到 `https://localhost:7096`，直接连接 Web API 的 HTTPS 端点，避免重定向时丢失 Authorization 请求头。开发代理允许 ASP.NET 本地开发证书。若 API 运行在其他地址，将 `.env.example` 复制为 `.env` 并修改 `API_PROXY_TARGET`。直接从浏览器访问 API 时，前端地址还必须加入后端的 `Cors:AllowedOrigins`。API 启动时会执行数据库种子逻辑，请按后端说明自行启动。

```powershell
npm run test
npm run build
```

登录响应包含 `accessToken`、`refreshToken` 和 `expiresAt`。Pinia 会把会话与令牌中的用户信息保存到浏览器 `localStorage`，刷新页面后恢复。到期前或受保护请求返回 401 时调用 `/api/auth/refresh`，并保存轮换后的令牌；刷新失败则清除会话。使用共享浏览器时请及时退出登录。所有管理操作都通过共享的授权请求客户端发送，服务端进行最终权限校验并记录写操作审计日志。
