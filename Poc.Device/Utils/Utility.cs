using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;

namespace Poc.Device.Utils;

public static class Utility
{
   private static readonly X509Certificate2 Certificate = new ("Certs/modelOneClient1.pfx", "passW0rd!");

    public static X509Certificate2 GetX509CertCertificate => Certificate;
    
    public static string HmacSha256Generator(string message)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(Certificate.SerialNumber);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmacsha256 = new HMACSHA256(keyBytes);
        byte[] hash = hmacsha256.ComputeHash(messageBytes);

        return Convert.ToBase64String(hash);
    }
    
    internal static HubConnection InitConnectionBuilder( string? baseUrl)
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(GetX509CertCertificate);
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        
        var connection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}/Hub/LogHub", 
                options =>
                {
                    options.Headers.Add("Client-Id",GetX509CertCertificate.Thumbprint);
                    options.HttpMessageHandlerFactory = _ => handler;
                })
            .WithAutomaticReconnect()
            .Build();

        connection.On<string>("ReceiveMessage", message =>
        {
            Console.WriteLine($"Received: {message}");
        });
        
        return connection;
    }
}