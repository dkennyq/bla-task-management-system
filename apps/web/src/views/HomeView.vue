<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
  <div class="min-h-screen flex flex-col items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
    <div class="card max-w-2xl w-full mx-4">
      <div class="text-center">
        <h1 class="text-5xl font-bold text-gray-900 mb-4">
          BLA Task Management
        </h1>
        <p class="text-xl text-gray-600 mb-8">
          Welcome to your task management system
        </p>
        
        <div class="space-y-4 max-w-md mx-auto">
          <RouterLink
            to="/login"
            class="btn-primary w-full block text-center text-lg py-3"
          >
            Get Started
          </RouterLink>
          
          <p class="text-sm text-gray-500">
            Organize your tasks efficiently with our modern task management system
          </p>
        </div>

        <div class="mt-12 grid grid-cols-1 md:grid-cols-3 gap-6 text-left">
          <div class="p-4 bg-blue-50 rounded-lg">
            <div class="text-blue-600 font-bold text-lg mb-2">📝 Create Tasks</div>
            <p class="text-gray-600 text-sm">Easily create and organize your tasks with priorities and due dates</p>
          </div>
          
          <div class="p-4 bg-green-50 rounded-lg">
            <div class="text-green-600 font-bold text-lg mb-2">✅ Track Progress</div>
            <p class="text-gray-600 text-sm">Monitor task status and completion in real-time</p>
          </div>
          
          <div class="p-4 bg-purple-50 rounded-lg">
            <div class="text-purple-600 font-bold text-lg mb-2">🎯 Stay Organized</div>
            <p class="text-gray-600 text-sm">Filter, search, and manage tasks efficiently</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
