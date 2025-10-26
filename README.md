# 🚌 Bus Ticket Reservation System

A full-stack, spec-driven bus ticket reservation system built with ASP.NET Core, Angular, and PostgreSQL. 
Designed using Clean Architecture and Domain-Driven Design (DDD), 
this system enables users to search for buses, view seat layouts, and book or cancel tickets with real-time updates.

---

## 🚀 Features

- Search buses by route and journey date  
- View seat layout and availability  
- Book/Buy and cancel tickets with real-time updates  
- Responsive Angular UI with intuitive UX  
- xUnit test coverage for booking, cancellation, and seat logic

---

## 🛠 Tech Stack

| Layer       | Technology                  |
|-------------|-----------------------------|
| Frontend    | Angular , TypeScript     |
| Backend     | ASP.NET Core Web API        |
| Database    | PostgreSQL                  |
| ORM         | Entity Framework Core       |
| Testing     | xUnit + InMemoryDatabase    |
| Architecture| Clean Architecture + DDD    |

---

## ⚙️ Setup/Run Instructions (Visual Studio Only)

### 1. Open the Solution

- Open Visual Studio  
- Go to **File > Open > Project/Solution**  
- Select `BusTicketReservationSystem.sln`  
- Confirm the following projects are loaded in **Solution Explorer**:
  - `BusTicketReservationSystem.WebApi`  
  - `BusTicketReservationSystem.Application`
  - `BusTicketReservationSystem.Application.Contracts`
  - `BusTicketReservationSystem.Domain`  
  - `BusTicketReservationSystem.Infrastructure`  
  - `BusTicketReservationSystem.Tests`

---

### 2. Set the Backend as Startup Project

-set `BusTicketReservationSystem.WebApi` as Startup Project


---

### 3. Configure PostgreSQL Connection

- Open `appsettings.json` in `BusTicketReservationSystem.WebApi` 
- Update the `"DefaultConnection"` string:
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=BusTicketDB;Username=youruser;Password=yourpassword"
  }
- Go to Tools > NuGet Package Manager > Package Manager Console

- Set Default Project to `BusTicketReservationSystem.Infrastructure` 

- Type Update-Database and press Enter

### 4. For Seeding Demo Data Run These Scripts on Query Tool in pgAdmin4 

-- Enable UUID generation
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ROUTES: Dhaka to other cities (one-way only)
INSERT INTO "Routes" ("Id", "FromCity", "ToCity", "DistanceKm", "BoardingPoint", "DroppingPoint") VALUES
  (gen_random_uuid(), 'Dhaka', 'Rajshahi', 245, 'Gabtoli', 'Rajshahi Terminal'),
  (gen_random_uuid(), 'Dhaka', 'Chattogram', 295, 'Gabtoli', 'Chattogram Terminal'),
  (gen_random_uuid(), 'Dhaka', 'Sylhet', 240, 'Gabtoli', 'Sylhet Terminal'),
  (gen_random_uuid(), 'Dhaka', 'Khulna', 270, 'Gabtoli', 'Khulna Terminal'),
  (gen_random_uuid(), 'Dhaka', 'Barishal', 180, 'Gabtoli', 'Barishal Terminal'),
  (gen_random_uuid(), 'Dhaka', 'Mymensingh', 120, 'Gabtoli', 'Mymensingh Terminal'),
  (gen_random_uuid(), 'Dhaka', 'Sherpur', 150, 'Gabtoli', 'Sherpur Terminal');

-- BUSES
INSERT INTO "Buses" ("Id", "CompanyName", "BusName", "TotalSeats", "Price") VALUES
  (gen_random_uuid(), 'GreenLine', 'GL Express 1', 40, 850),
  (gen_random_uuid(), 'Shohag', 'Shohag Elite', 40, 900),
  (gen_random_uuid(), 'Hanif', 'Hanif Classic', 40, 780);

-- BUS SCHEDULES: Each route × each bus × multiple dates
DO $$
DECLARE
  route_rec RECORD;
  bus_rec RECORD;
  journey_date DATE;
  journey_dates DATE[] := ARRAY[
    DATE '2025-10-27',
    DATE '2025-10-28',
    DATE '2025-10-29'
  ];
BEGIN
  FOR route_rec IN SELECT "Id" FROM "Routes" LOOP
    FOR bus_rec IN SELECT "Id" FROM "Buses" LOOP
      FOREACH journey_date IN ARRAY journey_dates LOOP
        INSERT INTO "BusSchedules" ("Id", "BusId", "RouteId", "JourneyDate", "StartTime", "ArrivalTime")
        VALUES (
          gen_random_uuid(),
          bus_rec."Id",
          route_rec."Id",
          journey_date,
          journey_date + TIME '08:00:00',
          journey_date + TIME '13:00:00'
        );
      END LOOP;
    END LOOP;
  END LOOP;
END $$;

-- SEATS: 10 rows × 4 seats = 40 seats per bus
DO $$
DECLARE
  bus_rec RECORD;
  i INT;
BEGIN
  FOR bus_rec IN SELECT "Id" FROM "Buses" LOOP
    FOR i IN 1..40 LOOP
      INSERT INTO "Seats" ("Id", "BusId", "SeatNumber", "Row", "Status")
      VALUES (
        gen_random_uuid(),
        bus_rec."Id",
        'A' || i,
        (i - 1) / 4 + 1,
        0
      );
    END LOOP;
  END LOOP;
END $$;

### 5. Adjust API Link in Frontend
   
   - go to 'busticketreservationsystem.clientapp/src/app/services/api.ts'
   - find line 6 and update the link with your api serving port
  
    private baseUrl = 'https://localhost:7196/api'; // adjust to your backend port


### 6. Run the Project

   --   Press F5 or click Start Debugging

   --   Visual Studio will:

        Launch the ASP.NET Core API

        Automatically start the Angular frontend (via launchSettings.json)

        Open Swagger or landing page

        Open Angular app at http://localhost:4200


### 7. Run Unit Tests

  -- Go to Test > Test Explorer

  -- Click Run All Tests ✅ Tests cover:

        BookingTests

        CancellationTests

        SeatStateServiceTests
        
        SearchServiceTests

### 8. NuGet Package List

   --   Backend NuGet Packages
    
        Microsoft.EntityFrameworkCore

        Microsoft.EntityFrameworkCore.Design

        Microsoft.EntityFrameworkCore.Tools

        Npgsql.EntityFrameworkCore.PostgreSQL

        Swashbuckle.AspNetCore

        xUnit, Moq
