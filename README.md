# TiendaRopa

Proyecto de Tienda de Ropa Online desarrollado en **ASP.NET Core Razor Pages** con arquitectura en capas (Domain / Infraestructura / Application / Presentación), usando **Entity Framework Core** y **SQL Server**.

## 🏗️ Arquitectura

La solución está organizada en 4 proyectos:

```
TiendaRopa.sln
│
├── TiendaRopa                   → Presentación (Razor Pages, lo que ve el usuario)
├── TiendaRopa.Application       → Lógica de negocio y validaciones (Servicios)
├── TiendaRopa.Domain            → Entidades puras (modelos)
└── TiendaRopa.Infraestructura   → Acceso a datos (DbContext, EF Core, SQL Server)
```

**Regla de dependencias:** `TiendaRopa` → `Application` → `Infraestructura` → `Domain`
(Domain no depende de nada; cada capa solo conoce las que están "debajo" de ella)

## ⚙️ Cómo configurar el proyecto en tu PC

1. Clona el repositorio:
   ```
   git clone https://github.com/mijaeljs/killers.git
   ```
2. Abre `TiendaRopa.sln` en Visual Studio 2022.
3. Asegúrate de tener **SQL Server Express** corriendo en tu PC (`localhost\SQLEXPRESS`).
4. Cada integrante genera **su propia base de datos local** (no se comparte la BD, solo el código que la crea). Desde la carpeta raíz `TiendaRopa`, ejecuta:
   ```
   dotnet ef database update --project TiendaRopa.Infraestructura --startup-project TiendaRopa
   ```
   Esto crea automáticamente `TiendaRopaDB` con todas las tablas, usando las migraciones ya incluidas en el repositorio.
5. Ejecuta el proyecto con `dotnet run` desde la carpeta `TiendaRopa`, o con F5 en Visual Studio.

> ⚠️ La cadena de conexión está en `TiendaRopa/appsettings.json`. Si tu instancia de SQL Server tiene otro nombre (no es `localhost\SQLEXPRESS`), ajústala ahí.

## ✅ Avance actual — Módulo de Catálogo (completo)

**Entidades (Domain):** `Categoria`, `Talla`, `Color`, `Producto`, `ProductoVariante` (combina producto + talla + color + stock)

**Servicio (Application):** `ProductoServicio` — obtener productos con sus variantes, obtener categorías, registrar producto (con validaciones de nombre y precio)

**Páginas (Razor Pages):**
- `/Productos` — catálogo con tarjetas por producto, mostrando tallas/colores/stock disponibles
- `/Productos/Crear` — formulario para registrar nuevos productos

## 🔜 Pendiente — Módulos por asignar

### Módulo 2: Clientes y Carrito

**Entidades a crear (en `TiendaRopa.Domain`):** `Cliente`, `Carrito`, `ItemCarrito`

- `Cliente`: datos del comprador (nombre, correo, teléfono, etc.)
- `Carrito`: pertenece a un Cliente, contiene varios `ItemCarrito`
- `ItemCarrito`: referencia a una `ProductoVariante` específica (producto + talla + color) y una cantidad

**Servicio a crear (en `TiendaRopa.Application`):** `CarritoServicio` — agregar al carrito, quitar del carrito, editar cantidades, registrar cliente. Debe validar que la cantidad solicitada no supere el `Stock` disponible en la variante.

**Páginas a crear:**
- `/Clientes/Registrar` — registro de cliente
- `/Carrito/Index` — ver contenido del carrito
- Botón "Agregar al carrito" en `/Productos` (deberás editar `Index.cshtml` del catálogo para agregarlo, seleccionando talla/color primero)

### Módulo 3: Pedidos y Checkout

**Entidades a crear (en `TiendaRopa.Domain`):** `Pedido`, `DetallePedido`

- `Pedido`: relacionado con `Cliente`, tiene fecha y estado (ej: "Pendiente", "Confirmado", "Enviado")
- `DetallePedido`: relacionado con `Pedido` y `ProductoVariante`, guarda cantidad y precio al momento de la compra

**Servicio a crear (en `TiendaRopa.Application`):** `PedidoServicio` — convertir un Carrito en un Pedido confirmado (esto debe descontar el `Stock` de cada `ProductoVariante` involucrada), consultar historial de pedidos de un cliente, cambiar estado del pedido (para el admin).

**Páginas a crear:**
- `/Checkout/Index` — confirmar el pedido a partir del carrito actual
- `/Pedidos/Index` — historial de pedidos del cliente
- `/Pedidos/Admin` (opcional) — panel para que el admin vea y cambie el estado de todos los pedidos

## 📌 Convenciones a seguir (importante para que todo encaje)

- Sigue el mismo patrón de capas: entidades en `Domain`, DbContext ya existe en `Infraestructura` (agrega tu `DbSet<>` ahí), lógica de negocio en `Application`, páginas en `TiendaRopa/Pages`.
- Usa `[Key]` en la propiedad Id de cada entidad nueva (con `using System.ComponentModel.DataAnnotations;`).
- Después de crear tus entidades nuevas, genera una migración:
  ```
  dotnet ef migrations add NombreDeTuModulo --project TiendaRopa.Infraestructura --startup-project TiendaRopa
  dotnet ef database update --project TiendaRopa.Infraestructura --startup-project TiendaRopa
  ```
- Registra tu nuevo servicio en `Program.cs` con `builder.Services.AddScoped<TuServicio>();`
- Haz `git pull` antes de empezar a trabajar cada día, para traer los avances de los demás.

## 👥 Equipo

- **Catálogo de productos** — completado
- **Clientes y carrito** — pendiente de asignar
- **Pedidos y checkout** — pendiente de asignar
