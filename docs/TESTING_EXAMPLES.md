# 🧪 Ejemplos de Pruebas - Tasks API

Este documento contiene ejemplos visuales y capturas de lo que verás al probar la API.

---

## 📖 Ejemplo 1: Swagger UI

### Vista inicial de Swagger

```
┌─────────────────────────────────────────────────────────────┐
│  TasksApi.WebApi                                    v1      │
│                                                              │
│  Servers                                                     │
│  http://localhost:5077                                      │
│                                                              │
│  ▼ Tasks                                                    │
│     GET /api/tasks                                          │
│         Get all tasks for a specific user                   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Endpoint expandido

```
┌─────────────────────────────────────────────────────────────┐
│  ▼ GET /api/tasks                                           │
│                                                              │
│     Get all tasks for a specific user                       │
│                                                              │
│     [Try it out]                                            │
│                                                              │
│     Parameters                                               │
│     ┌─────────────────────────────────────────────────────┐│
│     │ userId * (required)                                  ││
│     │ ┌───────────────────────────────────────────────┐   ││
│     │ │ 00000000-0000-0000-0000-000000000001         │   ││
│     │ └───────────────────────────────────────────────┘   ││
│     │ string (uuid)                                        ││
│     └─────────────────────────────────────────────────────┘│
│                                                              │
│     [Execute]                                               │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Respuesta exitosa

```
┌─────────────────────────────────────────────────────────────┐
│  Responses                                                   │
│                                                              │
│  Server response                                             │
│  Code: 200    Description: Success                          │
│                                                              │
│  Response body                                               │
│  ┌───────────────────────────────────────────────────────┐ │
│  │ []                                                     │ │
│  └───────────────────────────────────────────────────────┘ │
│                                                              │
│  Response headers                                            │
│  content-type: application/json; charset=utf-8              │
│  date: Wed, 10 Jun 2026 02:27:00 GMT                       │
│  server: Kestrel                                            │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 📬 Ejemplo 2: Postman

### Request configurado

```
┌─────────────────────────────────────────────────────────────┐
│  GET  http://localhost:5077/api/tasks?userId={{userId}}    │
│                                                              │
│  Params  Authorization  Headers  Body  Pre-request  Tests   │
│                                                              │
│  Query Params                                                │
│  KEY        VALUE                                   ☐ Bulk   │
│  ─────────────────────────────────────────────────────────  │
│  ✓ userId   {{userId}}                              [...]   │
│             00000000-0000-0000-0000-000000000001            │
│                                                              │
│                                          [Send] [Save ▼]    │
└─────────────────────────────────────────────────────────────┘
```

### Respuesta en Postman

```
┌─────────────────────────────────────────────────────────────┐
│  Response                                                    │
│                                                              │
│  Status: 200 OK    Time: 87 ms    Size: 2 B                │
│                                                              │
│  Body  Cookies  Headers (6)  Test Results                   │
│                                                              │
│  Pretty  Raw  Preview  Visualize                            │
│  ┌───────────────────────────────────────────────────────┐ │
│  │ []                                                     │ │
│  │                                                        │ │
│  │                                                        │ │
│  │                                                        │ │
│  │                                                        │ │
│  └───────────────────────────────────────────────────────┘ │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 🧪 Ejemplo 3: Pruebas con curl

### Request básico

```bash
curl "http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001"
```

**Respuesta:**
```json
[]
```

---

### Request con headers verbose

```bash
curl -v "http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001"
```

**Respuesta:**
```
* Host localhost:5077 was resolved.
* IPv6: ::1
* IPv4: 127.0.0.1
*   Trying [::1]:5077...
* Connected to localhost (::1) port 5077
> GET /api/tasks?userId=00000000-0000-0000-0000-000000000001 HTTP/1.1
> Host: localhost:5077
> User-Agent: curl/8.19.0
> Accept: */*
> 
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Wed, 10 Jun 2026 02:27:00 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
< 
[]
```

---

### Request con formato JSON

```powershell
curl.exe "http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001" `
  -s | ConvertFrom-Json | ConvertTo-Json -Depth 10
