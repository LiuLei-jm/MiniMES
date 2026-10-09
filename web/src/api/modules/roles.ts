import type { RoleSummary } from '../../types'
import { authorizedRequest } from '../authenticated'

export const getRoles = (): Promise<RoleSummary[]> => authorizedRequest('/api/roles', { method: 'GET' })

export const createRole = (name: string): Promise<{ id: string; name: string }> => authorizedRequest('/api/roles', {
  method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name }),
})

export const updateRole = (id: string, name: string): Promise<void> => authorizedRequest(`/api/roles/${id}`, {
  method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name }),
})

export const deleteRole = (id: string): Promise<void> => authorizedRequest(`/api/roles/${id}`, { method: 'DELETE' })

export const assignUserRoles = (userId: string, roleIds: string[]): Promise<void> => authorizedRequest(`/api/users/${userId}/roles`, {
  method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ roleIds }),
})
