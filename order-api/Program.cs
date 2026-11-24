using Dapper;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/menu", async (NpgsqlDataSource db) =>
{
    await using var conn = await db.OpenConnectionAsync();
    var items = await conn.QueryAsync(
        "SELECT id, name, price, is_available AS isAvailable FROM menu_items ORDER BY id");
    return Results.Ok(items);
});

app.Run();