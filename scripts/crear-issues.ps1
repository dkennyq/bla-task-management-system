# Script para crear GitHub Issues desde USER_STORIES.md
# Ejecutar: .\scripts\crear-issues.ps1

Write-Host " Creando GitHub Issues desde USER_STORIES.md..." -ForegroundColor Cyan

# Verificar que gh CLI esté instalado
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host " GitHub CLI (gh) no está instalado." -ForegroundColor Red
    Write-Host "Instalar con: choco install gh" -ForegroundColor Yellow
    exit 1
}

# Verificar autenticación
gh auth status 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host " No estás autenticado en GitHub CLI." -ForegroundColor Red
    Write-Host "Ejecutar: gh auth login" -ForegroundColor Yellow
    exit 1
}

Write-Host " GitHub CLI configurado correctamente" -ForegroundColor Green
Write-Host ""
Write-Host " Este script creará 12 issues en GitHub" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para más información, ver: docs/GITHUB_TASKS_SETUP.md" -ForegroundColor Yellow
