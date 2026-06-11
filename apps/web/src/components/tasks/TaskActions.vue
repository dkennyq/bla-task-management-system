<script setup lang="ts">
import { computed } from 'vue'
import type { Task, TaskStatus } from '../../types/task'

const props = defineProps<{
  task: Task
  loading?: boolean
  statusUpdating?: boolean
}>()

const emit = defineEmits<{
  (e: 'edit', task: Task): void
  (e: 'delete', task: Task): void
  (e: 'statusChange', task: Task, newStatus: TaskStatus): void
}>()

const isActionLoading = computed(() =>
  props.loading || props.statusUpdating
)

function handleEdit() {
  if (isActionLoading.value) return
  emit('edit', props.task)
}

function handleDelete() {
  if (isActionLoading.value) return
  emit('delete', props.task)
}

function handleStatusChange(event: Event) {
  if (isActionLoading.value) return
  const select = event.target as HTMLSelectElement
  const newStatus = select.value as TaskStatus
  if (newStatus === props.task.status) return
  emit('statusChange', props.task, newStatus)
}
</script>

<template>
  <div class="flex items-center gap-1.5">
    <!-- Status Dropdown -->
    <div class="relative">
      <select
        :value="task.status"
        @change="handleStatusChange"
        :disabled="isActionLoading"
        class="text-xs rounded-md border-gray-300 py-1 pl-2 pr-6 focus:ring-blue-500 focus:border-blue-500 disabled:opacity-50 disabled:cursor-not-allowed appearance-none bg-white"
        :class="{
          'text-yellow-800 bg-yellow-50 border-yellow-300': task.status === 'Pending',
          'text-blue-800 bg-blue-50 border-blue-300': task.status === 'InProgress',
          'text-green-800 bg-green-50 border-green-300': task.status === 'Completed',
        }"
        aria-label="Change task status"
      >
        <option value="Pending">Pending</option>
        <option value="InProgress">In Progress</option>
        <option value="Completed">Completed</option>
      </select>
      <svg
        v-if="statusUpdating"
        class="absolute right-1.5 top-1/2 -translate-y-1/2 w-3 h-3 animate-spin text-gray-500"
        xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" aria-hidden="true"
      >
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
      </svg>
    </div>

    <!-- Edit Button -->
    <button
      @click.stop="handleEdit"
      :disabled="isActionLoading"
      class="p-1.5 rounded-lg text-gray-400 hover:text-blue-600 hover:bg-blue-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
      aria-label="Edit task"
      title="Edit task"
    >
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
      </svg>
    </button>

    <!-- Delete Button -->
    <button
      @click.stop="handleDelete"
      :disabled="isActionLoading"
      class="p-1.5 rounded-lg text-gray-400 hover:text-red-600 hover:bg-red-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
      aria-label="Delete task"
      title="Delete task"
    >
      <svg
        class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true"
      >
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
      </svg>
    </button>
  </div>
</template>
