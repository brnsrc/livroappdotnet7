using Microsoft.EntityFrameworkCore; //ToQueryString, GetonnectionString
using WorkingWithEFCore2.Models; //Northwind
string server = @"bruno-desk\SQLEXPRESS";
//"."; //Sql Server for windows
// @".\net7book"; //
// "tcp:apps-services-net7.database.windows.net,1433"; // Azure SQL Database
//"tcp:127.0.0.1,1433"; // Azure SQL Edge

//to use SQL Server authentication
string username = "bruno"; //Azure SQL Edge
Write("Enter your SQL Server password: ");
string? password = ReadLine();

if (password == null)
{
    WriteLine("Password cannot be null!");
    return;
}

string connectionString = $"Server = {server};" +
"Initial Catalog = Northwind;" +
//to use SQL authentication
"Persist Security Info = False;" +
$"User ID = {username}; Password = {password};" +
//to use Windows authentication
// "Integrated Security = True;"
//other options
"MultipleActiveResultsSets=True;" +
"Encrypt = True; TrustServerCertificate=False;" +
"Connection Timeout = 30;";

DbContextOptionsBuilder<Northwind> builder = new();
builder.UseSqlServer(connectionString);
using (Northwind db = new(builder.Options))
{
    Write("Enter a unit price: ");
    string? priceText = ReadLine();
    if (!decimal.TryParse(priceText, out decimal price))
    {
        WriteLine("You must enter a valid unit price.");
        return;
    }

    var products = db.Products.Where(p => p.UnitPrice > price)
        .Select(p => new { p.ProductId, p.ProductName, p.UnitPrice });
    WriteLine("----------------------------------------------------------");
    WriteLine(
        "| {0,5} | {1,-35} | {2,8} |", "Id", "Name", "Price");
    WriteLine("----------------------------------------------------------");
    foreach (var p in products)
    {
        WriteLine("| {0,5} | {1,-35} | {2,8:C} |",
            p.ProductId, p.ProductName, p.UnitPrice);
    }
    WriteLine("----------------------------------------------------------");
    WriteLine(products.ToQueryString());
    WriteLine();
    WriteLine($"Provider: {db.Database.ProviderName}");
    WriteLine($"Connection: {db.Database.GetConnectionString()}");
}