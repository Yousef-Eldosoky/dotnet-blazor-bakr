using Bakr.Data;
using Bakr.Shared.Dtos;
using Bakr.Shared.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Endpoints;

public static class GenresEndpoint
{
    public static RouteGroupBuilder MapGenreEndpoint(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/genres").WithParameterValidation().RequireAuthorization();

        group.MapGet("/", GetGenresAsync);
        
        group.MapGet("/{id:int}", GetGenreAsync).WithName(nameof(GetGenreAsync));
        
        group.MapPost("/", PostGenreAsync).RequireAuthorization("AdminPolicy");
        
        group.MapPut("/{id:int}", PutGenreAsync).RequireAuthorization("AdminPolicy");
        
        group.MapDelete("/{id:int}", DeleteGenreAsync).RequireAuthorization("AdminPolicy");

        return group;
    }
    
    public static async Task<Ok<List<Genre>>> GetGenresAsync(ApplicationDbContext dbContext)
    {
        return TypedResults.Ok(await dbContext.Genres.AsNoTracking().ToListAsync());
    }
    
    private static async Task<Results<NotFound, Ok<Genre>>> GetGenreAsync(int id, ApplicationDbContext dbContext)
    {
        Genre? genre = await dbContext.Genres.FindAsync(id);
        if (genre is null) return TypedResults.NotFound();
        return TypedResults.Ok(genre);
    }
    
    private static async Task<CreatedAtRoute> PostGenreAsync(CreateGenreDto newGenre, ApplicationDbContext dbContext)
    {
        Genre genre = new()
        {
            Name = newGenre.Name
        };
        dbContext.Genres.Add(genre);
        await dbContext.SaveChangesAsync();
        return TypedResults.CreatedAtRoute(nameof(GetGenreAsync), new { id = genre.Id });
    }
    
    private static async Task<Results<NotFound, NoContent>> PutGenreAsync(int id, CreateGenreDto newGenre, ApplicationDbContext dbContext)
    {
        Genre? genre = await dbContext.Genres.FindAsync(id);
        if (genre is null) return TypedResults.NotFound();
        genre.Name = newGenre.Name;
        dbContext.Genres.Entry(genre).CurrentValues.SetValues(genre);
        await dbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    
    private static async Task<NoContent> DeleteGenreAsync(int id, ApplicationDbContext dbContext)
    {
        await dbContext.Genres.Where(genre => genre.Id == id).ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
}
