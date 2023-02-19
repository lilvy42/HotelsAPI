public class HotelsAPI : IAPI
{
    public void Register(WebApplication app)
    {
        app.MapGet("/hotels", GetAll)
            .Produces<List<Hotel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("GetAllHotels")
            .WithTags("Getters");

    app.MapGet("/hotels/search/name/{query}", SearchByName)
        .Produces<List<Hotel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithName("SearchHotels")
        .WithTags("Getters");

    app.MapGet("/hotels/{id}", GetById)
        .Produces<Hotel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithName("GetOneHotel")
            .WithTags("Getters");

    app.MapPost("/hotels", Create)
    .Accepts<Hotel>("application/json")
    .Produces<Hotel>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status401Unauthorized)
    .WithName("CreateHotel")
    .WithTags("Creators");

    app.MapPut("/hotels", Update)
    .Accepts<Hotel>("application/json")
    .Produces(StatusCodes.Status401Unauthorized)
    .WithName("UpdateHotel")
    .WithTags("Updaters");

    app.MapDelete("/hotels/{id}", Delete)
    .Produces(StatusCodes.Status401Unauthorized)
    .WithName("DeleteHotel")
    .WithTags("Deteters");

    }

    [Authorize] 
    private async Task<IResult> GetAll (IHotelRepository repository) => 
            Results.Extensions.Xml(await repository.GetHotelsAsync());

    [Authorize]
    private async Task<IResult> SearchByName (string query, IHotelRepository repository) =>
        await repository.GetHotelsAsync(query) is IEnumerable<Hotel> hotels
        ? Results.Ok(hotels)
        : Results.NotFound(Array.Empty<Hotel>);

    [Authorize] 
    private async Task<IResult> GetById (int id, IHotelRepository repository) =>
            await repository.GetHotelAsync(id) is Hotel hotel
            ? Results.Ok(hotel)
            : Results.NotFound();
    
    [Authorize] 
    private async Task<IResult> Create ([FromBody] Hotel hotel, IHotelRepository repository)
    {
        await repository.InsertHotelAsync(hotel);
        await repository.SaveAsync();
        return Results.Created($"/hotels/{hotel.Id}", hotel);
    }

    [Authorize]
    private async Task<IResult> Update ([FromBody] Hotel hotel, IHotelRepository repository)
    {
        await repository.UpdateHotelAsync(hotel);
        await repository.SaveAsync();
        return Results.NoContent();
    }

    [Authorize] 
    private async Task<IResult> Delete (int id, IHotelRepository repository)
    {
        await repository.DeleteHotelAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    }
}