<script setup lang="ts">
import { onMounted, computed, ref } from 'vue'
import { useTasksStore } from '../stores/tasksStore'
import TaskFormModal from '../components/tasks/TaskFormModal.vue'
import type { Task, TaskStatus } from '../types/task'
import type { SortBy } from '../stores/tasksStore'

const tasksStore = useTasksStore()

const showModal = ref(false)
const modalMode = ref<'create' | 'edit'>('create')
const editingTask = ref<Task | null>(null)

const statusFilters: (TaskStatus | 'All')[] = ['All', 'Pending', 'InProgress', 'Completed']
const sortOptions: { value: SortBy; label: string }[] = [
  { value: 'createdAt', label: 'Created Date' },
  { value: 'dueDate', label: 'Due Date' },
  { value: 'priority', label: 'Priority' },
  { value: 'title', label: 'Title' },
]

function statusLabel(status: TaskStatus | 'All'): string {
  if (status === 'InProgress') return 'In Progress'
  if (status === 'All') return 'All'
  return status
}

function statusBadgeColor(status: TaskStatus): string {
  switch (status) {
    case 'Pending': return 'bg-yellow-100 text-yellow-800'
    case 'InProgress': return 'bg-blue-100 text-blue-800'
    case 'Completed': return 'bg-green-100 text-green-800'
  }
}

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

const emptyMessage = computed(() => {
  if (tasksStore.tasks.length === 0) {
    return 'There are no records.'
  }
  return 'No tasks match your current filters.'
})

onMounted(() => {
  tasksStore.fetchTasks()
})

function handleRetry() {
  tasksStore.fetchTasks()
}

function openCreateModal() {
  modalMode.value = 'create'
  editingTask.value = null
  showModal.value = true
}

function openEditModal(task: Task) {
  modalMode.value = 'edit'
  editingTask.value = task
  showModal.value = true
}

function handleModalSave() {
  showModal.value = false
  editingTask.value = null
  tasksStore.fetchTasks()
}

function handleModalClose() {
  showModal.value = false
  editingTask.value = null
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-3xl font-bold text-gray-900">Tasks</h2>
        <button
          @click="openCreateModal()"
          class="btn-primary inline-flex items-center gap-2"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          Create New Task
        </button>
      </div>

      <!-- Filters Bar -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6">
        <div class="flex flex-col sm:flex-row gap-4">
          <!-- Status Filter Pills -->
          <div class="flex flex-wrap gap-2 items-center">
            <button
              v-for="status in statusFilters"
              :key="status"
              @click="tasksStore.setFilter(status)"
              :class="[
                'px-3 py-1.5 rounded-full text-sm font-medium transition-colors',
                tasksStore.filter === status
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200',
              ]"
              :aria-pressed="tasksStore.filter === status"
            >
              {{ statusLabel(status) }}
              <span
                v-if="status !== 'All'"
                :class="[
                  'ml-1.5 px-1.5 py-0.5 rounded-full text-xs',
                  tasksStore.filter === status
                    ? 'bg-blue-500 text-white'
                    : 'bg-gray-200 text-gray-600',
                ]"
              >
                {{ tasksStore.tasksByStatus[status === 'InProgress' ? 'inProgress' : status.toLowerCase() as 'pending' | 'inProgress' | 'completed'] }}
              </span>
            </button>
          </div>

          <!-- Search + Sort -->
          <div class="flex flex-1 gap-3 sm:justify-end">
            <div class="relative flex-1 max-w-xs">
              <svg
                class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400"
                fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              <input
                v-model="tasksStore.searchQuery"
                type="search"
                placeholder="Search tasks..."
                class="input-field pl-10"
                aria-label="Search tasks"
              >
            </div>
            <select
              v-model="tasksStore.sortBy"
              class="input-field w-auto"
              aria-label="Sort by"
            >
              <option
                v-for="opt in sortOptions"
                :key="opt.value"
                :value="opt.value"
              >
                {{ opt.label }}
              </option>
            </select>
            <button
              @click="tasksStore.setSortOrder(tasksStore.sortOrder === 'asc' ? 'desc' : 'asc')"
              class="px-3 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
              :aria-label="`Sort ${tasksStore.sortOrder === 'asc' ? 'descending' : 'ascending'}`"
            >
              <svg
                class="w-4 h-4 text-gray-600"
                fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true"
              >
                <path
                  v-if="tasksStore.sortOrder === 'asc'"
                  stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 4h13M3 8h9m-9 4h6m4 0l4-4m0 0l4 4m-4-4v12"
                />
                <path
                  v-else
                  stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 4h13M3 8h9m-9 4h9m5-4v12m0 0l-4-4m4 4l4-4"
                />
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="tasksStore.loading" class="flex justify-center py-16">
        <svg
          class="animate-spin h-10 w-10 text-blue-600" xmlns="http://www.w3.org/2000/svg"
          fill="none" viewBox="0 0 24 24" aria-hidden="true"
        >
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
        </svg>
      </div>

      <!-- Error State -->
      <div
        v-else-if="tasksStore.error"
        class="bg-red-50 border border-red-200 text-red-700 px-6 py-4 rounded-lg text-center"
        role="alert"
      >
        <p class="mb-3">{{ tasksStore.error }}</p>
        <button @click="handleRetry" class="btn-primary text-sm">
          Try Again
        </button>
      </div>

      <!-- Empty State -->
      <div
        v-else-if="tasksStore.filteredTasks.length === 0"
        class="card text-center py-16"
      >
        <svg
          class="mx-auto h-12 w-12 text-gray-400 mb-4" fill="none" stroke="currentColor"
          viewBox="0 0 24 24" aria-hidden="true"
        >
          <path
            stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"
            d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
          />
        </svg>
        <p class="text-gray-500 text-lg">{{ emptyMessage }}</p>
        <button
          v-if="tasksStore.tasks.length === 0"
          @click="openCreateModal()"
          class="btn-primary inline-flex items-center gap-2 mt-4"
        >
          Create Task
        </button>
      </div>

      <!-- Task Cards -->
      <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <button
          v-for="task in tasksStore.filteredTasks"
          :key="task.id"
          @click="openEditModal(task)"
          class="card hover:shadow-lg hover:border-blue-200 transition-all duration-200 border border-transparent block text-left w-full cursor-pointer"
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
            <span
              :class="[
                'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium shrink-0',
                statusBadgeColor(task.status),
              ]"
            >
              {{ statusLabel(task.status) }}
            </span>
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
        </button>
      </div>
      <TaskFormModal
        :is-open="showModal"
        :mode="modalMode"
        :task-data="editingTask"
        @save="handleModalSave"
        @close="handleModalClose"
      />
    </div>
  </div>
</template>
