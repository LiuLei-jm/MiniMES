import type { AuditLogPage } from '../../types'
import { authorizedRequest } from '../authenticated'

export function getAuditLogs(category = '', keyword = '', page = 1, pageSize = 20): Promise<AuditLogPage> {
  const query = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
  if (category) query.set('category', category)
  if (keyword.trim()) query.set('keyword', keyword.trim())
  return authorizedRequest(`/api/audit-logs?${query}`, { method: 'GET' })
}
