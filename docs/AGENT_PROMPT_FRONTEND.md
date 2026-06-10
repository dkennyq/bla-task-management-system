# 🎨 Frontend Agent Prompt - BLA Task Management System

This document contains the complete prompt for an AI agent to implement frontend tasks autonomously.

---

## 📋 Project Context

**Project:** BLA Task Management System  
**Stack:** Vue.js 3 (Composition API), Vite, Pinia, TailwindCSS  
**Repository:** https://github.com/dkennyq/bla-task-management-system  
**GitHub Project:** https://github.com/users/dkennyq/projects/1

---

## 🎯 Your Role

You are a **specialized agent in Vue.js 3 frontend development with Composition API and TDD**.

Your goal is to implement frontend features following:
- ✅ Vue.js 3 Composition API (setup script)
- ✅ Test-Driven Development with Vitest
- ✅ TailwindCSS for styles
- ✅ Pinia for state management
- ✅ TypeScript (optional but recommended)

---

## 📚 Reference Documents

**YOU MUST READ BEFORE STARTING:**

1. **`docs/USER_STORIES.md`**
   - Contains all 17 complete user stories
   - Frontend User Stories: US-11 to US-17
   - UI mockups and specifications
   - **Special section for AI Agents** (lines 22-98)

2. **`docs/DEVELOPMENT_WORKFLOW.md`**
   - How to work with Vite dev server
   - Automatic hot reload

3. **`docs/TESTING_APIS.md`**
   - Backend API URLs
   - Tasks API: http://localhost:5077
   - Users API: http://localhost:5078

---

## 🚀 General Instructions

### 1️⃣ Before Starting

```bash
# 1. Read the GitHub issue
gh issue view <ISSUE_NUMBER>

# 2. Identify the corresponding User Story
#    Issue #9  → US-13: Login Page
#    Issue #10 → US-15: Task List View
#    etc.

# 3. Read the complete guide in docs/USER_STORIES.md
#    Find the corresponding user story section
```

### 2️⃣ Verify Prerequisites

```bash
# Databases running
docker ps

# Backend APIs running
# Tasks API: http://localhost:5077
# Users API: http://localhost:5078

# If not:
cd apps/tasks-api/src/TasksApi.WebApi
dotnet watch run

# (In another terminal for Users API when ready)
```

### 3️⃣ Frontend Project Setup

**⚠️ IMPORTANT: The Vue.js project does NOT exist yet. You must create it first.**

```bash
cd C:\Users\devke\source\bla-task-management-system\apps

# Create Vue.js project with Vite
npm create vite@latest web -- --template vue

cd web

# Install dependencies
npm install

# Install additional dependencies
npm install pinia
npm install vue-router
npm install axios
npm install -D tailwindcss postcss autoprefixer
npm install -D vitest @vue/test-utils happy-dom
npm install -D @vitest/ui

# Initialize TailwindCSS
npx tailwindcss init -p
```

### 4️⃣ Initial Configuration

**tailwind.config.js:**

```js
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
```

**src/style.css:**

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

**vite.config.js:**

```js
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  server: {
    port: 3000
  },
  test: {
    globals: true,
    environment: 'happy-dom'
  }
})
```

**package.json (scripts):**

```json
{
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview",
    "test": "vitest",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest --coverage"
  }
}
```

### 5️⃣ Start Dev Server

```bash
cd apps/web
npm run dev

# Dev server: http://localhost:3000
# Hot reload: automatic on changes
```

---

## 🔴🟢 TDD Methodology for Frontend

### Red-Green-Refactor Cycle

```
1. 🔴 RED (Test Fails)
   - Write the component test FIRST
   - The test must FAIL (component doesn't exist)
   - Verify it fails for the right reason

2. 🟢 GREEN (Test Passes)
   - Create the minimum component to pass
   - Don't worry about CSS yet
   - Just basic functionality

3. ♻️ REFACTOR (Improve + Style)
   - Improve the code
   - Add TailwindCSS styles
   - Optimize performance
   - All tests must keep passing
```

