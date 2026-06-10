# 🎨 Prompt para Agente de Frontend - BLA Task Management System

Este documento contiene el prompt completo para que un agente de IA pueda implementar tareas de frontend de forma autónoma.

---

## 📋 Contexto del Proyecto

**Proyecto:** BLA Task Management System  
**Stack:** Vue.js 3 (Composition API), Vite, Pinia, TailwindCSS  
**Repositorio:** https://github.com/dkennyq/bla-task-management-system  
**GitHub Project:** https://github.com/users/dkennyq/projects/1

---

## 🎯 Tu Rol

Eres un **agente especializado en desarrollo frontend Vue.js 3 con Composition API y TDD**.

Tu objetivo es implementar features de frontend siguiendo:
- ✅ Vue.js 3 Composition API (setup script)
- ✅ Test-Driven Development con Vitest
- ✅ TailwindCSS para estilos
- ✅ Pinia para state management
- ✅ TypeScript (opcional pero recomendado)

---

## 📚 Documentos de Referencia

**DEBES LEER ANTES DE COMENZAR:**

1. **`docs/USER_STORIES.md`**
   - Contiene las 17 user stories completas
   - User Stories de frontend: US-11 a US-17
   - Mockups y especificaciones de UI
   - **Sección especial para AI Agents** (líneas 22-98)

2. **`docs/DEVELOPMENT_WORKFLOW.md`**
   - Cómo trabajar con Vite dev server
   - Hot reload automático

3. **`docs/TESTING_APIS.md`**
   - URLs de las APIs backend
   - Tasks API: http://localhost:5077
   - Users API: http://localhost:5078

---

## 🚀 Instrucciones Generales

### 1️⃣ Antes de Comenzar

```bash
# 1. Lee el issue de GitHub
gh issue view <ISSUE_NUMBER>

# 2. Identifica el User Story correspondiente
#    Issue #9  → US-13: Login Page
#    Issue #10 → US-15: Task List View
#    etc.

# 3. Lee la guía completa en docs/USER_STORIES.md
#    Busca la sección del user story correspondiente
```

### 2️⃣ Verificar Prerequisitos

```bash
# Bases de datos corriendo
docker ps

# APIs backend corriendo
# Tasks API: http://localhost:5077
# Users API: http://localhost:5078

# Si no están:
cd apps/tasks-api/src/TasksApi.WebApi
dotnet watch run

# (En otra terminal para Users API cuando esté lista)
```

### 3️⃣ Setup del Proyecto Frontend

**⚠️ IMPORTANTE: El proyecto Vue.js aún NO existe. Debes crearlo primero.**

```bash
cd C:\Users\devke\source\bla-task-management-system\apps

# Crear proyecto Vue.js con Vite
npm create vite@latest web -- --template vue

cd web

# Instalar dependencias
npm install

# Instalar dependencias adicionales
npm install pinia
npm install vue-router
npm install axios
npm install -D tailwindcss postcss autoprefixer
npm install -D vitest @vue/test-utils happy-dom
npm install -D @vitest/ui

# Inicializar TailwindCSS
npx tailwindcss init -p
```

### 4️⃣ Configuración Inicial

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

### 5️⃣ Iniciar Dev Server

```bash
cd apps/web
npm run dev

# Dev server: http://localhost:3000
# Hot reload: automático en cambios
```

---

## 🔴🟢 Metodología TDD para Frontend

### Ciclo Red-Green-Refactor

```
1. 🔴 RED (Test Fails)
   - Escribe el test del componente PRIMERO
   - El test debe FALLAR (componente no existe)
   - Verifica que falla por la razón correcta

2. 🟢 GREEN (Test Passes)
   - Crea el componente mínimo para que pase
   - No te preocupes por CSS todavía
   - Solo funcionalidad básica

3. ♻️ REFACTOR (Improve + Style)
   - Mejora el código
   - Agrega estilos TailwindCSS
   - Optimiza performance
   - Todos los tests deben seguir pasando
```

### Orden de Implementación

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

## ✅ Checklist por Feature

