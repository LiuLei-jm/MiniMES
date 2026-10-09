export interface NavigationItem {
  label: string
  routeName: string
  symbol: string
  requiredPermission?: string
}

export const navigationItems: NavigationItem[] = [
  { label: '总览', routeName: 'overview', symbol: '▦' },
  { label: '用户管理', routeName: 'users', symbol: '♙', requiredPermission: 'users:view' },
  { label: '角色管理', routeName: 'roles', symbol: '◇', requiredPermission: 'roles:view' },
  { label: '权限管理', routeName: 'permissions', symbol: '⌘', requiredPermission: 'permissions:view' },
  { label: '审计日志', routeName: 'audit', symbol: '≡', requiredPermission: 'audit:view' },
  { label: '个人资料', routeName: 'profile', symbol: '◎' },
]
