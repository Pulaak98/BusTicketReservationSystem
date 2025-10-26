using BusTicketReservationSystem.Application.Contracts.Interfaces;
using BusTicketReservationSystem.Application.Services;
using BusTicketReservationSystem.Infrastructure.Data;
using BusTicketReservationSystem.Infrastructure.Repositories;
using BusTicketReservationSystem.Infrastructure.Unit_of_Work;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBusScheduleRepository, BusScheduleRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<BookingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();


app.MapControllers();

if (app.Environment.IsDevelopment())
{
    var angularClientPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "busticketreservationsystem.clientapp");

    var startInfo = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = "/c start npm start",
        WorkingDirectory = angularClientPath,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    try
    {
        Process.Start(startInfo);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to start Angular CLI: " + ex.Message);
    }
}

app.Run();
