using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using Microsoft.Extensions.Configuration;

namespace HabitsApp.Infrastructure.Services;
public class GeminiService : IGeminiService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;

    public GeminiService(IConfiguration configuration, HttpClient httpClient)
    {
        this.configuration = configuration;
        this.httpClient = httpClient;
    }

    public async Task<string> SendPromptAsync(string prompt)
    {
        var apiKey = configuration["Gemini:ApiKey"];
        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    // Sistem talimatı/rol ayarı
                    parts = new[]
                    {
                        new { text = "You are HabitBot, a friendly and motivational assistant inside the HabitFlux habit tracking app. Help users build and maintain positive habits related to health, productivity, and focus. Always respond in the same language the user writes in. Keep answers supportive, concise, and practical." }
                    },
                    role = "user" // Veya "model", eğer bu çoklu dönüşümlü bir konuşmanın parçasıysa ve önceki bir model yanıtını ekliyorsanız
                },
                    // Kullanıcının gerçek istemi
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    },
                    role = "user" // Bu, 'prompt' içeriğinin kullanıcıdan geldiğini belirtir
                }
                    }
                };

        var response = await httpClient.PostAsJsonAsync(endpoint, body);

        if (!response.IsSuccessStatusCode)
        {
            return "";
        }

        var json = await response.Content.ReadAsStringAsync();
        var parsed = JsonDocument.Parse(json);

        var content = parsed.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();


        return content ?? "";
    }
};
