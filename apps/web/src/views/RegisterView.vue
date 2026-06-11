<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const fullName = ref('')
const acceptTerms = ref(false)

const showPassword = ref(false)
const showConfirmPassword = ref(false)
const registered = ref(false)

const fieldErrors = ref<Record<string, string>>({})

function validateField(field: string): string | undefined {
  switch (field) {
    case 'username':
      if (!username.value.trim()) return 'Username is required'
      if (username.value.trim().length < 3) return 'Username must be at least 3 characters'
      if (username.value.trim().length > 50) return 'Username must be 50 characters or less'
      if (!/^[a-zA-Z0-9_@.+\-]+$/.test(username.value.trim())) return 'Username can only contain letters, numbers, underscores, and email characters'
      return undefined
    case 'email':
      if (!email.value.trim()) return 'Email is required'
      if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value.trim())) return 'Please enter a valid email address'
      return undefined
    case 'password':
      if (!password.value) return 'Password is required'
      if (password.value.length < 8) return 'Password must be at least 8 characters'
      if (!/[A-Z]/.test(password.value)) return 'Password must contain at least one uppercase letter'
      if (!/[a-z]/.test(password.value)) return 'Password must contain at least one lowercase letter'
      if (!/[0-9]/.test(password.value)) return 'Password must contain at least one number'
      if (!/[^A-Za-z0-9]/.test(password.value)) return 'Password must contain at least one special character'
      return undefined
    case 'confirmPassword':
      if (!confirmPassword.value) return 'Please confirm your password'
      if (confirmPassword.value !== password.value) return 'Passwords do not match'
      return undefined
    case 'fullName':
      if (!fullName.value.trim()) return 'Full name is required'
      if (fullName.value.trim().length < 2) return 'Full name must be at least 2 characters'
      if (fullName.value.trim().length > 100) return 'Full name must be 100 characters or less'
      return undefined
    case 'acceptTerms':
      if (!acceptTerms.value) return 'You must accept the terms and conditions'
      return undefined
    default:
      return undefined
  }
}

function clearFieldError(field: string) {
  fieldErrors.value = { ...fieldErrors.value, [field]: '' }
  if (!fieldErrors.value[field]) {
    const { [field]: _, ...rest } = fieldErrors.value
    fieldErrors.value = rest
  }
}

function validateForm(): boolean {
  const errors: Record<string, string> = {}
  for (const field of ['username', 'email', 'password', 'confirmPassword', 'fullName', 'acceptTerms']) {
    const err = validateField(field)
    if (err) errors[field] = err
  }
  fieldErrors.value = errors
  return Object.keys(errors).length === 0
}

const passwordStrength = computed(() => {
  const pwd = password.value
  let score = 0
  if (pwd.length >= 8) score++
  if (pwd.length >= 12) score++
  if (/[A-Z]/.test(pwd)) score++
  if (/[a-z]/.test(pwd)) score++
  if (/[0-9]/.test(pwd)) score++
  if (/[^A-Za-z0-9]/.test(pwd)) score++
  if (score <= 2) return { label: 'Weak', class: 'bg-red-500', width: '33%' }
  if (score <= 4) return { label: 'Medium', class: 'bg-yellow-500', width: '66%' }
  return { label: 'Strong', class: 'bg-green-500', width: '100%' }
})

