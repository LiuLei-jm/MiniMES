export interface TokenPair {
  accessToken: string
  refreshToken: string
  expiresAt: string
}

export interface LoginCredentials {
  userName: string
  password: string
}

export interface UserInfo {
  id: string
  userName: string
  roles: string[]
  permissions: string[]
}

export interface Session extends TokenPair {
  user: UserInfo
}

export interface CreateUserResponse {
  id: string
  userName: string
}

export interface CreateUserRequest {
  userName: string
  password: string
}

export interface UserSummary {
  id: string
  userName: string
  isActive: boolean
  createdTime: string
  lastLoginAt: string | null
  roles: string[]
}

export interface UserDetails extends UserSummary {
  permissions: string[]
}

export interface UserPage {
  items: UserSummary[]
  total: number
  page: number
  pageSize: number
}

export interface UpdateUserRequest {
  userName: string
  isActive: boolean
}

export interface RoleSummary {
  id: string
  name: string
  userCount: number
  permissions: string[]
}

export interface PermissionSummary {
  id: string
  code: string
  description: string
  roleCount: number
}

export interface AuditLogItem {
  id: string
  category: 'Security' | 'Operation' | 'Login' | string
  action: string
  success: boolean
  userId: string | null
  userName: string | null
  targetType: string | null
  targetId: string | null
  ipAddress: string | null
  traceId: string | null
  details: string | null
  occurredAt: string
}

export interface AuditLogPage {
  items: AuditLogItem[]
  total: number
  page: number
  pageSize: number
}
