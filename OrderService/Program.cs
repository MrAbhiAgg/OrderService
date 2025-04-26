using System.Data.SqlClient;
using Confluent.Kafka.Admin;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using OrderService;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("OrderDbConnection");
var bootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
EnsureDatabaseExists(connectionString);
builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
CreateKafkaTopic("orders-topic", bootstrapServers);
var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();



app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Order}/{action=Index}/{id?}");

app.Run();

void EnsureDatabaseExists(string connectionString)
{
    try
    {
        Console.WriteLine("Connection string " + connectionString);
        const string databaseName = "OrderServiceDB";

        var connectionStringWithoutDb = string.Join(";", connectionString.Split(';')
            .Where(p => !p.TrimStart().StartsWith("Database=", StringComparison.OrdinalIgnoreCase)));

        using var connection = new SqlConnection(connectionStringWithoutDb);
        connection.Open();

        var checkDbQuery = $"""
        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}')
        BEGIN
            CREATE DATABASE [{databaseName}]
        END
    """;

        using (var command = new SqlCommand(checkDbQuery, connection))
        {
            command.ExecuteNonQuery();
        }

        RunDatabaseInitialization(connectionString);
    }
    catch(Exception e)
    {
        Console.WriteLine("My Error 1:- " + e.ToString());
    }
    
}

void RunDatabaseInitialization(string connectionString)
{
    try
    {
        Console.WriteLine("Connection string "+connectionString);
        const string sqlScriptPath = "init-db.sql";

        if (!File.Exists(sqlScriptPath))
        {
            Console.WriteLine("Database initialization script not found.");
            return;
        }

        var script = File.ReadAllText(sqlScriptPath);

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var command = new SqlCommand(script, connection);
        command.ExecuteNonQuery();

        Console.WriteLine("Database initialization script executed successfully.");
    }
    catch (Exception ex) 
    {
        Console.WriteLine("My Error :- "+ex.ToString());
    }
    

}

void CreateKafkaTopic(string topicName, string bootstrapServers)
{
   
    try
    {
        Console.WriteLine("Kafka server: " + bootstrapServers);

        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();
        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
        bool topicExists = metadata.Topics.Any(t => t.Topic == topicName && !t.Error.IsError);

        if (!topicExists)
        {
            adminClient.CreateTopicsAsync(new TopicSpecification[]
            {
            new TopicSpecification
            {
                Name = topicName,
                NumPartitions = 1,
                ReplicationFactor = 1
            }
            }).Wait();

            Console.WriteLine($"Topic '{topicName}' created.");
        }
        else
        {
            Console.WriteLine($"Topic '{topicName}' already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

}