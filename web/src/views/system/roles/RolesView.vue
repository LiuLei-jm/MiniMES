<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { createRole, deleteRole, getRoles, updateRole } from '../../../api/modules/roles'
import { assignRolePermissions, getPermissions } from '../../../api/modules/permissions'
import type { PermissionSummary, RoleSummary } from '../../../types'

const roles = ref<RoleSummary[]>([])
const loading = ref(false)
const dialogVisible = ref(false)
const editing = ref<RoleSummary | null>(null)
const roleName = ref('')
const permissionDialogVisible = ref(false)
const permissionTarget = ref<RoleSummary | null>(null)
const permissions = ref<PermissionSummary[]>([])
const selectedPermissionIds = ref<string[]>([])
const permissionSearch = ref('')
const visiblePermissions = computed(() => {
  const keyword = permissionSearch.value.trim().toLowerCase()
  if (!keyword) return permissions.value
  return permissions.value.filter(item =>
    item.code.toLowerCase().includes(keyword) || item.description.toLowerCase().includes(keyword))
})
const allVisibleSelected = computed(() =>
  visiblePermissions.value.length > 0 && visiblePermissions.value.every(item => selectedPermissionIds.value.includes(item.id)))
const someVisibleSelected = computed(() =>
  visiblePermissions.value.some(item => selectedPermissionIds.value.includes(item.id)) && !allVisibleSelected.value)

async function loadRoles() {
  loading.value = true
  try { roles.value = await getRoles() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '角色列表加载失败') }
  finally { loading.value = false }
}
function openCreate() { editing.value = null; roleName.value = ''; dialogVisible.value = true }
function openEdit(row: unknown) { editing.value = row as RoleSummary; roleName.value = editing.value.name; dialogVisible.value = true }
async function save() {
  if (!roleName.value.trim()) return ElMessage.warning('请输入角色名称')
  loading.value = true
  try {
    if (editing.value) await updateRole(editing.value.id, roleName.value.trim())
    else await createRole(roleName.value.trim())
    ElMessage.success('角色已保存'); dialogVisible.value = false; await loadRoles()
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '角色保存失败') }
  finally { loading.value = false }
}
async function remove(row: unknown) {
  const role = row as RoleSummary
  await ElMessageBox.confirm(`确认删除角色“${role.name}”吗？`, '删除角色', { type: 'warning' })
  try { await deleteRole(role.id); ElMessage.success('角色已删除'); await loadRoles() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '角色删除失败') }
}
async function openPermissions(row: unknown) {
  permissionTarget.value = row as RoleSummary
  permissionSearch.value = ''
  try {
    if (!permissions.value.length) permissions.value = await getPermissions()
    selectedPermissionIds.value = permissions.value.filter(item => permissionTarget.value!.permissions.includes(item.code)).map(item => item.id)
    permissionDialogVisible.value = true
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '权限列表加载失败')
  }
}
function toggleVisiblePermissions(value: boolean | string | number) {
  const visibleIds = visiblePermissions.value.map(item => item.id)
  if (Boolean(value)) {
    selectedPermissionIds.value = [...new Set([...selectedPermissionIds.value, ...visibleIds])]
  } else {
    const visibleIdSet = new Set(visibleIds)
    selectedPermissionIds.value = selectedPermissionIds.value.filter(id => !visibleIdSet.has(id))
  }
}
async function savePermissions() {
  if (!permissionTarget.value) return
  loading.value = true
  try { await assignRolePermissions(permissionTarget.value.id, selectedPermissionIds.value); ElMessage.success('角色权限已更新'); permissionDialogVisible.value = false; await loadRoles() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '权限分配失败') }
  finally { loading.value = false }
}
onMounted(loadRoles)
</script>

