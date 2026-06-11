<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

const isAuthenticated = computed(() => authStore.isAuthenticated)
const isManager = computed(() => authStore.isManager)
const currentUser = computed(() => authStore.currentUser)

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <div class="min-h-screen flex flex-col">
    <!-- Header -->
    <header class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <!-- Logo / Brand -->
          <div class="flex items-center">
            <RouterLink to="/" class="text-2xl font-bold text-blue-600 hover:text-blue-700">
              BLA Tasks
            </RouterLink>
          </div>

          <!-- Navigation -->
          <nav class="flex items-center space-x-4">
            <template v-if="isAuthenticated">
              <RouterLink
                to="/tasks"
                class="text-gray-700 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium"
              >
                Tasks
              </RouterLink>
              <RouterLink
                v-if="isManager"
                to="/admin/users"
                class="text-gray-700 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium"
              >
                Admin
              </RouterLink>
              <div class="flex items-center space-x-3 border-l border-gray-200 pl-4">
                <span class="text-sm text-gray-600">
                  {{ currentUser?.username || currentUser?.email }}
                </span>
                <span class="text-xs bg-purple-100 text-purple-700 px-2 py-0.5 rounded-full font-medium">
                  {{ currentUser?.role }}
                </span>
                <button
                  @click="handleLogout"
                  class="text-sm text-red-600 hover:text-red-700 font-medium"
                >
                  Logout
                </button>
              </div>
            </template>
            
            <template v-else>
              <RouterLink
                to="/login"
                class="btn-primary"
              >
                Login
              </RouterLink>
            </template>
          </nav>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="flex-grow">
      <slot></slot>
    </main>

    <!-- Footer -->
    <footer class="bg-white border-t border-gray-200 mt-auto">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <p class="text-center text-sm text-gray-500">
          © 2026 BLA Task Management System. Built with Vue.js 3 & TailwindCSS.
        </p>
      </div>
    </footer>
  </div>
</template>