async function handleSubmit() {
  if (!validateForm() || authStore.loading) return

  try {
    await authStore.register({
      username: username.value.trim(),
      email: email.value.trim(),
      password: password.value,
      fullName: fullName.value.trim(),
    })
    registered.value = true
    setTimeout(() => {
      router.push('/login')
    }, 2000)
  } catch {
    // Error is set by authStore
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 px-4 py-12">
    <div class="card w-full max-w-md">
      <div v-if="registered" class="text-center py-8">
        <div class="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
          <svg class="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <h2 class="text-2xl font-bold text-gray-900 mb-2">Registration Successful!</h2>
        <p class="text-gray-600 mb-4">Your account has been created successfully.</p>
        <p class="text-sm text-gray-500">Redirecting to login page...</p>
        <div class="mt-4 w-full bg-gray-200 rounded-full h-2 overflow-hidden">
          <div class="bg-green-500 h-2 rounded-full animate-pulse"></div>
        </div>
      </div>

      <div v-else>
        <div class="text-center mb-8">
          <h1 class="text-3xl font-bold text-gray-900">Create Account</h1>
          <p class="text-gray-600 mt-2">Register to start managing your tasks</p>
        </div>

        <form @submit.prevent="handleSubmit" novalidate>
          <div class="space-y-4">
            <!-- Username -->
            <div>
              <label for="username" class="block text-sm font-medium text-gray-700 mb-1">
                Username <span class="text-red-500">*</span>
              </label>
              <input
                id="username" v-model="username" type="text" name="username" autocomplete="username"
                placeholder="johndoe" aria-describedby="username-error" autofocus
                :aria-invalid="fieldErrors.username ? 'true' : 'false'" class="input-field"
                :class="{ 'border-red-500 focus:ring-red-500': fieldErrors.username }"
                @input="clearFieldError('username')"
                @blur="fieldErrors.username = validateField('username') || ''"
              >
              <p v-if="fieldErrors.username" id="username-error" class="mt-1 text-sm text-red-600" role="alert">
                {{ fieldErrors.username }}
              </p>
            </div>

            <!-- Email -->
            <div>
              <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
                Email <span class="text-red-500">*</span>
              </label>
              <input
                id="email" v-model="email" type="email" name="email" autocomplete="email"
                placeholder="john.doe@example.com" aria-describedby="email-error"
                :aria-invalid="fieldErrors.email ? 'true' : 'false'" class="input-field"
                :class="{ 'border-red-500 focus:ring-red-500': fieldErrors.email }"
                @input="clearFieldError('email')"
                @blur="fieldErrors.email = validateField('email') || ''"
              >
              <p v-if="fieldErrors.email" id="email-error" class="mt-1 text-sm text-red-600" role="alert">
                {{ fieldErrors.email }}
              </p>
            </div>

            <!-- Full Name -->
            <div>
              <label for="fullName" class="block text-sm font-medium text-gray-700 mb-1">
                Full Name <span class="text-red-500">*</span>
              </label>
              <input
                id="fullName" v-model="fullName" type="text" name="fullName" autocomplete="name"
                placeholder="John Doe" aria-describedby="fullName-error"
                :aria-invalid="fieldErrors.fullName ? 'true' : 'false'" class="input-field"
                :class="{ 'border-red-500 focus:ring-red-500': fieldErrors.fullName }"
                @input="clearFieldError('fullName')"
                @blur="fieldErrors.fullName = validateField('fullName') || ''"
              >
              <p v-if="fieldErrors.fullName" id="fullName-error" class="mt-1 text-sm text-red-600" role="alert">
                {{ fieldErrors.fullName }}
              </p>
            </div>

            <!-- Password -->
            <div>
              <label for="password" class="block text-sm font-medium text-gray-700 mb-1">
                Password <span class="text-red-500">*</span>
              </label>
              <div class="relative">
                <input
                  id="password" v-model="password" :type="showPassword ? 'text' : 'password'"
                  name="password" autocomplete="new-password" placeholder="Create a strong password"
                  aria-describedby="password-error password-strength"
                  :aria-invalid="fieldErrors.password ? 'true' : 'false'" class="input-field pr-10"
                  :class="{ 'border-red-500 focus:ring-red-500': fieldErrors.password }"
                  @input="clearFieldError('password')"
                  @blur="fieldErrors.password = validateField('password') || ''"
                >
                <button
                  type="button" @click="showPassword = !showPassword"
                  class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                  :aria-label="showPassword ? 'Hide password' : 'Show password'"
                >
                  <svg v-if="!showPassword" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                  <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                  </svg>
                </button>
              </div>
              <div v-if="password" id="password-strength" class="mt-2">
                <div class="w-full bg-gray-200 rounded-full h-1.5 overflow-hidden">
                  <div
                    :class="passwordStrength.class"
                    :style="{ width: passwordStrength.width }"
                    class="h-1.5 rounded-full transition-all duration-300"
                  ></div>
                </div>
                <p class="text-xs text-gray-500 mt-1">
                  Strength: <span :class="{
                    'text-red-600': passwordStrength.label === 'Weak',
                    'text-yellow-600': passwordStrength.label === 'Medium',
                    'text-green-600': passwordStrength.label === 'Strong',
                  }" class="font-medium">{{ passwordStrength.label }}</span>
                </p>
              </div>
              <p v-if="fieldErrors.password" id="password-error" class="mt-1 text-sm text-red-600" role="alert">
                {{ fieldErrors.password }}
              </p>
            </div>

            <!-- Confirm Password -->
            <div>
              <label for="confirmPassword" class="block text-sm font-medium text-gray-700 mb-1">
                Confirm Password <span class="text-red-500">*</span>
              </label>
              <div class="relative">
                <input
                  id="confirmPassword" v-model="confirmPassword" :type="showConfirmPassword ? 'text' : 'password'"
                  name="confirmPassword" autocomplete="new-password" placeholder="Repeat your password"
                  aria-describedby="confirmPassword-error"
                  :aria-invalid="fieldErrors.confirmPassword ? 'true' : 'false'" class="input-field pr-10"
                  :class="{ 'border-red-500 focus:ring-red-500': fieldErrors.confirmPassword }"
                  @input="clearFieldError('confirmPassword')"
                  @blur="fieldErrors.confirmPassword = validateField('confirmPassword') || ''"
                >
                <button
                  type="button" @click="showConfirmPassword = !showConfirmPassword"
                  class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                  :aria-label="showConfirmPassword ? 'Hide confirm password' : 'Show confirm password'"
                >
                  <svg v-if="!showConfirmPassword" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                  <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                  </svg>
                </button>
              </div>
              <p v-if="fieldErrors.confirmPassword" id="confirmPassword-error" class="mt-1 text-sm text-red-600" role="alert">
                {{ fieldErrors.confirmPassword }}
              </p>
            </div>

            <!-- Terms Checkbox -->
            <div>
              <label class="flex items-start gap-2 cursor-pointer select-none">
                <input
                  v-model="acceptTerms" type="checkbox"
                  class="mt-0.5 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                  :class="{ 'border-red-500': fieldErrors.acceptTerms }"
                >
                <span class="text-sm text-gray-600">
                  I accept the
                  <a href="#" class="text-indigo-600 hover:text-indigo-500 font-medium" @click.prevent>Terms and Conditions</a>
                </span>
              </label>
              <p v-if="fieldErrors.acceptTerms" class="mt-1 text-sm text-red-600" role="alert">
                {{ fieldErrors.acceptTerms }}
              </p>
            </div>

            <!-- API Error -->
            <div
              v-if="authStore.error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm"
              role="alert"
            >
              {{ authStore.error }}
            </div>

            <!-- Submit Button -->
            <button
              type="submit" :disabled="authStore.loading"
              class="btn-primary w-full flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg
                v-if="authStore.loading" class="animate-spin h-5 w-5" xmlns="http://www.w3.org/2000/svg" fill="none"
                viewBox="0 0 24 24" aria-hidden="true"
              >
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
              </svg>
              {{ authStore.loading ? 'Creating Account...' : 'Create Account' }}
            </button>
          </div>
        </form>

        <p class="mt-6 text-center text-sm text-gray-500">
          Already have an account?
          <router-link to="/login" class="text-indigo-600 hover:text-indigo-500 font-medium">
            Sign in
          </router-link>
        </p>
      </div>
    </div>
  </div>
</template>
