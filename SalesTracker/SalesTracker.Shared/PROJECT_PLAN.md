# Sales Tracker and Inventory Management System

## Project Overview
A comprehensive sales tracking and inventory management application built with .NET 10 using a shared codebase.

**Architecture**: Offline-first, cross-platform application with platform-specific data storage:
- **Native Apps** (macOS, iOS, Android, Windows): .NET MAUI + Blazor Hybrid + SQLite
- **Web App** (Browser): Blazor WebAssembly + IndexedDB

All data and business logic runs locally on the device/browser. Cloud synchronization can be added as a future enhancement.

**Shared Code**: Blazor components, business logic, and models are shared across all platforms.

---


- 
## Features Overview

### 1. Dashboard
The main landing page displaying key business metrics:
- **Total Sales Month** - Current month sales with comparison to last month (% change)
- **Net Profit** - Calculated profit based on (Sale Price - Cost) * Qty
- **Pending Deliveries** - Count of orders where `HasDelivered = false`
- **Backordered Items** - Items where `Allocated Inventory Qty > Current Inventory Qty`

### 2. Sales Management
#### New Sale
- Form to create new orders
- Input fields for all order properties
- Real-time inventory validation
- Automatic inventory allocation on order creation

#### View Sales
- Searchable/filterable list of all orders
- Filter by:
  - Date range
  - Customer name
  - Payment status
  - Delivery status
- Sort by various fields
- Edit existing orders
- Quick actions (mark as paid, mark as delivered)

### 3. Inventory Management
#### New Item / Edit Item
- Form to create/update item information
- Image upload capability (multiple images per item)
- Pricing configuration
- Initial inventory setup

#### View Items
- **Search** - Search by ItemID, Name, Description
- **Display List** - Grid/List view with:
  - Sorting capabilities
  - Filtering options
  - Stock level indicators (low stock warnings)
  - Quick edit access

---

## Data Models

### Order Entity
```
- OrderID (Primary Key, Auto-generated)
- CustomerName (string, required)
- ItemID (Foreign Key to Items)
- SellDate (DateTime, can be past or future)
- Price (decimal, sale price at time of order)
- Qty (int, quantity ordered)
- HasReceivedPayment (bool, default: false)
- HasDelivered (bool, default: false)
- DeliveryDate (DateTime?, nullable)
- PaymentDate (DateTime?, nullable)
- CreatedDate (DateTime, auto-generated)
- ModifiedDate (DateTime, auto-updated)
```

### Item Entity
```
- ItemID (Primary Key, Auto-generated or custom)
- Name (string, required)
- Description (string, optional)
- SalePrice (decimal?, nullable - allows flexible pricing)
- Images (List<string> or separate Images table, paths/URLs)
- Cost (decimal, required for profit calculation)
- CurrentInventoryQty (int, actual stock on hand)
- AllocatedInventoryQty (int, reserved for orders)
- AvailableInventoryQty (computed: Current - Allocated)
- CreatedDate (DateTime)
- ModifiedDate (DateTime)
```

### ItemImage Entity (if normalized)
```
- ImageID (Primary Key)
- ItemID (Foreign Key)
- ImagePath (string)
- IsPrimary (bool)
- DisplayOrder (int)
```

---

## Technical Architecture

### Project Structure
```
SalesTracker.sln
├── SalesTracker.Shared/              # Shared code (Blazor components, models, interfaces)
├── SalesTracker.BusinessLogic/       # Business logic and service interfaces
├── SalesTracker.Data.Abstractions/   # Data access interfaces (IDataStore)
├── SalesTracker.Data.Sqlite/         # SQLite implementation (for native apps)
├── SalesTracker.Data.IndexedDb/      # IndexedDB implementation (for web)
├── SalesTracker.Maui/                # MAUI app (iOS, Android, macOS, Windows)
└── SalesTracker.Web/                 # Blazor WebAssembly app
```

### Blazor Components (Shared across all platforms)
- **Components** (in `SalesTracker.Shared/Components/`):
  - Dashboard widgets (KPI cards, charts)
  - Order forms and lists
  - Item management forms and grids
  - Search and filter components
  - Image upload component
  
- **Pages** (in `SalesTracker.Shared/Pages/`):
  - `/` - Dashboard
  - `/sales/new` - New Sale Form
  - `/sales/view` - Sales List
  - `/items/new` - New Item Form
  - `/items/edit/{id}` - Edit Item Form
  - `/items/view` - Items List