### Implementation Order

```
1️⃣ Store (Pinia)
   ├─ Test: stores/__tests__/auth.spec.js
   └─ Code: stores/auth.js

2️⃣ API Services
   ├─ Test: services/__tests__/api.spec.js
   └─ Code: services/api.js

3️⃣ Components
   ├─ Test: components/__tests__/LoginForm.spec.js
   └─ Code: components/LoginForm.vue

4️⃣ Views
   ├─ Test: views/__tests__/LoginView.spec.js
   └─ Code: views/LoginView.vue

5️⃣ Router
   └─ Code: router/index.js
```

---

## 📝 Template de Implementación

### Paso 1: Pinia Store

**Ubicación del test:**
```
apps/web/src/stores/__tests__/auth.spec.js
```

**Ejemplo de test:**

```js
import { describe, it, expect, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../auth'

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('should initialize with null user', () => {
    const store = useAuthStore()
    expect(store.user).toBe(null)
    expect(store.isAuthenticated).toBe(false)
  })

  it('should set user on login', async () => {
    const store = useAuthStore()
    const mockUser = { email: 'test@example.com', token: 'abc123' }
    
    await store.login('test@example.com', 'password')
    
    expect(store.user).toEqual(mockUser)
    expect(store.isAuthenticated).toBe(true)
  })
})
```

**Ubicación del código:**
```
apps/web/src/stores/auth.js
```

**Ejemplo de código:**

```js
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { login as loginApi } from '../services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null)
  const token = ref(localStorage.getItem('token'))

  const isAuthenticated = computed(() => !!user.value)

  async function login(email, password) {
    const response = await loginApi(email, password)
    user.value = response.user
    token.value = response.token
    localStorage.setItem('token', response.token)
  }

  function logout() {
    user.value = null
    token.value = null
    localStorage.removeItem('token')
  }

  return { user, token, isAuthenticated, login, logout }
})
```

### Paso 2: API Service

**Ubicación del test:**
```
apps/web/src/services/__tests__/api.spec.js
```

**Ejemplo de test:**

```js
import { describe, it, expect, vi } from 'vitest'
import { login, getTasks, createTask } from '../api'

global.fetch = vi.fn()

describe('API Service', () => {
  it('should call login endpoint', async () => {
    const mockResponse = { token: 'abc123', user: { email: 'test@example.com' } }
    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => mockResponse
    })

    const result = await login('test@example.com', 'password')

    expect(fetch).toHaveBeenCalledWith(
      'http://localhost:5078/api/users/login',
      expect.objectContaining({
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: 'test@example.com', password: 'password' })
      })
    )
    expect(result).toEqual(mockResponse)
  })
})
```

**Ubicación del código:**
```
apps/web/src/services/api.js
```

**Ejemplo de código:**

```js
const TASKS_API = 'http://localhost:5077/api'
const USERS_API = 'http://localhost:5078/api'

export async function login(email, password) {
  const response = await fetch(`${USERS_API}/users/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  })
  
  if (!response.ok) throw new Error('Login failed')
  return response.json()
}

export async function getTasks(userId) {
  const response = await fetch(`${TASKS_API}/tasks?userId=${userId}`)
  if (!response.ok) throw new Error('Failed to fetch tasks')
  return response.json()
}

export async function createTask(task) {
  const response = await fetch(`${TASKS_API}/tasks`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(task)
  })
  
  if (!response.ok) throw new Error('Failed to create task')
  return response.json()
}
```

### Paso 3: Componente Vue

**Ubicación del test:**
```
apps/web/src/components/__tests__/LoginForm.spec.js
```

**Ejemplo de test:**

```js
import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import LoginForm from '../LoginForm.vue'

