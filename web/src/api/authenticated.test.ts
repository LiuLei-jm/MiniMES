import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { createUser } from './modules/users'
import { useAuthStore } from '../stores/auth'
import type { TokenPair } from '../types'

const storage = new Map<string, string>()
vi.stubGlobal('localStorage', {
  getItem: (key: string) => storage.get(key) ?? null,
  setItem: (key: string, value: string) => storage.set(key, value),
  removeItem: (key: string) => storage.delete(key),
})

function pair(accessToken: string, refreshToken: string, expiresAt: string): TokenPair {
  return { accessToken, refreshToken, expiresAt }
}

function token(userName = 'admin'): string {
  const claims = { sub: 'user-1', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': userName, permission: 'users:create' }
  return `header.${btoa(JSON.stringify(claims))}.signature`
}

function ok(body: object): Response { return new Response(JSON.stringify(body), { status: 200, headers: { 'Content-Type': 'application/json' } }) }

beforeEach(() => { storage.clear(); setActivePinia(createPinia()); vi.unstubAllGlobals(); vi.stubGlobal('localStorage', {
  getItem: (key: string) => storage.get(key) ?? null,
  setItem: (key: string, value: string) => storage.set(key, value),
  removeItem: (key: string) => storage.delete(key),
}) })

describe('session renewal', () => {
  it('adds the current access token to the first protected request', async () => {
    const auth = useAuthStore()
    const accessToken = token()
    auth.setTokens(pair(accessToken, 'refresh-token', new Date(Date.now() + 3600_000).toISOString()))
    const fetchMock = vi.fn().mockResolvedValue(ok({ id: 'created-1', userName: 'operator' }))
    vi.stubGlobal('fetch', fetchMock)

    await createUser({ userName: 'operator', password: 'password' })

    expect(fetchMock).toHaveBeenCalledTimes(1)
    expect(fetchMock.mock.calls[0]?.[0]).toBe('/api/users')
    expect(fetchMock.mock.calls[0]?.[1]?.headers).toMatchObject({
      Authorization: `Bearer ${accessToken}`,
      'content-type': 'application/json',
    })
  })

  it('shares one refresh request and persists the rotated token', async () => {
    const auth = useAuthStore()
    auth.setTokens(pair(token(), 'old-refresh', new Date(Date.now() - 1000).toISOString()))
    const fetchMock = vi.fn().mockResolvedValue(ok(pair(token(), 'new-refresh', new Date(Date.now() + 3600_000).toISOString())))
    vi.stubGlobal('fetch', fetchMock)
    await Promise.all([auth.refresh(), auth.refresh()])
    expect(fetchMock).toHaveBeenCalledTimes(1)
    expect(auth.session?.refreshToken).toBe('new-refresh')
    expect(storage.get('minimes.admin.session.v1')).toContain('new-refresh')
  })

  it('refreshes expired credentials before a protected request', async () => {
    const auth = useAuthStore()
    auth.setTokens(pair(token(), 'old-refresh', new Date(Date.now() - 1000).toISOString()))
    const fetchMock = vi.fn()
      .mockResolvedValueOnce(ok(pair(token(), 'new-refresh', new Date(Date.now() + 3600_000).toISOString())))
      .mockResolvedValueOnce(ok({ id: 'created-1', userName: 'operator' }))
    vi.stubGlobal('fetch', fetchMock)
    await expect(createUser({ userName: 'operator', password: 'password' })).resolves.toEqual({ id: 'created-1', userName: 'operator' })
    expect(fetchMock.mock.calls[0]?.[0]).toBe('/api/auth/refresh')
    expect(new Headers(fetchMock.mock.calls[1]?.[1]?.headers).get('Authorization')).toBe(`Bearer ${token()}`)
  })

  it('clears persisted credentials when refresh is rejected', async () => {
    const auth = useAuthStore()
    auth.setTokens(pair(token(), 'expired', new Date(Date.now() - 1000).toISOString()))
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(JSON.stringify({ title: 'Invalid token' }), { status: 401 })))
    await expect(auth.refresh()).rejects.toThrow('Invalid token')
    expect(auth.session).toBeNull()
    expect(storage.size).toBe(0)
  })

  it('retries a protected request once after a 401 response', async () => {
    const auth = useAuthStore()
    auth.setTokens(pair(token(), 'first-refresh', new Date(Date.now() + 3600_000).toISOString()))
    const fetchMock = vi.fn()
      .mockResolvedValueOnce(new Response(null, { status: 401 }))
      .mockResolvedValueOnce(ok(pair(token('admin'), 'second-refresh', new Date(Date.now() + 3600_000).toISOString())))
      .mockResolvedValueOnce(ok({ id: 'new-id', userName: 'operator' }))
    vi.stubGlobal('fetch', fetchMock)
    await expect(createUser({ userName: 'operator', password: 'password' })).resolves.toEqual({ id: 'new-id', userName: 'operator' })
    expect(fetchMock.mock.calls.map((call) => call[0])).toEqual(['/api/users', '/api/auth/refresh', '/api/users'])
    expect(auth.session?.refreshToken).toBe('second-refresh')
  })
})
