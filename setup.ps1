# setup.ps1
# Verifica los prerequisitos para correr el proyecto en C:\Proyectos\SalonReservas

Write-Host "=== Verificando prerequisitos ===" -ForegroundColor Cyan

function Test-Cmd {
    param([string]$cmd, [string]$nombre)
    $existe = Get-Command $cmd -ErrorAction SilentlyContinue
    if ($existe) {
        Write-Host "[OK] $nombre encontrado: $($existe.Source)" -ForegroundColor Green
        return $true
    } else {
        Write-Host "[X]  $nombre NO encontrado" -ForegroundColor Red
        return $false
    }
}

$dotnetOk = Test-Cmd "dotnet" ".NET SDK"
$nodeOk   = Test-Cmd "node" "Node.js"
$dockerOk = Test-Cmd "docker" "Docker"

Write-Host ""
Write-Host "=== Opciones para ejecutar ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Backend (sin Docker):"
Write-Host "   cd backend"
Write-Host "   dotnet restore"
Write-Host "   dotnet run --project src/RoomReservations.Api"
Write-Host "   -> http://localhost:5080/swagger"
Write-Host ""
Write-Host "2. Frontend (sin Docker):"
Write-Host "   cd frontend"
Write-Host "   npm install"
Write-Host "   npm run dev"
Write-Host "   -> http://localhost:5173"
Write-Host ""
Write-Host "3. Todo con Docker:"
Write-Host "   docker compose up --build"
Write-Host "   -> Frontend: http://localhost:8080"
Write-Host "   -> API:      http://localhost:5080/swagger"
Write-Host ""
Write-Host "4. Tests:"
Write-Host "   cd backend"
Write-Host "   dotnet test"
