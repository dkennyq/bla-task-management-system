<script setup lang="ts">
import { onMounted, computed, ref } from 'vue'
import { useToast } from 'vue-toastification'
import { useTasksStore } from '../stores/tasksStore'
import TaskCard from '../components/tasks/TaskCard.vue'
import TaskFormModal from '../components/tasks/TaskFormModal.vue'
import DeleteConfirmDialog from '../components/tasks/DeleteConfirmDialog.vue'
import type { Task, TaskStatus } from '../types/task'
import type { SortBy } from '../stores/tasksStore'

const toast = useToast()
const tasksStore = useTasksStore()

const showModal = ref(false)
const modalMode = ref<'create' | 'edit'>('create')
const editingTask = ref<Task | null>(null)

const showDeleteDialog = ref(false)
const taskToDelete = ref<Task | null>(null)
const deletingTask = ref(false)
const updatingTaskId = ref<string | null>(null)

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

function handleEdit(task: Task) {
  openEditModal(task)
}

function handleDelete(task: Task) {
  taskToDelete.value = task
  showDeleteDialog.value = true
}

function handleDeleteCancel() {
  showDeleteDialog.value = false
  taskToDelete.value = null
}

async function handleDeleteConfirm() {
  if (!taskToDelete.value) return
  deletingTask.value = true
  try {
    await tasksStore.deleteTask(taskToDelete.value.id)
    toast.success('Task deleted successfully')
    showDeleteDialog.value = false
    taskToDelete.value = null
  } catch {
    toast.error('Failed to delete task')
  } finally {
    deletingTask.value = false
  }
}

async function handleStatusChange(task: Task, newStatus: TaskStatus) {
  updatingTaskId.value = task.id
  try {
    const dto = {
      title: task.title,
      description: task.description,
      priority: task.priority,
      status: newStatus,
      dueDate: task.dueDate || undefined,
    }
    await tasksStore.updateTask(task.id, dto)
    toast.success('Status updated successfully')
  } catch {
    toast.error('Failed to update status')
  } finally {
    updatingTaskId.value = null
  }
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
        <TaskCard
          v-for="task in tasksStore.filteredTasks"
          :key="task.id"
          :task="task"
          :status-updating="task.id === updatingTaskId"
          @edit="handleEdit"
          @delete="handleDelete"
          @status-change="handleStatusChange"
        />
      </div>

      <!-- Modals -->
      <TaskFormModal
        :is-open="showModal"
        :mode="modalMode"
        :task-data="editingTask"
        @save="handleModalSave"
        @close="handleModalClose"
      />

      <DeleteConfirmDialog
        :is-open="showDeleteDialog"
        :task-title="taskToDelete?.title ?? ''"
        :loading="deletingTask"
        @confirm="handleDeleteConfirm"
        @cancel="handleDeleteCancel"
      />
    </div>
  </div>
</template>
