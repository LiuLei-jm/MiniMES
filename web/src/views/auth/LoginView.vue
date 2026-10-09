<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()
const form = reactive({ userName: '', password: '' })
const formRef = ref<FormInstance>()
const loading = ref(false)
const error = ref('')
const rules: FormRules = {
  userName: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
}

async function submit() {
  if (!await formRef.value?.validate().catch(() => false)) return
  loading.value = true
  error.value = ''
  try {
    await auth.login({ userName: form.userName.trim(), password: form.password })
    ElMessage.success('登录成功')
    const redirect = typeof route.query.redirect === 'string' && route.query.redirect.startsWith('/')
      ? route.query.redirect
      : '/overview'
    await router.replace(redirect)
  } catch (e) {
    error.value = e instanceof Error ? e.message : '登录失败，请稍后重试'
  } finally { loading.value = false }
}
</script>

<template>
  <main class="login-page">
    <section class="login-intro">
      <div class="brand"><span class="brand-icon">M</span><span>MiniMES<span class="brand-dot">.</span></span></div>
      <div class="intro-content">
        <div class="eyebrow light">MANUFACTURING EXECUTION SYSTEM</div>
        <h1>让生产管理，<br /><em>有序发生。</em></h1>
        <p>从身份认证到用户管理，以清晰、安全的方式连接每一个生产环节。</p>
        <div class="intro-line"><span></span><span>MINIMES · ADMIN CONSOLE</span></div>
      </div>
      <div class="intro-footer">© {{ new Date().getFullYear() }} MiniMES</div>
      <div class="decor decor-one"></div><div class="decor decor-two"></div>
    </section>
    <section class="login-form-side">
      <div class="mobile-brand"><span class="brand-icon">M</span> MiniMES</div>
      <div class="login-card">
        <span class="eyebrow">欢迎回来 / WELCOME BACK</span>
        <h2>登录管理后台</h2>
        <p class="muted">请输入您的账号信息以继续访问。</p>
        <el-alert v-if="error" :title="error" type="error" show-icon :closable="false" class="form-error" />
        <el-form ref="formRef" :model="form" :rules="rules" label-position="top" @submit.prevent="submit">
          <el-form-item label="用户名" prop="userName">
            <el-input v-model="form.userName" size="large" placeholder="请输入用户名" autocomplete="username" />
          </el-form-item>
          <el-form-item label="密码" prop="password">
            <el-input v-model="form.password" size="large" type="password" show-password placeholder="请输入密码" autocomplete="current-password" />
          </el-form-item>
          <el-button class="submit-button" type="primary" size="large" native-type="submit" :loading="loading" :disabled="loading" aria-label="登录 MiniMES 管理后台">
            <span>{{ loading ? '正在登录' : '登录系统' }}</span><span v-if="!loading">→</span>
          </el-button>
        </el-form>
        <p class="login-note">仅限已授权的 MiniMES 用户使用</p>
      </div>
      <div class="form-side-footer">安全访问 · 权限控制 · 会话自动续期</div>
    </section>
  </main>
</template>
