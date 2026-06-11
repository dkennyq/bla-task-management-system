<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useToast } from 'vue-toastification'
import { getAllUsers, createUserByAdmin, updateUserRole } from '../services/api'
import CreateUserModal from '../components/admin/CreateUserModal.vue'
import EditUserRoleModal from '../components/admin/EditUserRoleModal.vue'
import type { UserListItem, UserRole } from '../types/user'

const toast = useToast()

const users = ref<UserListItem[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

const showCreateModal = ref(false)
const showEditRoleModal = ref(false)
const editingUser = ref<UserListItem | null>(null)

async function fetchUsers() {
  loading.value = true
  error.value = null
  try {
    users.value = await getAllUsers()
  } catch (err: unknown) {
    error.value = (err as { message?: string }).message || 'Failed to load users'
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  showCreateModal.value = true
}

function openEditRoleModal(user: UserListItem) {
  editingUser.value = user
  showEditRoleModal.value = true
}

async function handleCreateUser(data: { username: string; email: string; password: string; fullName: string; role: UserRole }) {
  await createUserByAdmin(data)
  toast.success('User created successfully')
  showCreateModal.value = false
  await fetchUsers()
}

async function handleUpdateRole(userId: string, role: UserRole) {
  await updateUserRole(userId, { role })
  toast.success('User role updated successfully')
  showEditRoleModal.value = false
  editingUser.value = null
  await fetchUsers()
}

onMounted(() => {
  fetchUsers()
})

function formatDate(dateStr: string): string {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString()
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-3xl font-bold text-gray-900">User Management</h2>
        <button
          @click="openCreateModal()"
          class="btn-primary inline-flex items-center gap-2"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          Create User
        </button>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center py-16">
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
        v-else-if="error"
        class="bg-red-50 border border-red-200 text-red-700 px-6 py-4 rounded-lg text-center"
        role="alert"
      >
        <p class="mb-3">{{ error }}</p>
        <button @click="fetchUsers()" class="btn-primary text-sm">
          Try Again
        </button>
      </div>

      <!-- Users Table -->
      <div v-else class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Role</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Created</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="user in users" :key="user.id" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                {{ user.email }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                <span
                  :class="[
                    'px-2 py-1 rounded-full text-xs font-medium',
                    user.role === 'Manager'
                      ? 'bg-purple-100 text-purple-700'
                      : 'bg-gray-100 text-gray-700',
                  ]"
                >
                  {{ user.role }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                <span
                  :class="[
                    'px-2 py-1 rounded-full text-xs font-medium',
                    user.isActive
                      ? 'bg-green-100 text-green-700'
                      : 'bg-red-100 text-red-700',
                  ]"
                >
                  {{ user.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(user.createdAt) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button
                  @click="openEditRoleModal(user)"
                  class="text-blue-600 hover:text-blue-800"
                >
                  Edit Role
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-if="users.length === 0" class="text-center py-16 text-gray-500">
          No users found.
        </div>
      </div>

      <!-- Modals -->
      <CreateUserModal
        :is-open="showCreateModal"
        :on-save="handleCreateUser"
        @close="showCreateModal = false"
      />

      <EditUserRoleModal
        v-if="editingUser"
        :is-open="showEditRoleModal"
        :user="editingUser"
        :on-save="handleUpdateRole"
        @close="showEditRoleModal = false; editingUser = null"
      />
    </div>
  </div>
</template>
