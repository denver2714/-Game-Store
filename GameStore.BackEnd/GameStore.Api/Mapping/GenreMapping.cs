using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GenreMapping
{
    public static GenreDto toDto(this Genre genre)
    {
        return new GenreDto(genre.Id, genre.Name);
    }
}
