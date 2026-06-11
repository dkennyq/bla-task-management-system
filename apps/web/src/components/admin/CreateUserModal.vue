<script setup lang="ts">
import { ref, watch, nextTick, onMounted, onUnmounted } from 'vue'
import type { UserRole } from '../../types/user'

const props = defineProps<{
  isOpen: boolean
  onSave: (data: { username: string; email: string; password: string; fullName: string; role: UserRole }) => Promise<void>
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()

interface FormData {
  username: string
  email: string
  password: string
  fullName: string
  role: UserRole
}

const form = ref<FormData>({
  username: '',
  email: '',
  password: '',
  fullName: '',
  role: 'Operator',
})

const errors = ref<Record<string, string>>({})
const submitting = ref(false)
const submitError = ref<string | null>(null)
const emailInput = ref<HTMLInputElement | null>(null)

function initForm() {
  form.value = {
    username: '',
    email: '',
    password: '',
    fullName: '',
    role: 'Operator',
  }
  errors.value = {}
  submitError.value = null
}

function validate(): boolean {
  const newErrors: Record<string, string> = {}

  if (!form.value.username.trim()) {
    newErrors.username = 'Username is required'
  } else if (form.value.username.length < 3) {
    newErrors.username = 'Username must be at least 3 characters'
  }

  if (!form.value.email.trim()) {
    newErrors.email = 'Email is required'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)) {
    newErrors.email = 'Invalid email format'
  }

  if (!form.value.password) {
    newErrors.password = 'Password is required'
  } else if (form.value.password.length < 8) {
    newErrors.password = 'Password must be at least 8 characters'
  }

  if (!form.value.fullName.trim()) {
    newErrors.fullName = 'Full name is required'
  }

  errors.value = newErrors
  return Object.keys(newErrors).length === 0
}

function handleClose() {
  if (!submitting.value) {
    emit('close')
  }
}

async function handleSubmit() {
  if (!validate() || submitting.value) return

  submitting.value = true
  submitError.value = null

  try {
    await props.onSave({
      username: form.value.username.trim(),
      email: form.value.email.trim(),
      password: form.value.password,
      fullName: form.value.fullName.trim(),
      role: form.value.role,
    })
    initForm()
  } catch (err: unknown) {
    submitError.value = (err as { message?: string }).message || 'Failed to create user'
  } finally {
    submitting.value = false
  }
}

watch(() => props.isOpen, (open) => {
  if (open) {
    initForm()
    nextTick(() => {
      emailInput.value?.focus()
    })
  }
})

function onKeyDown(e: KeyboardEvent) {
  if (e.key === 'Escape' && props.isOpen) {
    handleClose()
  }
}

onMounted(() => {
  document.addEventListener('keydown', onKeyDown)
  if (props.isOpen) {
    initForm()
    nextTick(() => {
      emailInput.value?.focus()
    })
  }
})

onUnmounted(() => document.removeEventListener('keydown', onKeyDown))
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto">
    <div
      class="fixed inset-0 bg-black bg-opacity-50 transition-opacity" @click="handleClose"
      data-testid="modal-backdrop"
    ></div>
    <div class="flex min-h-full items-center justify-center p-4">
      <div
        class="relative w-full max-w-lg bg-white rounded-xl shadow-xl" role="dialog" aria-modal="true"
        aria-label="Create New User"
      >
        <div class="flex items-center justify-between px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-semibold text-gray-900">Create New User</h3>
          <button
            @click="handleClose" type="button" class="text-gray-400 hover:text-gray-600 transition-colors"
            aria-label="Close modal"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form @submit.prevent="handleSubmit" class="px-6 py-4 space-y-4">
          <div>
            <label for="create-user-username" class="block text-sm font-medium text-gray-700 mb-1">
              Username <span class="text-red-500">*</span>
            </label>
            <input
              id="create-user-username" v-model="form.username" type="text" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.username }"
              placeholder="Enter username"
            >
            <p v-if="errors.username" class="mt-1 text-sm text-red-600">{{ errors.username }}</p>
          </div>

          <div>
            <label for="create-user-email" class="block text-sm font-medium text-gray-700 mb-1">
              Email <span class="text-red-500">*</span>
            </label>
            <input
              id="create-user-email" ref="emailInput" v-model="form.email" type="email" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.email }"
              placeholder="Enter email address"
            >
            <p v-if="errors.email" class="mt-1 text-sm text-red-600">{{ errors.email }}</p>
          </div>

          <div>
            <label for="create-user-fullname" class="block text-sm font-medium text-gray-700 mb-1">
              Full Name <span class="text-red-500">*</span>
            </label>
            <input
              id="create-user-fullname" v-model="form.fullName" type="text" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.fullName }"
              placeholder="Enter full name"
            >
            <p v-if="errors.fullName" class="mt-1 text-sm text-red-600">{{ errors.fullName }}</p>
          </div>

          <div>
            <label for="create-user-password" class="block text-sm font-medium text-gray-700 mb-1">
              Password <span class="text-red-500">*</span>
            </label>
            <input
              id="create-user-password" v-model="form.password" type="password" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.password }"
              placeholder="Enter password"
            >
            <p v-if="errors.password" class="mt-1 text-sm text-red-600">{{ errors.password }}</p>
          </div>

          <div>
            <label for="create-user-role" class="block text-sm font-medium text-gray-700 mb-1">
              Role <span class="text-red-500">*</span>
            </label>
            <select
              id="create-user-role" v-model="form.role" class="input-field"
            >
              <option value="Operator">Operator</option>
              <option value="Manager">Manager</option>
            </select>
          </div>

          <div
            v-if="submitError" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm"
            role="alert"
          >
            {{ submitError }}
          </div>

          <div class="flex justify-end gap-3 pt-2 border-t border-gray-200">
            <button type="button" @click="handleClose" class="btn-secondary" :disabled="submitting">
              Cancel
            </button>
            <button type="submit" class="btn-primary inline-flex items-center gap-2" :disabled="submitting">
              <svg
                v-if="submitting" class="animate-spin -ml-1 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg"
                fill="none" viewBox="0 0 24 24" aria-hidden="true"
              >
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
              </svg>
              Create User
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
