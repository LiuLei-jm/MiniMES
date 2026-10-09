import { defineStore } from 'pinia'
import { ref } from 'vue'
import { loginRequest, refreshTokenRequest } from '../api/modules/auth'
import { makeSession, readSession, writeSession } from '../utils/session'
import type { LoginCredentials, Session, TokenPair } from '../types'

let refreshInFlight: Promise<void> | null = null

export const useAuthStore = defineStore('auth', () => {
  const session = ref<Session | null>(readSession())

  function setTokens(pair: TokenPair): void {
    session.value = makeSession(pair)
    writeSession(session.value)
  }

  function clear(): void {
    session.value = null
    writeSession(null)
  }

  function hasPermission(permission?: string): boolean {
    if (!permission) return true
    const permissions = session.value?.user.permissions ?? []
    return permissions.includes('system:admin') || permissions.includes(permission)
  }

  async function login(credentials: LoginCredentials): Promise<void> {
    setTokens(await loginRequest(credentials))
  }

  async function refresh(): Promise<void> {
    if (!session.value?.refreshToken) throw new Error('登录已过期，请重新登录')

    if (!refreshInFlight) {
      refreshInFlight = refreshTokenRequest(session.value.refreshToken)
        .then(setTokens)
        .catch((error) => {
          clear()
          throw error
        })
        .finally(() => { refreshInFlight = null })
    }

    return refreshInFlight
  }

  return { session, setTokens, login, refresh, clear, hasPermission }
})
