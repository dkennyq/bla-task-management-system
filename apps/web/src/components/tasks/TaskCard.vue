<script setup lang="ts">
import type { Task } from '../../types/task'
import TaskActions from './TaskActions.vue'
import type { TaskStatus } from '../../types/task'

defineProps<{
  task: Task
  statusUpdating?: boolean
}>()

const emit = defineEmits<{
  (e: 'edit', task: Task): void
  (e: 'delete', task: Task): void
  (e: 'statusChange', task: Task, newStatus: TaskStatus): void
}>()

function priorityBadgeColor(priority: string): string {
  switch (priority) {
    case 'High': return 'bg-red-100 text-red-800'
    case 'Medium': return 'bg-orange-100 text-orange-800'
    case 'Low': return 'bg-gray-100 text-gray-600'
    default: return 'bg-gray-100 text-gray-600'
  }
}

function formatDate(dateStr?: string): string {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  })
}

function handleClick(task: Task) {
  emit('edit', task)
}
</script>

<template>
  <div
    class="card hover:shadow-lg hover:border-blue-200 transition-all duration-200 border border-transparent block text-left w-full cursor-pointer group"
    @click="handleClick(task)" data-testid="task-card"
  >
    <div class="flex items-start justify-between gap-3 mb-3">
      <span
        :class="[
          'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
          priorityBadgeColor(task.priority),
        ]"
      >
        {{ task.priority }}
      </span>
      <div class="flex items-center gap-2 shrink-0" @click.stop>
        <TaskActions
          :task="task"
          :status-updating="statusUpdating"
          @edit="emit('edit', $event)"
          @delete="emit('delete', $event)"
          @status-change="(t, s) => emit('statusChange', t, s)"
        />
      </div>
    </div>
    <h3 class="text-lg font-semibold text-gray-900 mb-1 truncate">{{ task.title }}</h3>
    <p class="text-sm text-gray-600 mb-3 line-clamp-2">{{ task.description }}</p>
    <div class="flex items-center justify-between text-xs text-gray-500">
      <span v-if="task.dueDate" class="flex items-center gap-1" title="Due date">
        <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
        {{ formatDate(task.dueDate) }}
      </span>
      <span title="Created">{{ formatDate(task.createdAt) }}</span>
    </div>
  </div>
</template>
