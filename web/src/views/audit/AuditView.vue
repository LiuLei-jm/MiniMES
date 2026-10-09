<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getAuditLogs } from '../../api/modules/audit'
import type { AuditLogItem } from '../../types'

const logs = ref<AuditLogItem[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = 20
const category = ref('')
const keyword = ref('')
const loading = ref(false)
const categories = [{ label: '全部日志', value: '' }, { label: '安全审计', value: 'Security' }, { label: '操作日志', value: 'Operation' }, { label: '登录日志', value: 'Login' }]

async function load() {
  loading.value = true
  try { const result = await getAuditLogs(category.value, keyword.value, page.value, pageSize); logs.value = result.items; total.value = result.total }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '审计日志加载失败') }
  finally { loading.value = false }
}
async function search() { page.value = 1; await load() }
async function changePage(value: number) { page.value = value; await load() }
const categoryLabel = (value: string) => categories.find(x => x.value === value)?.label || value
onMounted(load)
</script>

<template>
  <div class="page-head"><span class="eyebrow">SECURITY / 审计中心</span><h1>审计日志<span class="heading-accent">.</span></h1><p>集中查看安全事件、管理操作与登录记录。</p></div>
  <section class="surface data-panel">
    <div class="table-toolbar"><el-select v-model="category" style="width:150px" @change="search"><el-option v-for="item in categories" :key="item.value" :label="item.label" :value="item.value" /></el-select><el-input v-model="keyword" clearable placeholder="用户、动作或目标" @keyup.enter="search" /><el-button @click="search">查询</el-button></div>
    <el-table v-loading="loading" :data="logs" stripe>
      <el-table-column label="时间" width="180"><template #default="{ row }">{{ new Date(row.occurredAt).toLocaleString('zh-CN') }}</template></el-table-column>
      <el-table-column label="分类" width="110"><template #default="{ row }">{{ categoryLabel(row.category) }}</template></el-table-column>
      <el-table-column prop="action" label="动作" min-width="160" />
      <el-table-column label="结果" width="90"><template #default="{ row }"><el-tag :type="row.success ? 'success' : 'danger'">{{ row.success ? '成功' : '失败' }}</el-tag></template></el-table-column>
      <el-table-column label="用户" min-width="130"><template #default="{ row }">{{ row.userName || row.userId || '匿名' }}</template></el-table-column>
      <el-table-column label="目标" min-width="190"><template #default="{ row }">{{ row.targetType ? `${row.targetType} · ${row.targetId || '—'}` : '—' }}</template></el-table-column>
      <el-table-column prop="ipAddress" label="IP" min-width="130" />
      <el-table-column prop="details" label="详情" min-width="150" />
    </el-table>
    <el-pagination v-if="total > pageSize" background layout="prev, pager, next" :total="total" :page-size="pageSize" :current-page="page" @current-change="changePage" />
  </section>
</template>
