import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import LoginView from '../views/auth/LoginView.vue'
import AdminLayout from '../layouts/AdminLayout.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', name: 'login', component: LoginView, meta: { publicOnly: true, title: '登录' } },
    { path: '/', component: AdminLayout, meta: { requiresAuth: true }, children: [
      { path: '', redirect: { name: 'overview' } },
      { path: 'overview', name: 'overview', component: () => import('../views/dashboard/OverviewView.vue'), meta: { title: '总览' } },
      { path: 'users', name: 'users', component: () => import('../views/system/users/UsersView.vue'), meta: { title: '用户管理', requiredPermission: 'users:view' } },
      { path: 'roles', name: 'roles', component: () => import('../views/system/roles/RolesView.vue'), meta: { title: '角色管理', requiredPermission: 'roles:view' } },
      { path: 'permissions', name: 'permissions', component: () => import('../views/system/permissions/PermissionsView.vue'), meta: { title: '权限管理', requiredPermission: 'permissions:view' } },
      { path: 'audit', name: 'audit', component: () => import('../views/audit/AuditView.vue'), meta: { title: '审计日志', requiredPermission: 'audit:view' } },
      { path: 'profile', name: 'profile', component: () => import('../views/profile/ProfileView.vue'), meta: { title: '个人资料' } },
    ] },
    { path: '/:pathMatch(.*)*', redirect: { name: 'overview' } },
  ],
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()
  if (to.meta.publicOnly && auth.session) return { name: 'overview' }
  if (to.meta.requiresAuth && !auth.session) return { name: 'login', query: { redirect: to.fullPath } }
  if (to.meta.requiresAuth && auth.session && Date.parse(auth.session.expiresAt) - Date.now() < 30_000) {
    try {
      await auth.refresh()
    } catch {
      return { name: 'login', query: { redirect: to.fullPath } }
    }
  }

  const requiredPermission = typeof to.meta.requiredPermission === 'string' ? to.meta.requiredPermission : undefined
  if (auth.session && !auth.hasPermission(requiredPermission)) return { name: 'overview' }

  document.title = `${String(to.meta.title || '管理后台')} · MiniMES`
  return true
})

export default router
