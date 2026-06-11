<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue'
import type { UserListItem, UserRole } from '../../types/user'

const props = defineProps<{
  isOpen: boolean
  user: UserListItem
  onSave: (userId: string, role: UserRole) => Promise<void>
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()

const selectedRole = ref<UserRole>('Operator')
const submitting = ref(false)
const submitError = ref<string | null>(null)

function initForm() {
  selectedRole.value = props.user.role
  submitError.value = null
}

function handleClose() {
  if (!submitting.value) {
    emit('close')
  }
}

async function handleSubmit() {
  if (submitting.value) return

  submitting.value = true
  submitError.value = null

  try {
    await props.onSave(props.user.id, selectedRole.value)
    submitError.value = null
  } catch (err: unknown) {
    submitError.value = (err as { message?: string }).message || 'Failed to update role'
  } finally {
    submitting.value = false
  }
}

watch(() => props.isOpen, (open) => {
  if (open) {
    initForm()
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
        class="relative w-full max-w-md bg-white rounded-xl shadow-xl" role="dialog" aria-modal="true"
        aria-label="Edit User Role"
      >
        <div class="flex items-center justify-between px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-semibold text-gray-900">Edit User Role</h3>
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
            <label class="block text-sm font-medium text-gray-700 mb-1">User</label>
            <p class="text-sm text-gray-900">{{ user.email }}</p>
          </div>

          <div>
            <label for="edit-role-select" class="block text-sm font-medium text-gray-700 mb-1">
              Role <span class="text-red-500">*</span>
            </label>
            <select
              id="edit-role-select" v-model="selectedRole" class="input-field"
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
              Save Changes
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
