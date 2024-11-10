using Bakr.Data;
using Bakr.Shared.Dtos;
using Bakr.Shared.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Endpoints;

public static class GenresEndpoint
{
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
    public static RouteGroupBuilder MapGenreEndpoint(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/genres").WithParameterValidation().RequireAuthorization();

        group.MapGet("/", GetGenresAsync);


        group.MapGet("/{id:int}", GetGenreAsync).WithName(nameof(GetGenreAsync));

        group.MapPost("/", async Task<CreatedAtRoute> (CreateGenreDto newGenre, ApplicationDbContext dbContext) =>
        {
            Genre genre = new()
            {
                Name = newGenre.Name
            };
            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();
            return TypedResults.CreatedAtRoute(nameof(GetGenreAsync), new { id = genre.Id });
        }).RequireAuthorization("AdminPolicy"); ;


        group.MapPut("/{id:int}", async Task<Results<NotFound, NoContent>> (int id, CreateGenreDto newGenre, ApplicationDbContext dbContext) =>
        {
            Genre? genre = await dbContext.Genres.FindAsync(id);
            if (genre is null) return TypedResults.NotFound();
            genre.Name = newGenre.Name;
            dbContext.Genres.Entry(genre).CurrentValues.SetValues(genre);
            await dbContext.SaveChangesAsync();
            return TypedResults.NoContent();
        }).RequireAuthorization("AdminPolicy"); ;


        group.MapDelete("/{id:int}", async (int id, ApplicationDbContext dbContext) =>
        {
            await dbContext.Genres.Where(genre => genre.Id == id).ExecuteDeleteAsync();
            return TypedResults.NoContent();
        }).RequireAuthorization("AdminPolicy");

        return group;
    }
}