describe('LoginForm', () => {
  it('renders email and password inputs', () => {
    const wrapper = mount(LoginForm)
    
    expect(wrapper.find('input[type="email"]').exists()).toBe(true)
    expect(wrapper.find('input[type="password"]').exists()).toBe(true)
    expect(wrapper.find('button[type="submit"]').exists()).toBe(true)
  })

  it('emits login event with credentials on submit', async () => {
    const wrapper = mount(LoginForm)
    
    await wrapper.find('input[type="email"]').setValue('test@example.com')
    await wrapper.find('input[type="password"]').setValue('password123')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('login')).toBeTruthy()
    expect(wrapper.emitted('login')[0]).toEqual([{
      email: 'test@example.com',
      password: 'password123'
    }])
  })
})
```

**Ubicación del código:**
```
apps/web/src/components/LoginForm.vue
```

**Ejemplo de código:**

```vue
<script setup>
import { ref } from 'vue'

const email = ref('')
const password = ref('')

const emit = defineEmits(['login'])

function handleSubmit() {
  emit('login', {
    email: email.value,
    password: password.value
  })
}
</script>

<template>
  <form @submit.prevent="handleSubmit" class="max-w-md mx-auto p-6 bg-white rounded-lg shadow-md">
    <h2 class="text-2xl font-bold mb-6 text-gray-800">Login</h2>
    
    <div class="mb-4">
      <label for="email" class="block text-gray-700 font-medium mb-2">
        Email
      </label>
      <input
        id="email"
        v-model="email"
        type="email"
        required
        class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        placeholder="your@email.com"
      />
    </div>

    <div class="mb-6">
      <label for="password" class="block text-gray-700 font-medium mb-2">
        Password
      </label>
      <input
        id="password"
        v-model="password"
        type="password"
        required
        class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        placeholder="••••••••"
      />
    </div>

    <button
      type="submit"
      class="w-full bg-blue-600 text-white font-semibold py-2 px-4 rounded-lg hover:bg-blue-700 transition duration-200"
    >
      Sign In
    </button>
  </form>
</template>
```

---

## ✅ Feature Checklist

```
Setup (First time only):
□ Vue.js project created with Vite
□ Dependencies installed (Pinia, Router, Axios, etc.)
□ TailwindCSS configured
□ Vitest configured

Store (Pinia):
□ Tests written and failing (Red)
□ Store created with state and actions
□ Tests passing (Green)
□ Code refactored

API Service:
□ Tests with mocks written (Red)
□ API functions implemented
□ Tests passing (Green)
□ Error handling added

Components:
□ Tests written and failing (Red)
□ Component created with basic logic
□ Tests passing (Green)
□ TailwindCSS styles applied
□ Component refactored

Views:
□ Tests written (Red)
□ View created
□ Tests passing (Green)
□ Router configured

Final Verification:
□ npm run test: all tests pass
□ npm run dev: app runs without errors
□ Manual verification in browser
□ Responsive design works
□ No console warnings
```

---

## 🧪 Testing Commands

```bash
# Run all tests
npm run test

# Watch mode (auto-runs on changes)
npm run test

# Interactive UI
npm run test:ui

# With coverage
npm run test:coverage

# Test a specific file
npm run test LoginForm.spec.js
```

---

## 🎨 TailwindCSS Style Guide

### Project Colors

```
Primary: blue-600
Secondary: gray-600
Success: green-600
Error: red-600
Warning: yellow-600
```

### Common Components

**Primary button:**
```html
<button class="bg-blue-600 text-white font-semibold py-2 px-4 rounded-lg hover:bg-blue-700 transition duration-200">
  Button
</button>
```

**Input:**
```html
<input class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" />
```

**Card:**
```html
<div class="bg-white rounded-lg shadow-md p-6">
  <!-- content -->
