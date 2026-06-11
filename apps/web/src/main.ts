import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import './style.css'
import App from './App.vue'
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(Toast, { timeout: 3000 })

// Initialize auth state from localStorage
import { useAuthStore } from './stores/authStore'
const authStore = useAuthStore()
authStore.initializeAuth()

app.mount('#app')

