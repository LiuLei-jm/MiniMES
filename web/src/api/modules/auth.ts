import type { LoginCredentials, TokenPair } from '../../types'
import { request } from '../http'

export function loginRequest(credentials: LoginCredentials): Promise<TokenPair> {
  return request('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  })
}

export function refreshTokenRequest(refreshToken: string): Promise<TokenPair> {
  return request('/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken }),
  })
}