</div>
```

---

## 📦 Expected File Structure

```
apps/web/
├── public/
├── src/
│   ├── assets/
│   ├── components/
│   │   ├── __tests__/
│   │   │   └── LoginForm.spec.js
│   │   ├── LoginForm.vue
│   │   ├── TaskList.vue
│   │   └── TaskItem.vue
│   ├── views/
│   │   ├── __tests__/
│   │   │   └── LoginView.spec.js
│   │   ├── LoginView.vue
│   │   └── TasksView.vue
│   ├── stores/
│   │   ├── __tests__/
│   │   │   └── auth.spec.js
│   │   ├── auth.js
│   │   └── tasks.js
│   ├── services/
│   │   ├── __tests__/
│   │   │   └── api.spec.js
│   │   └── api.js
│   ├── router/
│   │   └── index.js
│   ├── App.vue
│   ├── main.js
│   └── style.css
├── index.html
├── vite.config.js
├── tailwind.config.js
└── package.json
```

---

## 🎯 Complete Example: Implement Issue #9 (US-13: Login Page)

### 1. Read the Issue

```bash
gh issue view 9
```

### 2. Read the Guide

Open `docs/USER_STORIES.md` → Find "US-13: Login Page"

### 3. Implement with TDD

#### Store (15 minutes)
```bash
# 1. Create test
code src/stores/__tests__/auth.spec.js

# 2. Run (should fail)
npm run test auth.spec.js

# 3. Implement store
code src/stores/auth.js

# 4. Test passes
npm run test auth.spec.js
```

#### API Service (10 minutes)
```bash
# Test → Implement → Verify
code src/services/__tests__/api.spec.js
code src/services/api.js
npm run test api.spec.js
```

#### LoginForm Component (20 minutes)
```bash
# Test → Implement → Style
code src/components/__tests__/LoginForm.spec.js
code src/components/LoginForm.vue
npm run test LoginForm.spec.js
```

#### LoginView (10 minutes)
```bash
code src/views/LoginView.vue
```

### 4. Manual Verification

```bash
# 1. Start dev server
npm run dev

# 2. Open browser
# http://localhost:3000

# 3. Test login
# Email: admin@taskmanagement.com
# Password: Password123!

# 4. Verify it redirects correctly
```

### 5. Commit

```bash
git add .
git commit -m "feat: Implement US-13 Login Page #9

- Add auth store with Pinia
- Add API service for login
- Add LoginForm component with validation
- Add LoginView
- Configure router with auth guard
- All tests passing (X tests)

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
git push origin master
```

---

## 🚨 Constraints and Rules

### ❌ NOT Allowed

- ❌ Options API (use Composition API)
- ❌ Code without tests
- ❌ Inline styles (use TailwindCSS)
- ❌ var (use const/let)
- ❌ console.log in production

### ✅ YES Allowed/Required

- ✅ Composition API with <script setup>
- ✅ Pinia for state
- ✅ TailwindCSS for styles
- ✅ Vitest + @vue/test-utils
- ✅ Async/await for API calls
- ✅ Error handling

---

## 📞 Communication

When finishing the implementation, report:

```markdown
✅ COMPLETED: Issue #X - US-XX: [Title]

**Summary:**
- X new tests (all passing)
- X components created
- X views created
- X stores configured

**Files created:**
- apps/web/src/components/XxxComponent.vue
- apps/web/src/views/XxxView.vue
- apps/web/src/stores/xxx.js
- ...

**Verification:**
- ✅ npm run test: X tests passed
- ✅ npm run dev: Runs without errors
- ✅ Browser: Works correctly
- ✅ Responsive: Tested on mobile/desktop
- ✅ No console warnings

**Screenshots:**
[Attach screenshots if possible]

**Commit:** [hash]
**Push:** Completed to origin/master
```

---

## 🎓 Additional Resources

- **Vue.js 3:** https://vuejs.org/guide/introduction.html
- **Pinia:** https://pinia.vuejs.org/
- **TailwindCSS:** https://tailwindcss.com/docs
- **Vitest:** https://vitest.dev/guide/
- **Vue Test Utils:** https://test-utils.vuejs.org/

---

**Last updated:** 2026-06-09  
**Version:** 1.0  
**Author:** BLA Task Management Team
