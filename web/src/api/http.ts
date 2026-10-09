export class ApiError extends Error {
  constructor(message: string, public status: number, public code?: string) {
    super(message)
    this.name = 'ApiError'
  }
}

export async function parseResponse<T>(response: Response): Promise<T> {
  const body = await response.json().catch(() => null)

  if (!response.ok) {
    const validationErrors = body?.errors && typeof body.errors === 'object'
      ? Object.values(body.errors).flat().join('；')
      : ''

    throw new ApiError(
      validationErrors || body?.detail || body?.title || `请求失败 (${response.status})`,
      response.status,
      body?.code,
    )
  }

  return body as T
}

export async function request<T>(path: string, init: RequestInit): Promise<T> {
  return parseResponse<T>(await fetch(path, init))
}
