using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Lägg till tjänster för Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  skapar EncryptionService som en singleton-tjänst
builder.Services.AddSingleton<EncryptionService>();

var app = builder.Build();

// Använder Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Krypterar endpoint
app.MapPost("/encrypt", (EncryptionService encryptionService, string plaintext) =>
{
    return Results.Ok(new { encryptedText = encryptionService.Encrypt(plaintext) });
});

// Avkrypterar endpoint
app.MapPost("/decrypt", (EncryptionService encryptionService, string encryptedText) =>
{
    return Results.Ok(new { decryptedText = encryptionService.Decrypt(encryptedText) });
});

app.Run();

public class EncryptionService
{
    private readonly string[] summaries = new[]
    {
        "mascara", "lipgloss", "bronzer", "blush", "consealer", "foundation"
    };

    public string Encrypt(string plaintext)
    {
        // Kontrollerar så att endast tillåtna ord används
        if (!summaries.Contains(plaintext, StringComparer.OrdinalIgnoreCase))
        {
            return "You typed wrong, try again!";
        }

        byte[] textAsBytes = Encoding.UTF8.GetBytes(plaintext);
        return Convert.ToBase64String(textAsBytes);
    }

    public string Decrypt(string encryptedText)
    {
        byte[] textAsBytes = Convert.FromBase64String(encryptedText);
        string decodedText = Encoding.UTF8.GetString(textAsBytes);

        // Kontrollerar om det avkrypterade ordet är något av dem tillåtna orden
        if (!summaries.Contains(decodedText, StringComparer.OrdinalIgnoreCase))
        {
            return "You typed wrong, try again!";
        }

        return decodedText;
    }
}
