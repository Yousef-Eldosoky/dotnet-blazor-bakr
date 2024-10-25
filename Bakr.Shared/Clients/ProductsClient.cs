using System.Net.Http.Json;
using Bakr.Shared.Entities;

namespace Bakr.Shared.Clients;

public class ProductsClient(HttpClient httpClient)
{
    public async Task<Product[]> GetProductsAsync() => await httpClient.GetFromJsonAsync<Product[]>("api/products") ?? [];


    public async Task<Product> GetProductAsync(int id) => await httpClient.GetFromJsonAsync<Product>($"api/products/{id}") ?? throw new Exception("Product not found!"); 

    public async Task AddProductAsync(Product product) => await httpClient.PostAsJsonAsync("api/products", product);

    public async Task UpdateProductAsync(Product product) => await httpClient.PutAsJsonAsync("api/products/"+product.Id, product);

    public async Task DeleteProductAsync(int id) => await httpClient.DeleteAsync("api/products/"+id);
}
