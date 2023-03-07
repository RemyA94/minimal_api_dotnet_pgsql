using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using minimalsAPIPostgreSql.Data;
using minimalsAPIPostgreSql.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<OfficeDb>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

app.MapPost("/Employee/", async (Employee e, OfficeDb db) => {
    db.Employees.Add(e);
    await db.SaveChangesAsync();

    return Results.Created($"/Employee/{e.id}", e);
});


app.MapGet("/Employee/{id:int}", async (int id, OfficeDb db) => {
    return await db.Employees.FindAsync(id)
        is Employee e 
        ? Results.Ok(e)
        : Results.NotFound();
});

app.MapGet("/Employee", async (OfficeDb db) =>   await db.Employees.ToListAsync());


app.MapPut("/Employee/{id:int}", async (int id, Employee e, OfficeDb db) => {
    if(e.id != id)
        return Results.BadRequest();

    var employee  = await db.Employees.FindAsync(id);
    if(employee is null)
        return Results.NotFound();

    employee.FirstName = e.FirstName;
    employee.LastName = e.LastName;
    employee.Branch = e.Branch;
    employee.Age = e.Age;

    await db.SaveChangesAsync();

    return Results.Ok(employee); 
});


app.MapDelete("/Employee/{id:int}", async (int id, OfficeDb db ) => {
        var employee = await db.Employees.FindAsync(id);
        if(employee is null){
            return Results.NotFound();
        }

        db.Employees.Remove(employee);
        await db.SaveChangesAsync();

        return Results.NoContent();
});

app.Run();
