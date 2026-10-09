<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { createPermission, deletePermission, getPermissions, updatePermission } from '../../../api/modules/permissions'
import type { PermissionSummary } from '../../../types'

const permissions = ref<PermissionSummary[]>([])
const loading = ref(false)
const dialogVisible = ref(false)
const editingId = ref<string | null>(null)
const form = reactive({ code: '', description: '' })
async function load() { loading.value = true; try { permissions.value = await getPermissions() } catch (e) { ElMessage.error(e instanceof Error ? e.message : '权限加载失败') } finally { loading.value = false } }
function openCreate() { editingId.value = null; Object.assign(form, { code: '', description: '' }); dialogVisible.value = true }
function openEdit(row: unknown) { const item = row as PermissionSummary; editingId.value = item.id; Object.assign(form, { code: item.code, description: item.description }); dialogVisible.value = true }
async function save() {
  if (!form.code.trim() || !form.description.trim()) return ElMessage.warning('请填写权限编码和说明')
  loading.value = true
  try { if (editingId.value) await updatePermission(editingId.value, form.code, form.description); else await createPermission(form.code, form.description); ElMessage.success('权限已保存'); dialogVisible.value = false; await load() }
  catch (e) { ElMessage.error(e instanceof Error ? e.message : '权限保存失败') } finally { loading.value = false }
}
async function remove(row: unknown) { const item = row as PermissionSummary; await ElMessageBox.confirm(`确认删除权限“${item.code}”吗？`, '删除权限', { type: 'warning' }); try { await deletePermission(item.id); ElMessage.success('权限已删除'); await load() } catch (e) { ElMessage.error(e instanceof Error ? e.message : '权限删除失败') } }
onMounted(load)
</script>

<template>
  <div class="page-head"><span class="eyebrow">IDENTITY / 权限管理</span><h1>权限管理<span class="heading-accent">.</span></h1><p>维护策略权限编码，并通过角色管理完成动态授权。</p></div>
  <section class="surface data-panel">
    <div class="table-toolbar"><div class="toolbar-spacer"></div><el-button type="primary" @click="openCreate">新增权限</el-button></div>
    <el-table v-loading="loading" :data="permissions" stripe>
      <el-table-column prop="code" label="权限编码" min-width="220" />
      <el-table-column prop="description" label="说明" min-width="220" />
      <el-table-column prop="roleCount" label="关联角色" width="100" />
      <el-table-column label="操作" width="150"><template #default="{ row }"><el-button link type="primary" :disabled="row.code === 'system:admin'" @click="openEdit(row)">编辑</el-button><el-button link type="danger" :disabled="row.code === 'system:admin'" @click="remove(row)">删除</el-button></template></el-table-column>
    </el-table>
  </section>
  <el-dialog v-model="dialogVisible" :title="editingId ? '编辑权限' : '新增权限'" width="480px">
    <el-form label-position="top" @submit.prevent="save"><el-form-item label="权限编码" required><el-input v-model="form.code" placeholder="例如 quality:view" /></el-form-item><el-form-item label="权限说明" required><el-input v-model="form.description" maxlength="200" /></el-form-item></el-form>
    <template #footer><el-button @click="dialogVisible = false">取消</el-button><el-button type="primary" :loading="loading" @click="save">保存</el-button></template>
  </el-dialog>
</template>
