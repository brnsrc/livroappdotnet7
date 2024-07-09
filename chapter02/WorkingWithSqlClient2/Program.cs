using Microsoft.Data.SqlClient;
using System.Data; //Command Type

string server = @"bruno-desk\SQLEXPRESS"; // SQL Server for Windows bruno-desk\SQLEXPRESS
                                          // "."; // SQL Server for Windows
                                          // "tcp:apps-services-net7.database.windows.net,1433"; // Azure SQL Database
                                          //"tcp:127.0.0.1,1433"; // Azure SQL Edge

string username = "bruno";
Write("Enter your SQL Server password: ");
string? password = ReadLine();
if (string.IsNullOrWhiteSpace(password))
{
    WriteLine("Password cannot be empty or null!");
    return;
}

string connectionString = $"Server={server}; Database=Northwind;" +
    "Persist Security Info=true; " +
    // to use SQL authentication
    $"User ID={username}; Password={password};" +

     // to use Windows authentication
     "Integrated Security=false;" +
    // other options
    "Encrypt = true; TrustServerCertificate=true;" +
    "Connection Timeout=30;";

SqlConnection connection = new(connectionString);
connection.StateChange += Connection_StateChange;
connection.InfoMessage += Connection_InfoMessage;
try
{
    await connection.OpenAsync();
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
    // cmd.CommandText = "SELECT ProductId, ProductName, UnitPrice " +
    // "FROM Products WHERE UnitPrice > @price";
    // cmd.Parameters.AddWithValue("price", price);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.CommandText = "GetExpensiveProducts2";
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
    WriteLine($"Output count: {p2.Value}");
    WriteLine($"Return Value: {p3.Value}");
    WriteLine("--------------------------------------------------");
    WriteLine("| {0,5} | {1,-35} | {2,8} |", "Id", "Name", "Price");
    WriteLine("--------------------------------------------------");
    while (await r.ReadAsync())
    {
        WriteLine("| {0,5} | {1,-35} | {2,8:C} |",
        await r.GetFieldValueAsync<int>("ProductId"),
        await r.GetFieldValueAsync<string>("ProductName"),
        await r.GetFieldValueAsync<decimal>("UnitPrice"));
    }

    WriteLine("--------------------------------------------------");
    await r.CloseAsync();
    WriteLine($"Output count: {p2.Value}");
    WriteLine($"Return Value: {p3.Value}");
}
catch (SqlException ex)
{

    WriteLine($"SQL exception: {ex.Message}");
    return;
}
await connection.CloseAsync();

