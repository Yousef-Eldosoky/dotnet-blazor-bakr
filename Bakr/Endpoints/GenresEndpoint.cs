using Bakr.Data;
using Bakr.Shared.Dtos;
using Bakr.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Endpoints;

public static class GenresEndpoint
{
    const string getGenreEndpointName = "GetGenre";

    public static RouteGroupBuilder MapGenreEndpoint(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/genres").WithParameterValidation().RequireAuthorization();

        group.MapGet("/", async (BakrDbContext dbContext) =>
        {
            return Results.Ok(await dbContext.Genres.AsNoTracking().ToListAsync());
        });


        group.MapGet("/{id}", async (int id, BakrDbContext dbContext) =>
        {
            Genre? genre = await dbContext.Genres.FindAsync(id);
            if (genre is null) return Results.NotFound();
            return Results.Ok(genre);
        }).WithName(getGenreEndpointName);

        group.MapPost("/", async (CreateGenreDto newGenre, BakrDbContext dbContext) =>
        {
            Genre genre = new()
            {
                Name = newGenre.Name,
            };
            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();
            return Results.CreatedAtRoute(getGenreEndpointName, new { id = genre.Id }, genre);
        }).RequireAuthorization("AdminPolicy"); ;


        group.MapPut("/{id}", async (int id, CreateGenreDto newGenre, BakrDbContext dbContext) =>
        {
            Genre? genre = await dbContext.Genres.FindAsync(id);
            if (genre is null) return Results.NotFound();
            genre.Name = newGenre.Name;
            dbContext.Genres.Entry(genre).CurrentValues.SetValues(genre);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization("AdminPolicy"); ;


        group.MapDelete("/{id}", async (int id, BakrDbContext dbContext) =>
        {
            await dbContext.Genres.Where(genre => genre.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        }).RequireAuthorization("AdminPolicy");

        return group;
    }
}
