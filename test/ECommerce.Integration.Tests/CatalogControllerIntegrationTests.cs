using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ECommerce.Application.Features.Authentication.Commands.Register;
using ECommerce.Application.Features.Authentication.DTOs;
using ECommerce.Application.Features.Catalog.Content.Commands.CreateProduct;
using ECommerce.Application.Features.Catalog.Content.DTOs;
using ECommerce.Application.Responses;
using Xunit;

namespace ECommerce.Integration.Tests;

public class CatalogControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CatalogControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAndListProducts_ShouldAuthorizeRetrieveAndInvalidateCache()
    {
        // 1. Register a user to get an access token
        var registerCommand = new RegisterCommand
        {
            Email = $"catalog_user_{Guid.NewGuid()}@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Catalog",
            LastName = "User"
        };

        var regResponse = await _client.PostAsJsonAsync("/api/Authentication/register", registerCommand);
        Assert.Equal(HttpStatusCode.OK, regResponse.StatusCode);

        var authDto = await regResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(authDto);
        Assert.False(string.IsNullOrEmpty(authDto.AccessToken));

        // Authenticate client
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authDto.AccessToken);

        // 2. Create a product
        var createCommand = new CreateProductCommand
        {
            Name = "Integration Test Laptop",
            Description = "High specs laptop",
            BasePrice = 999.99m,
            Status = "Active"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/Catalog", createCommand);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(createdProduct);
        Assert.Equal(createCommand.Name, createdProduct.Name);

        // 3. List products (First call - queries database and populates cache)
        var listResponse1 = await _client.GetAsync("/api/Catalog");
        Assert.Equal(HttpStatusCode.OK, listResponse1.StatusCode);
        var result1 = await listResponse1.Content.ReadFromJsonAsync<PagedResult<ProductListItemDto>>();
        Assert.NotNull(result1);
        Assert.Single(result1.Items);

        // 4. List products (Second call - retrieves from cache)
        var listResponse2 = await _client.GetAsync("/api/Catalog");
        Assert.Equal(HttpStatusCode.OK, listResponse2.StatusCode);
        var result2 = await listResponse2.Content.ReadFromJsonAsync<PagedResult<ProductListItemDto>>();
        Assert.NotNull(result2);
        Assert.Single(result2.Items);

        // 5. Create a second product (triggers cache invalidation)
        var createCommand2 = new CreateProductCommand
        {
            Name = "Integration Test Mouse",
            Description = "Wireless mouse",
            BasePrice = 49.99m,
            Status = "Active"
        };
        var createResponse2 = await _client.PostAsJsonAsync("/api/Catalog", createCommand2);
        Assert.Equal(HttpStatusCode.Created, createResponse2.StatusCode);

        // 6. List products again (cache was invalidated, so it returns 2 products from database)
        var listResponse3 = await _client.GetAsync("/api/Catalog");
        Assert.Equal(HttpStatusCode.OK, listResponse3.StatusCode);
        var result3 = await listResponse3.Content.ReadFromJsonAsync<PagedResult<ProductListItemDto>>();
        Assert.NotNull(result3);
        Assert.Equal(2, result3.Items.Count);
    }
}
