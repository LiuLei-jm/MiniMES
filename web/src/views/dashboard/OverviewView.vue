<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '../../stores/auth'

const auth = useAuthStore()
const user = computed(() => auth.session?.user)
const expiry = computed(() => auth.session?.expiresAt ? new Date(auth.session.expiresAt).toLocaleString('zh-CN') : '—')
</script>

<template>
  <div class="page-head"><span class="eyebrow">CONTROL CENTER / 控制台</span><h1>早上好，{{ user?.userName }}<span class="heading-accent">.</span></h1><p>欢迎回到 MiniMES 管理后台。以下是当前可用的功能。</p></div>
  <div class="overview-grid">
    <section class="surface welcome-panel"><div class="panel-icon">✦</div><div><span class="eyebrow">QUICK ACCESS</span><h2>身份与权限管理</h2><p>集中维护用户、角色和权限，所有写操作均由服务端校验权限并记录审计日志。</p><router-link class="text-link" to="/users">进入用户管理 <span>→</span></router-link></div></section>
    <section class="surface session-panel"><span class="eyebrow">CURRENT SESSION</span><h2>当前会话</h2><div class="session-row"><span>用户名</span><strong>{{ user?.userName }}</strong></div><div class="session-row"><span>角色</span><strong>{{ user?.roles.length ? user.roles.join('、') : '未分配' }}</strong></div><div class="session-row"><span>访问令牌过期</span><strong>{{ expiry }}</strong></div><div class="session-row"><span>令牌续期</span><strong class="good">自动启用</strong></div></section>
  </div>
  <section class="surface api-panel"><div class="section-title"><div><span class="eyebrow">SPRINT 3 CAPABILITIES</span><h2>管理功能</h2></div><span class="count-pill">04 个模块</span></div><div class="api-list"><div><span class="method">CRUD</span><code>/api/users</code><span>用户资料、状态与角色分配</span></div><div><span class="method">CRUD</span><code>/api/roles</code><span>角色维护与权限分配</span></div><div><span class="method">CRUD</span><code>/api/permissions</code><span>动态权限维护</span></div><div><span class="method">GET</span><code>/api/audit-logs</code><span>安全、操作与登录日志</span></div></div><p class="api-note">页面按当前账号的权限调用接口，未授权操作由 Web API 统一拒绝。</p></section>
</template>
