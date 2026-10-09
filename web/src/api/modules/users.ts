import type { CreateUserRequest, CreateUserResponse, UpdateUserRequest, UserDetails, UserPage } from '../../types'
import { authorizedRequest } from '../authenticated'

export function createUser(payload: CreateUserRequest): Promise<CreateUserResponse> {
  return authorizedRequest('/api/users', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
}

export function getUsers(keyword = '', page = 1, pageSize = 20): Promise<UserPage> {
  const query = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
  if (keyword.trim()) query.set('keyword', keyword.trim())
  return authorizedRequest(`/api/users?${query}`, { method: 'GET' })
}

export function getUser(id: string): Promise<UserDetails> {
  return authorizedRequest(`/api/users/${id}`, { method: 'GET' })
}

export function getProfile(): Promise<UserDetails> {
  return authorizedRequest('/api/users/me', { method: 'GET' })
}

export function updateUser(id: string, payload: UpdateUserRequest): Promise<void> {
  return authorizedRequest(`/api/users/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
}

export function disableUser(id: string): Promise<void> {
  return authorizedRequest(`/api/users/${id}`, { method: 'DELETE' })
}