```

**Respuesta:**
```json
[]
```

---

## 📊 Ejemplo 4: Diferentes escenarios de respuesta

### ✅ Escenario 1: Usuario sin tareas (actual)

**Request:**
```
GET /api/tasks?userId=00000000-0000-0000-0000-000000000001
```

**Response:**
```json
[]
```

**Status:** `200 OK`

---

### ✅ Escenario 2: Usuario con tareas (futuro - cuando se implemente Create)

**Request:**
```
GET /api/tasks?userId=00000000-0000-0000-0000-000000000001
```

**Response:**
```json
[
  {
    "id": "507f1f77bcf86cd799439011",
    "title": "Implement User Authentication",
    "description": "Create JWT-based authentication system",
    "status": "InProgress",
    "userId": "00000000-0000-0000-0000-000000000001",
    "dueDate": "2026-06-15T23:59:59Z",
    "createdAt": "2026-06-09T21:00:00Z",
    "updatedAt": "2026-06-09T22:15:00Z"
  },
  {
    "id": "507f1f77bcf86cd799439012",
    "title": "Setup Docker Environment",
    "description": "Configure docker-compose for all services",
    "status": "Completed",
    "userId": "00000000-0000-0000-0000-000000000001",
    "dueDate": "2026-06-10T23:59:59Z",
    "createdAt": "2026-06-08T10:00:00Z",
    "updatedAt": "2026-06-09T15:30:00Z"
  }
]
```

**Status:** `200 OK`

---

### ❌ Escenario 3: Endpoint no implementado (POST /api/tasks)

**Request:**
```
POST /api/tasks
Content-Type: application/json

{
  "title": "Test Task",
  "description": "Test Description",
  "status": "Pending",
  "userId": "00000000-0000-0000-0000-000000000001",
  "dueDate": "2026-06-15T23:59:59Z"
}
```

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-abc123..."
}
```

**Status:** `404 Not Found`

**Razón:** El endpoint POST no está implementado aún (Issue #1)

---

### ❌ Escenario 4: userId inválido

**Request:**
```
GET /api/tasks?userId=invalid-guid
```

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "userId": [
      "The value 'invalid-guid' is not valid."
    ]
  },
  "traceId": "00-xyz789..."
}
```

**Status:** `400 Bad Request`

---

## 🎨 Ejemplo 5: Estructura de la colección de Postman

```
BLA Task Management System/
├── Tasks API/
│   ├── Get All Tasks ✅ (Funciona)
│   ├── Get Task By Id 🔲 (404)
│   ├── Create Task 🔲 (404)
│   ├── Update Task 🔲 (404)
│   └── Delete Task 🔲 (404)
└── Users API/
    ├── Register User 🔲 (No iniciada)
    ├── Login 🔲 (No iniciada)
    ├── Get Current User (Me) 🔲 (No iniciada)
    └── Get All Users (No Auth) 🔲 (No iniciada)

Variables:
  userId: 00000000-0000-0000-0000-000000000001
  taskId: (empty)
  jwt_token: (empty)
```

---

## 🔍 Qué observar en las pruebas

### ✅ Indicadores de éxito:

1. **Status Code: 200 OK**
   - La API respondió exitosamente

2. **Response Time: < 200ms**
   - La API es rápida (sin problemas de conexión a DB)

3. **Content-Type: application/json**
   - La respuesta está en el formato correcto

4. **Body: []**
   - Array vacío es correcto (no hay tareas aún)

### ⚠️ Indicadores de problemas:

1. **Status Code: 500 Internal Server Error**
   - Problema con la conexión a MongoDB
   - Verificar que MongoDB esté corriendo

2. **Status Code: 404 Not Found**
   - Endpoint no implementado (esperado para POST, PUT, DELETE)
   - O ruta incorrecta en el request

3. **Connection Refused**
   - La API no está corriendo
   - Verificar que `dotnet run` esté activo

4. **Timeout**
   - MongoDB no responde
   - Verificar `docker ps` para containers

---

## 📝 Checklist de pruebas

- [ ] Swagger UI carga correctamente
- [ ] GET /api/tasks responde 200 OK
- [ ] Response body es un array JSON
- [ ] Puedo cambiar el userId y sigue funcionando
- [ ] userId inválido da error 400
- [ ] POST /api/tasks responde 404 (esperado)
- [ ] Postman collection importada correctamente
- [ ] Variables de entorno funcionan en Postman
- [ ] Response time es razonable (< 200ms)
- [ ] Headers incluyen Content-Type correcto

---

## 🎯 Próximos pasos después de probar

Una vez que hayas verificado que todo funciona:

1. ✅ Familiarízate con las respuestas actuales
2. 🔲 Implementa US-02: Create Task (Issue #1)
3. 🔲 Prueba el nuevo endpoint POST /api/tasks
4. 🔲 Crea tareas de prueba
5. 🔲 Verifica que GET devuelve las tareas creadas
6. 🔲 Continúa con UPDATE, DELETE, GET by ID

---

**Última actualización:** 2026-06-09  
**Documento:** Ejemplos de pruebas Tasks API
