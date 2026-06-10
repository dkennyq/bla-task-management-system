import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import HomeView from '../views/HomeView.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: HomeView,
    meta: { requiresAuth: false }
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('../views/LoginView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/tasks',
    name: 'tasks',
    component: () => import('../views/TasksView.vue'),
    meta: { requiresAuth: true }
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// Navigation guard for authentication
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)

  if (requiresAuth && !authStore.isAuthenticated) {
    // Redirect to login if trying to access protected route
    next({ name: 'login', query: { redirect: to.fullPath } })
  } else if (to.name === 'login' && authStore.isAuthenticated) {
    // Redirect to tasks if already logged in and trying to access login
    next({ name: 'tasks' })
  } else {
    next()
  }
})

export default router
