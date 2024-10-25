using System.Net.Http.Json;
using Bakr.Shared.Entities;

namespace Bakr.Shared.Clients;

public class GenresClient(HttpClient httpClient)
{
    public async Task<Genre[]> GetGenresAsync() => await httpClient.GetFromJsonAsync<Genre[]>("genres") ?? [];
}
