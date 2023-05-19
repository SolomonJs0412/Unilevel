using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unilever.v1.Database.config;
using System.Net.Http;
using Unilever.v1.Common;
using System.Dynamic;
using Microsoft.Data.SqlClient;

namespace Unilever.v1.Utils
{
    public class CallNotWorkingUser : BackgroundService
    {
        // private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        public readonly ILogger<BackgroundService> _logger;
        public CallNotWorkingUser(IConfiguration configuration, ILogger<BackgroundService> logger)
        {
            // _httpClientFactory = httpClientFactory;
            _config = configuration;
            _logger = logger;
        }


        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var searchQuery = @"SELECT * FROM [User]
                                        WHERE LastLogin < DATEADD(MONTH, -3, GETDATE())";

                    using var connection = new SqlConnection("Server=localhost,1433;Database=UnilevelDB;User Id=sa;Password=Ashleynguci@1412;TrustServerCertificate=True;Encrypt=false");
                    await connection.OpenAsync();

                    using var command = new SqlCommand(searchQuery, connection);

                    var results = new List<object>();
                    var emails = new List<object>();
                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var result = new ExpandoObject() as IDictionary<string, object>;
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            result[reader.GetName(i)] = reader.GetValue(i);
                        }
                        results.Add(result);
                        emails.Add(result["Email"]);
                        Console.WriteLine(result["Email"]);
                        MailerService mailer = new MailerService();
                        string recipient = result["Email"].ToString();
                        string subject = "Welcome to our site!";
                        string body = $"You not use your account 3rd months";
                        mailer.SendMail(recipient, subject, body);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}