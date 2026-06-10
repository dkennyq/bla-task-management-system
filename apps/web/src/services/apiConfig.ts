import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig } from 'axios'
import type { ApiError } from '../types/api'

// API Base URLs
export const TASKS_API_URL = import.meta.env.VITE_TASKS_API_URL || 'http://localhost:5077/api'
export const USERS_API_URL = import.meta.env.VITE_USERS_API_URL || 'http://localhost:5078/api'

// Create Axios instances
export const tasksApiClient: AxiosInstance = axios.create({
  baseURL: TASKS_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const usersApiClient: AxiosInstance = axios.create({
  baseURL: USERS_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add JWT token
const requestInterceptor = (config: InternalAxiosRequestConfig) => {
  const token = localStorage.getItem('token')
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}

// Response error interceptor
const responseErrorInterceptor = (error: AxiosError) => {
  if (error.response) {
    const apiError: ApiError = {
      message: error.response.data as string || error.message,
      statusCode: error.response.status,
    }
    
    // Handle 401 Unauthorized
    if (error.response.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      // Redirect to login if needed
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
    }
    
    return Promise.reject(apiError)
  }
  
  return Promise.reject({
    message: error.message || 'Network error',
    statusCode: 0,
  } as ApiError)
}

// Apply interceptors to both clients
tasksApiClient.interceptors.request.use(requestInterceptor)
tasksApiClient.interceptors.response.use(undefined, responseErrorInterceptor)

usersApiClient.interceptors.request.use(requestInterceptor)
usersApiClient.interceptors.response.use(undefined, responseErrorInterceptor)
