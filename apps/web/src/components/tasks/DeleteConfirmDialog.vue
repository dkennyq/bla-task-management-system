<script setup lang="ts">
import { watch, nextTick, onMounted, onUnmounted, ref } from 'vue'

const props = defineProps<{
  isOpen: boolean
  taskTitle: string
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'confirm'): void
  (e: 'cancel'): void
}>()

const deleteBtnRef = ref<HTMLButtonElement | null>(null)

function handleCancel() {
  if (props.loading) return
  emit('cancel')
}

function handleConfirm() {
  if (props.loading) return
  emit('confirm')
}

function onKeyDown(e: KeyboardEvent) {
  if (e.key === 'Escape' && props.isOpen) {
    handleCancel()
  }
}

watch(() => props.isOpen, (open) => {
  if (open) {
    nextTick(() => {
      deleteBtnRef.value?.focus()
    })
  }
})

onMounted(() => {
  document.addEventListener('keydown', onKeyDown)
  if (props.isOpen) {
    nextTick(() => {
      deleteBtnRef.value?.focus()
    })
  }
})

onUnmounted(() => document.removeEventListener('keydown', onKeyDown))
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto">
    <div
      class="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
      @click="handleCancel"
      data-testid="modal-backdrop"
    ></div>
    <div class="flex min-h-full items-center justify-center p-4">
      <div
        class="relative w-full max-w-md bg-white rounded-xl shadow-xl" role="dialog" aria-modal="true"
        aria-label="Delete Task"
      >
        <div class="flex items-center justify-between px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-semibold text-gray-900">Delete Task</h3>
          <button
            @click="handleCancel" type="button" class="text-gray-400 hover:text-gray-600 transition-colors"
            aria-label="Close modal"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div class="px-6 py-6">
          <div class="flex items-start gap-3">
            <div class="flex-shrink-0 w-10 h-10 rounded-full bg-red-100 flex items-center justify-center">
              <svg class="w-5 h-5 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.082 16.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </div>
            <div>
              <p class="text-sm text-gray-700">
                Are you sure you want to delete
                <strong class="text-gray-900">'{{ taskTitle }}'</strong>?
              </p>
              <p class="mt-2 text-sm text-red-600">This action cannot be undone.</p>
            </div>
          </div>
        </div>

        <div class="flex justify-end gap-3 px-6 py-4 border-t border-gray-200">
          <button
            type="button" @click="handleCancel" class="btn-secondary" :disabled="loading"
          >
            Cancel
          </button>
          <button
            ref="deleteBtnRef"
            type="button"
            @click="handleConfirm"
            class="btn-danger inline-flex items-center gap-2"
            :disabled="loading"
          >
            <svg
              v-if="loading" class="animate-spin -ml-1 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg"
              fill="none" viewBox="0 0 24 24" aria-hidden="true"
            >
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
            </svg>
            Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
