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
app.MapPost("/orders", async (CreateOrderRequest req, NpgsqlDataSource db) =>
{
    if (string.IsNullOrWhiteSpace(req.CustomerName) || req.Items is null || req.Items.Count == 0)
        return Results.BadRequest(new { error = "customerName và items là bắt buộc" });

    if (req.Items.Any(i => i.Quantity <= 0))
        return Results.BadRequest(new { error = "quantity phải lớn hơn 0" });

    await using var conn = await db.OpenConnectionAsync();

    var ids = req.Items.Select(i => i.MenuItemId).Distinct().ToArray();
    var available = (await conn.QueryAsync<int>(
        "SELECT id FROM menu_items WHERE id = ANY(@ids) AND is_available = TRUE",
        new { ids })).ToHashSet();
    var invalid = ids.Where(id => !available.Contains(id)).ToList();
    if (invalid.Count > 0)
        return Results.BadRequest(new { error = "Món không tồn tại hoặc hết hàng", menuItemIds = invalid });

    await using var tx = await conn.BeginTransactionAsync();
    var orderId = await conn.ExecuteScalarAsync<int>(
        "INSERT INTO orders (customer_name) VALUES (@name) RETURNING id",
        new { name = req.CustomerName }, tx);

    foreach (var item in req.Items)
    {
        await conn.ExecuteAsync(
            "INSERT INTO order_items (order_id, menu_item_id, quantity) VALUES (@orderId, @menuItemId, @quantity)",
            new { orderId, item.MenuItemId, item.Quantity }, tx);
    }
    await tx.CommitAsync();

    return Results.Created($"/orders/{orderId}", new { id = orderId, status = "Pending" });
});
app.Run();
record CreateOrderRequest(string CustomerName, List<OrderItemRequest> Items);
record OrderItemRequest(int MenuItemId, int Quantity);