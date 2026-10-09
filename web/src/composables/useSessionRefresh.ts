import { onMounted, onUnmounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const refreshThresholdMs = 30_000

export function useSessionRefresh(): void {
  const auth = useAuthStore()
  const router = useRouter()
  let timer: ReturnType<typeof setTimeout> | undefined

  async function renewIfNeeded() {
    if (!auth.session || Date.parse(auth.session.expiresAt) - Date.now() > refreshThresholdMs) return
    try {
      await auth.refresh()
    } catch {
      await router.replace({ name: 'login' })
    }
  }

  watch(
    () => auth.session?.expiresAt,
    (expiresAt) => {
      if (timer) clearTimeout(timer)
      if (expiresAt) {
        timer = setTimeout(renewIfNeeded, Math.max(0, Date.parse(expiresAt) - Date.now() - refreshThresholdMs))
      }
    },
    { immediate: true },
  )

  const onVisibilityChange = () => {
    if (document.visibilityState === 'visible') void renewIfNeeded()
  }

  onMounted(() => document.addEventListener('visibilitychange', onVisibilityChange))
  onUnmounted(() => {
    if (timer) clearTimeout(timer)
    document.removeEventListener('visibilitychange', onVisibilityChange)
  })
}