### Business Logic Layer (Shared Services)
- **Services** (in `SalesTracker.BusinessLogic/`, injected via DI):
  - `OrderService` - Business logic for orders
  - `ItemService` - Business logic for items
  - `InventoryService` - Inventory calculations and allocation
  - `DashboardService` - Analytics and reporting
  - `ImageService` - Image handling and local storage
  - `SyncService` - (Future) Cloud synchronization

### Data Abstraction Layer
- **Interface** `IDataStore<T>` (in `SalesTracker.Data.Abstractions/`):
  ```csharp
  public interface IDataStore<T> where T : class
  {
      Task<List<T>> GetAllAsync();
      Task<T> GetByIdAsync(int id);
      Task<int> AddAsync(T entity);
      Task UpdateAsync(T entity);
      Task DeleteAsync(int id);
      Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate);
  }
  ```

### Platform-Specific Data Implementations

#### SQLite Implementation (Native Apps)
- **Project**: `SalesTracker.Data.Sqlite`
- **Technology**: Entity Framework Core with SQLite provider
- **DbContext**: `SalesTrackerDbContext`
- **Storage Location**: 
  - Android: `/data/data/{package}/files/salestracker.db3`
  - iOS: `Library/Application Support/salestracker.db3`
  - Windows: `%LOCALAPPDATA%/SalesTracker/salestracker.db3`
  - macOS: `~/Library/Application Support/SalesTracker/salestracker.db3`

#### IndexedDB Implementation (Web App)
- **Project**: `SalesTracker.Data.IndexedDb`
- **Technology**: JavaScript Interop with IndexedDB API
- **Library**: `Blazor.IndexedDB.Framework` or custom JS interop
- **Database Name**: `SalesTrackerDB`
- **Object Stores**: `Orders`, `Items`, `ItemImages`
- **Storage Location**: Browser's IndexedDB (varies by browser)

### Platform-Specific Projects

#### MAUI App (`SalesTracker.Maui`)
- Uses Blazor Hybrid (BlazorWebView)
- References SQLite data implementation
- Platform-specific features:
  - Camera access for item photos
  - File picker for image selection
  - Native sharing/export

#### Blazor WebAssembly App (`SalesTracker.Web`)
- Pure web app, runs in browser
- References IndexedDB data implementation
- Progressive Web App (PWA) capable
- Features:
  - File input for image uploads
  - Browser-based image capture (where supported)
  - Download/export functionality

### Dependency Injection Setup

#### MAUI (MauiProgram.cs)
```csharp
builder.Services.AddSingleton<IDataStore<Order>, SqliteDataStore<Order>>();
builder.Services.AddSingleton<IDataStore<Item>, SqliteDataStore<Item>>();
// Register other services...
```

#### Blazor Web
```csharp
builder.Services.AddScoped<IDataStore<Order>, IndexedDbDataStore<Order>>();
builder.Services.AddScoped<IDataStore<Item>, IndexedDbDataStore<Item>>();
// Register other services...
```

### State Management
- Blazor state containers for:
  - User preferences (stored in local storage/SQLite)
  - Cached items list
  - Current order being edited

---

## Business Logic Rules

### Inventory Management
1. When an order is created:
   - Increase `AllocatedInventoryQty` by order `Qty`
   - Validate `AvailableInventoryQty >= Qty` before allowing order

2. When an order is delivered:
   - Decrease both `AllocatedInventoryQty` and `CurrentInventoryQty` by order `Qty`
   - Set `HasDelivered = true`
   - Set `DeliveryDate = Now` if not already set

3. When an order is cancelled:
   - Decrease `AllocatedInventoryQty` by order `Qty`
   - Return inventory to available pool

### Dashboard Calculations
- **Total Sales Month**: `SUM(Price * Qty)` where `SellDate` in current month
- **Last Month Comparison**: Compare with previous month's total
- **Net Profit**: `SUM((Price - Item.Cost) * Qty)` for completed orders
- **Pending Deliveries**: `COUNT(*)` where `HasDelivered = false` and `SellDate <= Today`
- **Backordered Items**: Items where `AllocatedInventoryQty > CurrentInventoryQty`

### Validation Rules
- `DeliveryDate` must be >= `SellDate`
- `PaymentDate` should be set when `HasReceivedPayment = true`
- `Price` should default to `Item.SalePrice` if available, otherwise manual entry


---

## UI/UX Considerations

### Dashboard
- Use card-based layout for KPIs
- Display trend indicators (↑ ↓) with color coding (green/red)
- Chart for sales over time (last 6-12 months)
- Quick action buttons to common tasks

### Sales Management
- Autocomplete for `CustomerName` (from previous orders)
- Item picker with search and image preview
- Real-time price calculation display
- Status badges (Paid/Unpaid, Delivered/Pending)
- Inline editing for quick updates

