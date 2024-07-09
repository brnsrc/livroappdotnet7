using System.Data;
using System.Net;
using Microsoft.Data.SqlClient;
using WorkingWithSqlClient;

string server = @"BRUNO-DESK\SQLEXPRESS";
string username = "bruno";

Write("Enter your SQL Server password: ");
string? password = ReadLine();
string connectionString = $"Server={server};" +
"Initial Catalog=NorthWind;" +
"Persist Security Info = False;" +
//"TrustServerCertificate=True" +
$"User ID= {username};" +
$"Password={password};" +
"Integrated Security = True;" +
"Encrypt = False;";
SqlConnection connection = new(connectionString);
connection.StateChange += EventHandlers.Connection_StateChange;
connection.InfoMessage += EventHandlers.Connection_InfoMessage;

try
{
    await connection.OpenAsync();
    // connection.Open();
    WriteLine($"SQL Server version: {connection.ServerVersion}");
    Write("Enter a unit price: ");
    string? priceText = ReadLine();
    if (!decimal.TryParse(priceText, out decimal price))
    {
        WriteLine("You must enter a valid unit price.");
        return;
    }
    SqlCommand cmd = connection.CreateCommand();
    // cmd.CommandType = CommandType.Text;
    // cmd.CommandText = "SELECT ProductId, ProductName, UnitPrice FROM Products"
    // + " WHERE UnitPrice > @price";
    // cmd.Parameters.AddWithValue("price", price);

    cmd.CommandType = CommandType.StoredProcedure;
    cmd.CommandText = "GetExpensiveProducts";
    SqlParameter p1 = new()
    {
        ParameterName = "price",
        SqlDbType = SqlDbType.Money,
        SqlValue = price
    };
    SqlParameter p2 = new()
    {
        Direction = ParameterDirection.Output,
        ParameterName = "count",
        SqlDbType = SqlDbType.Int
    };

    SqlParameter p3 = new()
    {
        Direction = ParameterDirection.ReturnValue,
        ParameterName = "rv",
        SqlDbType = SqlDbType.Int
    };

    cmd.Parameters.Add(p1);
    cmd.Parameters.Add(p2);
    cmd.Parameters.Add(p3);


    SqlDataReader r = await cmd.ExecuteReaderAsync();
    WriteLine("--------------------------------------------");
    WriteLine("| {0,5} | {1,-35} | {2,8} |", "Id", "Name", "Price");
    WriteLine("--------------------------------------------");
    while (await r.ReadAsync())
    {
        WriteLine("| {0,5} | {1,-35} | {2,8 } |",
        await r.GetFieldValueAsync<int>("ProductId"),
        await r.GetFieldValueAsync<string>("ProductName"),
        await r.GetFieldValueAsync<decimal>("UnitPrice"));
    }
    WriteLine("--------------------------------------------");
    await r.CloseAsync();
    WriteLine($"Output count: {p2.Value}");
    WriteLine($"Return value: {p3.Value}");
}
catch (SqlException ex)
{
    WriteLine($"SQL Exception: {ex.Message}");
    return;
}
await connection.CloseAsync();