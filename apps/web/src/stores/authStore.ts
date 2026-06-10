import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, LoginRequest, RegisterRequest } from '../types/user'
import { login as loginApi, register as registerApi, getCurrentUser } from '../services/api'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<User | null>(null)
  const token = ref<string | null>(localStorage.getItem('token'))
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const currentUser = computed(() => user.value)

  // Actions
  async function login(credentials: LoginRequest) {
    loading.value = true
    error.value = null
    try {
      const response = await loginApi(credentials)
      user.value = response.user
      token.value = response.token
      localStorage.setItem('token', response.token)
      localStorage.setItem('user', JSON.stringify(response.user))
      return response
    } catch (err: any) {
      error.value = err.message || 'Login failed'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function register(data: RegisterRequest) {
    loading.value = true
    error.value = null
    try {
      const response = await registerApi(data)
      return response
    } catch (err: any) {
      error.value = err.message || 'Registration failed'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchCurrentUser() {
    if (!token.value) return
    
    loading.value = true
    error.value = null
    try {
      const userData = await getCurrentUser()
      user.value = userData
      localStorage.setItem('user', JSON.stringify(userData))
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch user'
      logout()
    } finally {
      loading.value = false
    }
  }

  function logout() {
    user.value = null
    token.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  function initializeAuth() {
    const storedUser = localStorage.getItem('user')
    const storedToken = localStorage.getItem('token')
    
    if (storedToken && storedUser) {
      token.value = storedToken
      try {
        user.value = JSON.parse(storedUser)
      } catch {
        logout()
      }
    }
  }

  return {
    // State
    user,
    token,
    loading,
    error,
    // Getters
    isAuthenticated,
    currentUser,
    // Actions
    login,
    register,
    logout,
    fetchCurrentUser,
    initializeAuth,
  }
})