<template>
  <div class="page-head"><span class="eyebrow">IDENTITY / 角色管理</span><h1>角色管理<span class="heading-accent">.</span></h1><p>维护角色并查看用户与权限关联。</p></div>
  <section class="surface data-panel">
    <div class="table-toolbar"><div class="toolbar-spacer"></div><el-button type="primary" @click="openCreate">新增角色</el-button></div>
    <el-table v-loading="loading" :data="roles" stripe>
      <el-table-column prop="name" label="角色名称" min-width="180" />
      <el-table-column prop="userCount" label="用户数" width="100" />
      <el-table-column label="权限数" width="100"><template #default="{ row }">{{ row.permissions.length }}</template></el-table-column>
      <el-table-column label="权限" min-width="260"><template #default="{ row }"><el-tag v-for="permission in row.permissions.slice(0, 4)" :key="permission" effect="plain">{{ permission }}</el-tag><span v-if="row.permissions.length > 4">+{{ row.permissions.length - 4 }}</span></template></el-table-column>
      <el-table-column label="操作" width="230"><template #default="{ row }"><el-button link type="primary" :disabled="row.name === 'Admin'" @click="openPermissions(row)">配置权限</el-button><el-button link type="primary" :disabled="row.name === 'Admin'" @click="openEdit(row)">编辑</el-button><el-button link type="danger" :disabled="row.name === 'Admin'" @click="remove(row)">删除</el-button></template></el-table-column>
    </el-table>
  </section>
  <el-dialog v-model="dialogVisible" :title="editing ? '编辑角色' : '新增角色'" width="440px">
    <el-form label-position="top" @submit.prevent="save"><el-form-item label="角色名称" required><el-input v-model="roleName" maxlength="50" /></el-form-item></el-form>
    <template #footer><el-button @click="dialogVisible = false">取消</el-button><el-button type="primary" :loading="loading" @click="save">保存</el-button></template>
  </el-dialog>
  <el-dialog v-model="permissionDialogVisible" :title="`配置权限 · ${permissionTarget?.name || ''}`" width="720px">
    <div class="permission-toolbar">
      <el-input v-model="permissionSearch" clearable placeholder="搜索权限编码或说明" />
      <span>已选择 {{ selectedPermissionIds.length }} 项</span>
      <el-button link type="primary" @click="selectedPermissionIds = []">清空</el-button>
    </div>
    <div class="permission-select-all">
      <el-checkbox :model-value="allVisibleSelected" :indeterminate="someVisibleSelected" @change="toggleVisiblePermissions">全选当前结果（{{ visiblePermissions.length }}）</el-checkbox>
    </div>
    <el-checkbox-group v-model="selectedPermissionIds" class="permission-grid">
      <el-checkbox v-for="permission in visiblePermissions" :key="permission.id" :value="permission.id" class="permission-option">
        <span class="permission-code">{{ permission.code }}</span>
        <span class="permission-description">{{ permission.description }}</span>
      </el-checkbox>
    </el-checkbox-group>
    <el-alert v-if="!visiblePermissions.length" title="没有匹配的权限" type="info" :closable="false" show-icon />
    <template #footer><el-button @click="permissionDialogVisible = false">取消</el-button><el-button type="primary" :loading="loading" @click="savePermissions">保存</el-button></template>
  </el-dialog>
</template>

<style scoped>
.permission-toolbar{display:flex;align-items:center;gap:12px;margin-bottom:14px}.permission-toolbar .el-input{flex:1}.permission-toolbar span{white-space:nowrap;color:var(--el-text-color-secondary);font-size:13px}.permission-select-all{padding:10px 14px;border:1px solid var(--el-border-color);border-radius:8px 8px 0 0;background:var(--el-fill-color-light)}.permission-grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:0;max-height:380px;overflow:auto;border:1px solid var(--el-border-color);border-top:0;border-radius:0 0 8px 8px}.permission-option{height:auto;margin:0;padding:13px 14px;border-bottom:1px solid var(--el-border-color-lighter)}.permission-option:nth-child(odd){border-right:1px solid var(--el-border-color-lighter)}.permission-option :deep(.el-checkbox__label){display:flex;min-width:0;flex-direction:column;line-height:1.45}.permission-code{font-weight:600;color:var(--el-text-color-primary)}.permission-description{overflow:hidden;color:var(--el-text-color-secondary);font-size:12px;text-overflow:ellipsis;white-space:nowrap}@media(max-width:640px){.permission-grid{grid-template-columns:1fr}.permission-option:nth-child(odd){border-right:0}.permission-toolbar{align-items:flex-start;flex-wrap:wrap}.permission-toolbar .el-input{flex-basis:100%}}
</style>
