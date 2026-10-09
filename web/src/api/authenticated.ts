import { useAuthStore } from '../stores/auth'
import { ApiError, parseResponse } from './http'

const refreshThresholdMs = 30_000

export async function authorizedRequest<T>(path: string, init: RequestInit): Promise<T> {
  const auth = useAuthStore()
  if (!auth.session) throw new ApiError('请先登录', 401)

  if (Date.parse(auth.session.expiresAt) - Date.now() < refreshThresholdMs) {
    await auth.refresh()
  }

  const send = () => {
    const accessToken = auth.session?.accessToken
    if (!accessToken) throw new ApiError('登录已过期，请重新登录', 401)

    const headers: Record<string, string> = {
      ...Object.fromEntries(new Headers(init.headers).entries()),
      Authorization: `Bearer ${accessToken}`,
    }

    return fetch(path, { ...init, headers })
  }

  let response = await send()
  if (response.status === 401) {
    await auth.refresh()
    response = await send()
  }

  return parseResponse<T>(response)
}
