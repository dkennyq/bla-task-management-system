<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const rememberMe = ref(false)
const fieldErrors = ref<{ email?: string; password?: string }>({})

const rememberedEmail = localStorage.getItem('rememberedEmail')
if (rememberedEmail) {
  email.value = rememberedEmail
  rememberMe.value = true
}

const isLoading = computed(() => authStore.loading)
const authError = computed(() => authStore.error)

function validateField(field: 'email' | 'password'): string | undefined {
  if (field === 'email') {
    if (!email.value.trim()) return 'Email is required'
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value.trim())) return 'Please enter a valid email address'
  }
  if (field === 'password') {
    if (!password.value) return 'Password is required'
    if (password.value.length < 8) return 'Password must be at least 8 characters'
  }
  return undefined
}

function validateForm(): boolean {
  fieldErrors.value = {
    email: validateField('email'),
    password: validateField('password'),
  }
  return !fieldErrors.value.email && !fieldErrors.value.password
}

async function handleSubmit() {
  if (!validateForm()) return

  try {
    await authStore.login({ email: email.value.trim(), password: password.value })

    if (rememberMe.value) {
      localStorage.setItem('rememberedEmail', email.value.trim())
    } else {
      localStorage.removeItem('rememberedEmail')
    }

    const redirect = (route.query.redirect as string) || '/tasks'
    router.push(redirect)
  } catch {
    // Error is set by authStore
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 px-4 py-12">
    <div class="card w-full max-w-md">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900">Welcome Back</h1>
        <p class="text-gray-600 mt-2">Sign in to manage your tasks</p>
      </div>

      <form @submit.prevent="handleSubmit" novalidate>
        <div class="space-y-5">
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <input
              id="email" v-model="email" type="email" name="email" autocomplete="email"
              placeholder="you@example.com" aria-describedby="email-error"
              :aria-invalid="fieldErrors.email ? 'true' : 'false'" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': fieldErrors.email }" @input="fieldErrors.email = undefined"
              @blur="fieldErrors.email = validateField('email')"
            >
            <p v-if="fieldErrors.email" id="email-error" class="mt-1 text-sm text-red-600" role="alert">
              {{ fieldErrors.email }}
            </p>
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 mb-1">
              Password
            </label>
            <input
              id="password" v-model="password" type="password" name="password" autocomplete="current-password"
              placeholder="Enter your password" aria-describedby="password-error"
              :aria-invalid="fieldErrors.password ? 'true' : 'false'" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': fieldErrors.password }"
              @input="fieldErrors.password = undefined" @blur="fieldErrors.password = validateField('password')"
            >
            <p v-if="fieldErrors.password" id="password-error" class="mt-1 text-sm text-red-600" role="alert">
              {{ fieldErrors.password }}
            </p>
          </div>

          <div class="flex items-center justify-between">
            <label class="flex items-center gap-2 cursor-pointer select-none">
              <input
                v-model="rememberMe" type="checkbox"
                class="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
              >
              <span class="text-sm text-gray-600">Remember me</span>
            </label>
          </div>

          <div
            v-if="authError" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm"
            role="alert"
          >
            {{ authError }}
          </div>

          <button
            type="submit" :disabled="isLoading"
            class="btn-primary w-full flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <svg
              v-if="isLoading" class="animate-spin h-5 w-5" xmlns="http://www.w3.org/2000/svg" fill="none"
              viewBox="0 0 24 24" aria-hidden="true"
            >
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
            </svg>
            {{ isLoading ? 'Signing in...' : 'Sign In' }}
          </button>
        </div>
      </form>

      <p class="mt-6 text-center text-sm text-gray-500">
        Don't have an account?
        <router-link to="/register" class="text-indigo-600 hover:text-indigo-500 font-medium">
          Create one
        </router-link>
      </p>
    </div>
  </div>
</template>
