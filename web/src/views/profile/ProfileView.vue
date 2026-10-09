<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getProfile } from '../../api/modules/users'
import type { UserDetails } from '../../types'

const profile = ref<UserDetails | null>(null)
const loading = ref(false)
const formatDate = (value: string | null) => value ? new Date(value).toLocaleString('zh-CN') : '从未登录'

onMounted(async () => {
  loading.value = true
  try { profile.value = await getProfile() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '个人资料加载失败') }
  finally { loading.value = false }
})
</script>

<template>
  <div class="page-head"><span class="eyebrow">ACCOUNT / 个人资料</span><h1>个人资料<span class="heading-accent">.</span></h1><p>查看当前账号、角色和权限信息。</p></div>
  <section v-loading="loading" class="surface profile-panel">
    <template v-if="profile">
      <div class="profile-identity"><span class="avatar profile-avatar">{{ profile.userName.slice(0, 1).toUpperCase() }}</span><div><h2>{{ profile.userName }}</h2><el-tag :type="profile.isActive ? 'success' : 'danger'">{{ profile.isActive ? '正常' : '已停用' }}</el-tag></div></div>
      <el-descriptions :column="2" border>
        <el-descriptions-item label="用户 ID">{{ profile.id }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ formatDate(profile.createdTime) }}</el-descriptions-item>
        <el-descriptions-item label="最后登录">{{ formatDate(profile.lastLoginAt) }}</el-descriptions-item>
        <el-descriptions-item label="角色">{{ profile.roles.join('、') || '未分配' }}</el-descriptions-item>
      </el-descriptions>
      <div class="profile-permissions"><h3>有效权限</h3><div><el-tag v-for="permission in profile.permissions" :key="permission" effect="plain">{{ permission }}</el-tag><span v-if="!profile.permissions.length" class="muted">暂无权限</span></div></div>
    </template>
  </section>
</template>
