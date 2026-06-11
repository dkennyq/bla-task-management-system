import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, LoginRequest, RegisterRequest } from '../types/user'
import type { ApiError } from '../types/api'
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
      const loggedInUser: User = {
        id: response.userId,
        username: response.email,
        email: response.email,
        fullName: response.fullName,
        createdAt: '',
      }
      user.value = loggedInUser
      token.value = response.token
      localStorage.setItem('token', response.token)
      localStorage.setItem('user', JSON.stringify(loggedInUser))
      return response
    } catch (err: unknown) {
      const apiErr = err as ApiError
      const status = apiErr?.statusCode
      console.error('[AuthStore] Login failed:', status, apiErr?.message || apiErr)

      if (status === 401) {
        error.value = 'Invalid email or password.'
      } else if (status === 500) {
        error.value = 'Server error. Please try again later.'
      } else if (status === 0 || !status) {
        error.value = 'Network error. Please check your connection.'
      } else {
        error.value = 'Login failed. Please try again.'
      }
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
    } catch (err: unknown) {
      const apiErr = err as ApiError
      const status = apiErr?.statusCode
      console.error('[AuthStore] Register failed:', status, apiErr?.message || apiErr)

      if (status === 409) {
        error.value = apiErr?.message || 'Username or email already taken.'
      } else if (status === 400) {
        error.value = 'Invalid input. Please check your details.'
      } else if (status === 500) {
        error.value = 'Server error. Please try again later.'
      } else if (status === 0 || !status) {
        error.value = 'Network error. Please check your connection.'
      } else {
        error.value = apiErr?.message || 'Registration failed.'
      }
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
    } catch (err: unknown) {
      const apiErr = err as ApiError
      console.error('[AuthStore] Fetch user failed:', apiErr?.statusCode, apiErr?.message || apiErr)
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
