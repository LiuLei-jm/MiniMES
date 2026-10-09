import type { PermissionSummary } from '../../types'
import { authorizedRequest } from '../authenticated'

export const getPermissions = (): Promise<PermissionSummary[]> => authorizedRequest('/api/permissions', { method: 'GET' })
export const createPermission = (code: string, description: string): Promise<PermissionSummary> => authorizedRequest('/api/permissions', {
  method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ code, description }),
})
export const updatePermission = (id: string, code: string, description: string): Promise<void> => authorizedRequest(`/api/permissions/${id}`, {
  method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ code, description }),
})
export const deletePermission = (id: string): Promise<void> => authorizedRequest(`/api/permissions/${id}`, { method: 'DELETE' })
export const assignRolePermissions = (roleId: string, permissionIds: string[]): Promise<void> => authorizedRequest(`/api/roles/${roleId}/permissions`, {
  method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ permissionIds }),
})
