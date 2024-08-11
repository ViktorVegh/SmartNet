using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Models;
using IClients;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Models.DTOs;

public class UserClient : IUserClient
{
    private readonly HttpClient _httpClient;

    public UserClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:8080/");
    }

    public async Task<User> GetUserByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"/user/{id}");
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new ContentJsonConverter()
            }
        };

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(jsonResponse, options);
        
        Console.WriteLine($"User: {user.LearningMaterials.Count}");

        return user;
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        var response = await _httpClient.GetAsync($"/user/username/{username}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new ContentJsonConverter()
            }
        };

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(jsonResponse, options);
        Console.WriteLine($"User: {user}");

        return user;
    }
    
    public async Task<UserDto> GetUserDtoByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"/user/dto/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
    
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var userDto = JsonSerializer.Deserialize<UserDto>(jsonResponse);
    
        return userDto;
    }
    

}