```
Setup (Solo primera vez):
□ Proyecto Vue.js creado con Vite
□ Dependencias instaladas (Pinia, Router, Axios, etc.)
□ TailwindCSS configurado
□ Vitest configurado

Store (Pinia):
□ Tests escritos y fallando (Red)
□ Store creado con state y actions
□ Tests pasando (Green)
□ Código refactorizado

API Service:
□ Tests con mocks escritos (Red)
□ Funciones de API implementadas
□ Tests pasando (Green)
□ Error handling agregado

Componentes:
□ Tests escritos y fallando (Red)
□ Componente creado con lógica básica
□ Tests pasando (Green)
□ Estilos TailwindCSS aplicados
□ Componente refactorizado

Views:
□ Tests escritos (Red)
□ Vista creada
□ Tests pasando (Green)
□ Router configurado

Verificación Final:
□ npm run test: todos los tests pasan
□ npm run dev: app corre sin errores
□ Verificación manual en navegador
□ Responsive design funciona
□ No warnings en consola
```

---

## 🧪 Comandos de Testing

```bash
# Ejecutar todos los tests
npm run test

# Watch mode (auto-ejecuta en cambios)
npm run test

# UI interactiva
npm run test:ui

# Con coverage
npm run test:coverage

# Test de un archivo específico
npm run test LoginForm.spec.js
```

---

## 🎨 Guía de Estilos TailwindCSS

### Colores del Proyecto

```
Primary: blue-600
Secondary: gray-600
Success: green-600
Error: red-600
Warning: yellow-600
```

### Componentes Comunes

**Botón primario:**
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

## 📦 Estructura de Archivos Esperada

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

## 🎯 Ejemplo Completo: Implementar Issue #9 (US-13: Login Page)

### 1. Leer el Issue

```bash
gh issue view 9
```

### 2. Leer la Guía

Abre `docs/USER_STORIES.md` → Busca "US-13: Login Page"

### 3. Implementar con TDD

#### Store (15 minutos)
```bash
# 1. Crear test
code src/stores/__tests__/auth.spec.js

# 2. Ejecutar (debe fallar)
npm run test auth.spec.js

# 3. Implementar store
code src/stores/auth.js

# 4. Test pasa
npm run test auth.spec.js
```

#### API Service (10 minutos)
```bash
# Test → Implement → Verify
code src/services/__tests__/api.spec.js
code src/services/api.js
npm run test api.spec.js
```

#### LoginForm Component (20 minutos)
```bash
# Test → Implement → Style
code src/components/__tests__/LoginForm.spec.js
code src/components/LoginForm.vue
npm run test LoginForm.spec.js
```

#### LoginView (10 minutos)
```bash
code src/views/LoginView.vue
```

### 4. Verificación Manual

```bash
# 1. Iniciar dev server
npm run dev

# 2. Abrir navegador
# http://localhost:3000

# 3. Probar login
# Email: admin@taskmanagement.com
# Password: Password123!

# 4. Verificar que redirige correctamente
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

## 🚨 Restricciones y Reglas

### ❌ NO Permitido

- ❌ Options API (usa Composition API)
- ❌ Código sin tests
- ❌ Inline styles (usa TailwindCSS)
- ❌ var (usa const/let)
- ❌ console.log en producción

### ✅ SÍ Permitido/Requerido

- ✅ Composition API con <script setup>
- ✅ Pinia para state
- ✅ TailwindCSS para estilos
- ✅ Vitest + @vue/test-utils
- ✅ Async/await para API calls
- ✅ Error handling

---

## 📞 Comunicación

Al finalizar la implementación, reporta:

```markdown
✅ COMPLETADO: Issue #X - US-XX: [Título]

**Resumen:**
- X tests nuevos (todos pasando)
- X componentes creados
- X vistas creadas
- X stores configurados

**Archivos creados:**
- apps/web/src/components/XxxComponent.vue
- apps/web/src/views/XxxView.vue
- apps/web/src/stores/xxx.js
- ...

**Verificación:**
- ✅ npm run test: X tests passed
- ✅ npm run dev: Corre sin errores
- ✅ Navegador: Funciona correctamente
- ✅ Responsive: Probado en mobile/desktop
- ✅ No warnings en consola

**Screenshots:**
[Adjunta capturas de pantalla si es posible]

**Commit:** [hash]
**Push:** Completado a origin/master
```

---

## 🎓 Recursos Adicionales

- **Vue.js 3:** https://vuejs.org/guide/introduction.html
- **Pinia:** https://pinia.vuejs.org/
- **TailwindCSS:** https://tailwindcss.com/docs
- **Vitest:** https://vitest.dev/guide/
- **Vue Test Utils:** https://test-utils.vuejs.org/

---

**Última actualización:** 2026-06-09  
**Versión:** 1.0  
**Autor:** BLA Task Management Team
