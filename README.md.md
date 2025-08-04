# Sistema de Facturación con Blazor y SQL Server  

Aplicación web para gestión de facturas, productos y clientes, desarrollada con **Blazor Server**, **Entity Framework Core** y **SQL Server LocalDB**.  

## 🛠️ Tecnologías  
- **Frontend**: Blazor Server (.NET 6)  
- **Backend**: ASP.NET Core Web API  
- **Base de datos**: SQL Server LocalDB  
- **ORM**: Entity Framework Core 6  
- **Autenticación**: IdentityServer (configuración básica)  
- **Validaciones**: DataAnnotations  

## 📌 Requisitos  
- **.NET 6 SDK**  
- **SQL Server LocalDB** (incluido en Visual Studio)  
- **Visual Studio 2022** (recomendado) o VS Code con extensión C#  

## 🚀 Instalación  

### 1. Configuración de la Base de Datos  
- Asegúrate de tener **SQL Server LocalDB** instalado (viene con Visual Studio).  
- Modifica el archivo `appsettings.json` con tus credenciales (ya configuradas):  
  ```json
  "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FacturacionApp;User ID=AdmiinFac;Password=1234;TrustServerCertificate=True;Connection Timeout=30;"
  }


  erDiagram
    PRODUCTO ||--o{ DETALLE : "contiene"
    FACTURA ||--o{ DETALLE : "tiene"
    
    PRODUCTO {
        int Id
        string Nombre
        decimal Precio
    }
    
    FACTURA {
        int Id
        string Numero
        datetime Fecha
        string Cliente
        decimal Total
    }
    
    DETALLE {
        int Id
        int FacturaId
        int ProductoId
        int Cantidad
        decimal Precio
    }