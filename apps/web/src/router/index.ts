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
  {
    path: '/register',
    name: 'register',
    component: () => import('../views/RegisterView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/admin/users',
    name: 'admin-users',
    component: () => import('../views/UserManagementView.vue'),
    meta: { requiresAuth: true, requiresManager: true }
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// Navigation guard for authentication and role-based access
router.beforeEach((to, _from) => {
  const authStore = useAuthStore()
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  const requiresManager = to.matched.some(record => record.meta.requiresManager)

  if (requiresAuth && !authStore.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }
  if (requiresManager && !authStore.isManager) {
    return { name: 'tasks' }
  }
  if (to.name === 'login' && authStore.isAuthenticated) {
    return { name: 'tasks' }
  }
})

export default router
