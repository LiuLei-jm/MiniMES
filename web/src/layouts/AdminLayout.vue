<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { navigationItems } from '../config/navigation'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const user = computed(() => auth.session?.user)
const visibleNavigationItems = computed(() =>
  navigationItems.filter(item => auth.hasPermission(item.requiredPermission)))

function logout() {
  auth.clear()
  router.replace({ name: 'login' })
}
</script>

<template>
  <div class="admin-shell">
    <aside class="sidebar">
      <div class="sidebar-brand"><span class="brand-icon">M</span><span>MiniMES<span class="brand-dot">.</span><small>ADMIN CONSOLE</small></span></div>
      <div class="nav-caption">工作空间</div>
      <nav class="sidebar-nav" aria-label="主导航">
        <router-link v-for="item in visibleNavigationItems" :key="item.routeName" :to="{ name: item.routeName }" :class="{ active: route.name === item.routeName }">
          <span class="nav-symbol">{{ item.symbol }}</span> {{ item.label }}
        </router-link>
      </nav>
      <div class="sidebar-bottom">
        <div class="sidebar-status"><span class="status-dot"></span> 系统连接由 API 提供</div>
        <button class="sidebar-account" @click="logout"><span class="avatar">{{ user?.userName?.slice(0, 1).toUpperCase() }}</span><span><strong>{{ user?.userName }}</strong><small>退出登录</small></span><span class="logout-arrow">↗</span></button>
      </div>
    </aside>
    <div class="workspace">
      <header class="topbar"><div class="breadcrumb">工作空间 <span>/</span> {{ route.meta.title }}</div><div class="topbar-right"><span class="status-dot"></span> 已登录 <span class="topbar-divider"></span><span class="avatar small">{{ user?.userName?.slice(0, 1).toUpperCase() }}</span>{{ user?.userName }}</div></header>
      <main class="workspace-content"><router-view /></main>
    </div>
  </div>
</template>