### Inventory Management
- Grid view with column sorting and filtering
- Low stock indicators (e.g., red badge when `AvailableInventoryQty < 10`)
- Image gallery for multiple item images
- Bulk edit capabilities
- Export to CSV functionality

### Responsive Design
- Mobile-first approach
- Touch-friendly buttons and forms
- Collapsible sections on mobile
- Optimized image loading

---

## Platform-Specific Implementation Details

### Data Access Pattern
All services use the `IDataStore<T>` interface, making them platform-agnostic:

```

### Image Storage Strategy

#### Native Apps (MAUI)
- Images saved to device file system
- Path stored in database: `"/images/item_123_1.jpg"`
- Full path constructed at runtime: `{AppDataPath}/images/item_123_1.jpg`
- Advantages: No size limits, better performance

#### Web App (Blazor WebAssembly)
Store as Blobs in IndexedDB
  - Pro: More efficient storage
  - Con: Requires additional JS interop


```

### Key Architectural Benefits

✅ **Code Reuse**: Write UI once, runs everywhere
✅ **Consistent UX**: Same look and feel across all platforms
✅ **Single Business Logic**: No duplicate code maintenance
✅ **Easy Testing**: Test shared logic once
✅ **Platform Optimization**: Use best storage for each platform
✅ **Future-Proof**: Easy to add new platforms or cloud sync

### Considerations for Cloud Sync

When adding cloud sync later, the architecture remains the same:
- Services still use `IDataStore<T>` interface
- Add a `SyncService` that reads from local store and syncs to cloud
- Both SQLite and IndexedDB track `IsSynced` flag
- Sync logic is platform-agnostic
- No changes to business logic or UI components

---

## Future Enhancements (Phase 2)
- **Cloud synchronization** with conflict resolution
- Multi-device support (sync across user's devices)
- Cloud backup and restore
- Multi-user support with authentication
- Customer management module
- Invoice generation and printing (PDF export)
- Email notifications for low stock
- Barcode scanning for items
- Advanced reports and analytics
- Integration with payment gateways
- Supplier management
- Purchase order tracking
- Multi-location inventory support
- Export data to Excel/CSV

## Technology Stack

### Shared Components
- .NET 10
- Blazor (components and pages shared across platforms)
- Bootstrap or Fluent UI
- Blazorise or MudBlazor (component library)
- Chart.js via Blazor wrappers (e.g., ChartJs.Blazor)

### Native Apps (MAUI)
- .NET 10 MAUI
- Blazor Hybrid (BlazorWebView)
- SQLite with Entity Framework Core
- Microsoft.EntityFrameworkCore.Sqlite
- Platform-specific APIs (camera, file system, etc.)

### Web App (Blazor WebAssembly)
- Blazor WebAssembly (.NET 10)
- IndexedDB via JavaScript Interop
- Blazor.IndexedDB.Framework or TG.Blazor.IndexedDB
- Progressive Web App (PWA) manifest
- Service Worker for offline capabilities

### Additional Libraries
- SixLabors.ImageSharp (image processing and thumbnails)
- CommunityToolkit.Mvvm (helpers for data binding)
- Serilog (logging)
- Bogus (test data generation)
- FluentValidation (input validation)


###UI Style
- Blazor Bootstrap
---

## Notes
- **Offline-first means**: App works 100% without internet connection on all platforms
- **Shared codebase**: ~80-90% of code is shared between native and web apps
- **Platform-specific**: Only data storage and file I/O differ between platforms
- All business logic runs locally - no API calls needed
- **Images**:
  - Native apps: Stored in local file system
  - Web app: Stored as Base64 strings in IndexedDB or Blob URLs
- **Database sync**: SQLite and IndexedDB have different APIs but same logical schema
- **Migration consideration**: Plan for schema versioning for both SQLite and IndexedDB
- Use dependency injection to swap between SQLite and IndexedDB implementations
- Services are platform-agnostic and work with `IDataStore<T>` interface
- Add comprehensive unit tests for shared business logic
- Consider adding sample/demo data on first launch (both platforms)
- **PWA features** (web app): Installable, offline-capable, app-like experience
- **Publishing**:
  - MAUI: App stores (Apple App Store, Google Play, Microsoft Store, Mac App Store)
  - Web: Static hosting (Azure Static Web Apps, Netlify, Vercel, GitHub Pages)


##Development Phase Plan

### Phase 1: Core Functionality
- Create/Edit/View Items
- Create/Edit/View Orders
- Allow Image uploads for items

### Phase 2: Dashboard and Analytics
- Implement dashboard with key metrics
- Add charts for sales trends

### Phase 3: Search and Filtering
- Implement search and filter capabilities for orders and items