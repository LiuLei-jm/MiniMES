import type { Session, TokenPair, UserInfo } from '../types'

const KEY = 'minimes.admin.session.v1'
const nameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

function strings(value: unknown): string[] {
  return Array.isArray(value) ? value.filter((item): item is string => typeof item === 'string') : typeof value === 'string' ? [value] : []
}

export function decodeUser(token: string): UserInfo {
  const payload = token.split('.')[1]
  if (!payload) throw new Error('无效的访问令牌')
  const binary = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
  const bytes = Uint8Array.from(binary, (char) => char.charCodeAt(0))
  const claims = JSON.parse(new TextDecoder().decode(bytes)) as Record<string, unknown>
  const id = claims.sub ?? claims.userId
  const userName = claims[nameClaim] ?? claims.unique_name ?? claims.name
  if (typeof id !== 'string' || typeof userName !== 'string') throw new Error('令牌缺少用户信息')
  return { id, userName, roles: strings(claims[roleClaim] ?? claims.role), permissions: strings(claims.permission) }
}

export function makeSession(pair: TokenPair): Session {
  if (!pair.accessToken || !pair.refreshToken || !Number.isFinite(Date.parse(pair.expiresAt))) throw new Error('服务器返回的令牌信息不完整')
  return { ...pair, user: decodeUser(pair.accessToken) }
}

export function readSession(): Session | null {
  try {
    const raw = localStorage.getItem(KEY)
    if (!raw) return null
    const data = JSON.parse(raw) as Session
    return makeSession(data)
  } catch {
    localStorage.removeItem(KEY)
    return null
  }
}

export function writeSession(session: Session | null): void {
  if (session) localStorage.setItem(KEY, JSON.stringify(session))
  else localStorage.removeItem(KEY)
}
