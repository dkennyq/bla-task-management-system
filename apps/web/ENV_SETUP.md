# 🎯 Configuración de Entorno - Quick Start

## Archivos Disponibles

### ✅ `.env` - Configuración por Defecto (Docker)
```env
VITE_TASKS_API_URL=http://localhost:5001
VITE_USERS_API_URL=http://localhost:5002
```
**Usar cuando:** Ejecutas todo en Docker

### ✅ `.env.development` - Desarrollo Local
```env
VITE_TASKS_API_URL=http://localhost:5077
VITE_USERS_API_URL=http://localhost:5034
```
**Usar cuando:** Ejecutas APIs desde Visual Studio/VS Code

### ✅ `.env.test` - Docker Testing
```env
VITE_TASKS_API_URL=http://localhost:5001
VITE_USERS_API_URL=http://localhost:5002
```
**Usar cuando:** Ejecutas tests con Docker

## 🚀 Uso Rápido

### Opción 1: Todo en Docker (Recomendado)
```bash
# El archivo .env por defecto ya está configurado
npm run dev
```

### Opción 2: Desarrollo Local (APIs en Visual Studio)
```bash
# Usa .env.development automáticamente
npm run dev
```

### Opción 3: Cambiar de entorno manualmente
```bash
# Forzar modo test
npm run dev -- --mode test

# Forzar modo development
npm run dev -- --mode development
```

## 📊 Tabla de Puertos

| Entorno | Tasks API | Users API |
|---------|-----------|-----------|
| **Docker** | 5001 | 5002 |
| **Local (VS)** | 5077 | 5034 |

## ✅ Verificación

```bash
# Ver que archivo .env se está usando
cat .env

# Probar la app
npm run dev
```

Documentación completa: Ver `env-configuration-guide.md` en la carpeta de sesión.
