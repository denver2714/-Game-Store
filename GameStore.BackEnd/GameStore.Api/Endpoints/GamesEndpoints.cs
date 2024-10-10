using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games").WithParameterValidation(); // All of my routes for this specifc endpoints start with 'games' // WithPV is to recognize the data annotations


        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            await Task.Delay(3000);
            return await dbContext.Games
                     .Include(game => game.Genre) //if no games then it's null
                     .Select(game => game.ToGameSummaryDto())
                     .AsNoTracking()
                     .ToListAsync();
        });

        //Get /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);  // check to see if id is valid or not

            return game is null ?
                Results.NotFound() : Results.Ok(game.ToGameDetailsDto());

        })
        .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = game.Id },
                game.ToGameDetailsDto());

        });

        // PUT /games
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id); //if it didnt find anything then it would give a null
            if (existingGame is null)
            { // It's your choice as a programmer whether to give a not found error or just make a new endpoint for it
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                     .CurrentValues
                     .SetValues(updatedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });


        // DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                     .Where(game => game.Id == id)
                     .ExecuteDeleteAsync();

            return Results.NoContent();
        });
        return group;
    }
}
