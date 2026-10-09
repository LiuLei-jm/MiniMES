<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { createUser, disableUser, getUsers, updateUser } from '../../../api/modules/users'
import { assignUserRoles, getRoles } from '../../../api/modules/roles'
import { useAuthStore } from '../../../stores/auth'
import type { RoleSummary, UserSummary } from '../../../types'

const auth = useAuthStore()
const hasPermission = (code: string) => computed(() => auth.session?.user.permissions.includes(code) || auth.session?.user.permissions.includes('system:admin'))
const canCreate = hasPermission('users:create')
const canUpdate = hasPermission('users:update')
const canDelete = hasPermission('users:delete')
const canAssignRoles = hasPermission('roles:assign')
const loading = ref(false)
const users = ref<UserSummary[]>([])
const total = ref(0)
const keyword = ref('')
const page = ref(1)
const pageSize = 20
const dialogVisible = ref(false)
const roleDialogVisible = ref(false)
const roleTarget = ref<UserSummary | null>(null)
const selectedRoleIds = ref<string[]>([])
const availableRoles = ref<RoleSummary[]>([])
const editingId = ref<string | null>(null)
const formRef = ref<FormInstance>()
const form = reactive({ userName: '', password: '', confirm: '', isActive: true })
const rules: FormRules = {
  userName: [{ required: true, message: '请输入用户名', trigger: 'blur' }, { max: 30, message: '用户名最多 30 个字符', trigger: 'blur' }],
  password: [{ validator: (_rule, value, callback) => callback(editingId.value || value?.length >= 6 ? undefined : new Error('密码至少 6 位')), trigger: 'blur' }],
  confirm: [{ validator: (_rule, value, callback) => callback(editingId.value || value === form.password ? undefined : new Error('两次密码不一致')), trigger: 'blur' }],
}

async function loadUsers() {
  loading.value = true
  try { const result = await getUsers(keyword.value, page.value, pageSize); users.value = result.items; total.value = result.total }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '用户列表加载失败') }
  finally { loading.value = false }
}

function openCreate() { editingId.value = null; Object.assign(form, { userName: '', password: '', confirm: '', isActive: true }); dialogVisible.value = true }
function openEdit(user: UserSummary) { editingId.value = user.id; Object.assign(form, { userName: user.userName, password: '', confirm: '', isActive: user.isActive }); dialogVisible.value = true }
function editRow(row: unknown) { openEdit(row as UserSummary) }
async function openRoles(row: unknown) {
  roleTarget.value = row as UserSummary
  if (!availableRoles.value.length) availableRoles.value = await getRoles()
  selectedRoleIds.value = availableRoles.value.filter(role => roleTarget.value!.roles.includes(role.name)).map(role => role.id)
  roleDialogVisible.value = true
}
async function saveRoles() {
  if (!roleTarget.value) return
  loading.value = true
  try { await assignUserRoles(roleTarget.value.id, selectedRoleIds.value); ElMessage.success('角色分配成功'); roleDialogVisible.value = false; await loadUsers() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '角色分配失败') }
  finally { loading.value = false }
}

async function submit() {
  if (!await formRef.value?.validate().catch(() => false)) return
  loading.value = true
  try {
    if (editingId.value) await updateUser(editingId.value, { userName: form.userName.trim(), isActive: form.isActive })
    else await createUser({ userName: form.userName.trim(), password: form.password })
    ElMessage.success(editingId.value ? '用户更新成功' : '用户创建成功')
    dialogVisible.value = false
    await loadUsers()
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '保存失败') }
  finally { loading.value = false }
}

async function disable(user: UserSummary) {
  await ElMessageBox.confirm(`确认停用用户“${user.userName}”吗？`, '停用用户', { type: 'warning' })
  try { await disableUser(user.id); ElMessage.success('用户已停用'); await loadUsers() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '停用失败') }
}
function disableRow(row: unknown) { return disable(row as UserSummary) }

async function search() { page.value = 1; await loadUsers() }
async function changePage(value: number) { page.value = value; await loadUsers() }
onMounted(loadUsers)
</script>

<template>
  <div class="page-head"><span class="eyebrow">IDENTITY / 用户管理</span><h1>用户管理<span class="heading-accent">.</span></h1><p>查询、创建、编辑和停用系统用户。</p></div>
  <section class="surface data-panel">
    <div class="table-toolbar"><el-input v-model="keyword" clearable placeholder="按用户名搜索" @keyup.enter="search" /><el-button @click="search">查询</el-button><el-button type="primary" :disabled="!canCreate" @click="openCreate">新增用户</el-button></div>
    <el-table v-loading="loading" :data="users" stripe>
      <el-table-column prop="userName" label="用户名" min-width="160" />
      <el-table-column label="角色" min-width="180"><template #default="{ row }"><el-tag v-for="role in row.roles" :key="role" effect="plain">{{ role }}</el-tag><span v-if="!row.roles.length">—</span></template></el-table-column>
      <el-table-column label="状态" width="100"><template #default="{ row }"><el-tag :type="row.isActive ? 'success' : 'danger'">{{ row.isActive ? '正常' : '停用' }}</el-tag></template></el-table-column>
      <el-table-column label="最后登录" min-width="180"><template #default="{ row }">{{ row.lastLoginAt ? new Date(row.lastLoginAt).toLocaleString('zh-CN') : '从未登录' }}</template></el-table-column>
      <el-table-column label="操作" width="240" fixed="right"><template #default="{ row }"><el-button link type="primary" :disabled="!canAssignRoles" @click="openRoles(row)">分配角色</el-button><el-button link type="primary" :disabled="!canUpdate" @click="editRow(row)">编辑</el-button><el-button link type="danger" :disabled="!canDelete || !row.isActive" @click="disableRow(row)">停用</el-button></template></el-table-column>
    </el-table>
    <el-pagination v-if="total > pageSize" background layout="prev, pager, next" :total="total" :page-size="pageSize" :current-page="page" @current-change="changePage" />
  </section>
  <el-dialog v-model="dialogVisible" :title="editingId ? '编辑用户' : '新增用户'" width="480px" destroy-on-close>
    <el-form ref="formRef" :model="form" :rules="rules" label-position="top" @submit.prevent="submit">
      <el-form-item label="用户名" prop="userName"><el-input v-model="form.userName" maxlength="30" /></el-form-item>
      <template v-if="!editingId"><el-form-item label="密码" prop="password"><el-input v-model="form.password" type="password" show-password /></el-form-item><el-form-item label="确认密码" prop="confirm"><el-input v-model="form.confirm" type="password" show-password /></el-form-item></template>
      <el-form-item v-else label="账号状态"><el-switch v-model="form.isActive" active-text="启用" inactive-text="停用" /></el-form-item>
    </el-form>
    <template #footer><el-button @click="dialogVisible = false">取消</el-button><el-button type="primary" :loading="loading" @click="submit">保存</el-button></template>
  </el-dialog>
  <el-dialog v-model="roleDialogVisible" :title="`分配角色 · ${roleTarget?.userName || ''}`" width="480px">
    <el-select v-model="selectedRoleIds" multiple placeholder="请选择角色" style="width:100%"><el-option v-for="role in availableRoles" :key="role.id" :label="role.name" :value="role.id" /></el-select>
    <template #footer><el-button @click="roleDialogVisible = false">取消</el-button><el-button type="primary" :loading="loading" @click="saveRoles">保存</el-button></template>
  </el-dialog>
</template>
