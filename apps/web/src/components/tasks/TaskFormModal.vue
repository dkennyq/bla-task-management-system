<script setup lang="ts">
import { ref, watch, nextTick, onMounted, onUnmounted } from 'vue'
import { useTasksStore } from '../../stores/tasksStore'
import type { Task, TaskStatus, TaskPriority, CreateTaskDto, UpdateTaskDto } from '../../types/task'

const props = withDefaults(defineProps<{
  isOpen: boolean
  mode: 'create' | 'edit'
  taskData?: Task | null
}>(), {
  taskData: null,
})

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'save'): void
}>()

const tasksStore = useTasksStore()

interface FormData {
  title: string
  description: string
  priority: TaskPriority
  status: TaskStatus
  dueDate: string
}

const form = ref<FormData>({
  title: '',
  description: '',
  priority: 'Medium' as TaskPriority,
  status: 'Pending' as TaskStatus,
  dueDate: '',
})

const errors = ref<Record<string, string>>({})
const submitting = ref(false)
const submitError = ref<string | null>(null)
const titleInput = ref<HTMLInputElement | null>(null)

function formatDateForInput(dateStr: string): string {
  return dateStr.split('T')[0]
}

function initForm() {
  if (props.mode === 'edit' && props.taskData) {
    form.value = {
      title: props.taskData.title,
      description: props.taskData.description,
      priority: props.taskData.priority,
      status: props.taskData.status,
      dueDate: props.taskData.dueDate ? formatDateForInput(props.taskData.dueDate) : '',
    }
  } else {
    form.value = {
      title: '',
      description: '',
      priority: 'Medium' as TaskPriority,
      status: 'Pending' as TaskStatus,
      dueDate: '',
    }
  }
  errors.value = {}
  submitError.value = null
}

function validate(): boolean {
  const newErrors: Record<string, string> = {}

  if (!form.value.title.trim()) {
    newErrors.title = 'Title is required'
  } else if (form.value.title.length > 200) {
    newErrors.title = 'Title must be 200 characters or less'
  }

  if (form.value.description.length > 1000) {
    newErrors.description = 'Description must be 1000 characters or less'
  }

  if (!form.value.priority) {
    newErrors.priority = 'Priority is required'
  }

  if (form.value.dueDate) {
    const today = new Date()
    today.setHours(0, 0, 0, 0)
    const due = new Date(form.value.dueDate + 'T00:00:00')
    if (due < today) {
      newErrors.dueDate = 'Due date must be today or later'
    }
  }

  if (props.mode === 'edit' && !form.value.status) {
    newErrors.status = 'Status is required'
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
    if (props.mode === 'create') {
      const dto: CreateTaskDto = {
        title: form.value.title.trim(),
        description: form.value.description.trim(),
        priority: form.value.priority,
        status: form.value.status,
        dueDate: form.value.dueDate || undefined,
      }
      await tasksStore.createTask(dto)
    } else {
      const dto: UpdateTaskDto = {
        title: form.value.title.trim() || undefined,
        description: form.value.description.trim() || undefined,
        priority: form.value.priority || undefined,
        status: form.value.status || undefined,
        dueDate: form.value.dueDate || undefined,
      }
      await tasksStore.updateTask(props.taskData!.id, dto)
    }
    emit('save')
  } catch (err: unknown) {
    submitError.value = (err as { message?: string }).message || `Failed to ${props.mode} task`
  } finally {
    submitting.value = false
  }
}

watch(() => props.isOpen, (open) => {
  if (open) {
    initForm()
    nextTick(() => {
      titleInput.value?.focus()
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
      titleInput.value?.focus()
    })
  }
})

onUnmounted(() => document.removeEventListener('keydown', onKeyDown))
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto" @keydown.escape="handleClose">
    <div
      class="fixed inset-0 bg-black bg-opacity-50 transition-opacity" @click="handleClose"
      data-testid="modal-backdrop"
    ></div>
    <div class="flex min-h-full items-center justify-center p-4">
      <div
        class="relative w-full max-w-lg bg-white rounded-xl shadow-xl" role="dialog" aria-modal="true"
        :aria-label="mode === 'create' ? 'Create New Task' : 'Edit Task'"
      >
        <div class="flex items-center justify-between px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-semibold text-gray-900">
            {{ mode === 'create' ? 'Create New Task' : 'Edit Task' }}
          </h3>
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
            <label for="task-title" class="block text-sm font-medium text-gray-700 mb-1">
              Title <span class="text-red-500">*</span>
            </label>
            <input
              id="task-title" ref="titleInput" v-model="form.title" type="text" maxlength="200" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.title }" placeholder="Enter task title"
            >
            <p v-if="errors.title" class="mt-1 text-sm text-red-600">{{ errors.title }}</p>
          </div>

          <div>
            <label for="task-description" class="block text-sm font-medium text-gray-700 mb-1">
              Description
            </label>
            <textarea
              id="task-description" v-model="form.description" rows="3" maxlength="1000" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.description }"
              placeholder="Enter task description (optional)"
            ></textarea>
            <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
          </div>

          <div>
            <label for="task-priority" class="block text-sm font-medium text-gray-700 mb-1">
              Priority <span class="text-red-500">*</span>
            </label>
            <select
              id="task-priority" v-model="form.priority" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.priority }"
            >
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>
            <p v-if="errors.priority" class="mt-1 text-sm text-red-600">{{ errors.priority }}</p>
          </div>

          <div>
            <label for="task-duedate" class="block text-sm font-medium text-gray-700 mb-1">
              Due Date
            </label>
            <input
              id="task-duedate" v-model="form.dueDate" type="date" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.dueDate }"
            >
            <p v-if="errors.dueDate" class="mt-1 text-sm text-red-600">{{ errors.dueDate }}</p>
          </div>

          <div v-if="mode === 'edit'">
            <label for="task-status" class="block text-sm font-medium text-gray-700 mb-1">
              Status <span class="text-red-500">*</span>
            </label>
            <select
              id="task-status" v-model="form.status" class="input-field"
              :class="{ 'border-red-500 focus:ring-red-500': errors.status }"
            >
              <option value="Pending">Pending</option>
              <option value="InProgress">In Progress</option>
              <option value="Completed">Completed</option>
            </select>
            <p v-if="errors.status" class="mt-1 text-sm text-red-600">{{ errors.status }}</p>
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
              {{ mode === 'create' ? 'Create Task' : 'Save Changes' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
