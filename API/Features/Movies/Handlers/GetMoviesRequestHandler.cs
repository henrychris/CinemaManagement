using API.Data;
using API.Features.Movies.Requests;
using API.Features.Movies.Responses;
using API.Models.Domain;
using MediatR;
using ErrorOr;
using Shared.Responses;

namespace API.Features.Movies.Handlers;

public class GetMoviesRequestHandler(CinemaDbContext context, ILogger<GetMoviesRequestHandler> logger)
    : IRequestHandler<GetMoviesRequest, ErrorOr<PagedResponse<GetMovieResponse>>>
{
    public async Task<ErrorOr<PagedResponse<GetMovieResponse>>> Handle(GetMoviesRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching movies... \nRequest: {0}", request);
        Enum.TryParse<MovieSortOptions>(request.Sort, out var sortOption);

        var query = context.Movies.AsQueryable();

        query = ApplyFilters(query, request);
        query = SortQuery(query, sortOption);

        var pagedResults = query.Select(x => MovieMapper.ToGetMovieResponse(x));
        var response =
            await new PagedResponse<GetMovieResponse>().ToPagedList(pagedResults, request.PageNumber, request.PageSize);

        logger.LogInformation("Fetched Movies successfully. Returned {totalCount} movies.", response.TotalCount);
        return response;
    }

    private IQueryable<Movie> SortQuery(IQueryable<Movie> query, MovieSortOptions sortOption)
    {
        logger.LogInformation("Sorting Movies by: {sortOption}", sortOption);
        query = sortOption switch
        {
            MovieSortOptions.TitleAsc => query.OrderBy(x => x.Title),
            MovieSortOptions.TitleDesc => query.OrderByDescending(x => x.Title),
            MovieSortOptions.RatingAsc => query.OrderBy(x => x.Rating),
            MovieSortOptions.RatingDesc => query.OrderByDescending(x => x.Rating),
            MovieSortOptions.ReleaseDateAsc => query.OrderBy(x => x.ReleaseDate),
            MovieSortOptions.ReleaseDateDesc => query.OrderByDescending(x => x.ReleaseDate),
            MovieSortOptions.DurationAsc => query.OrderBy(x => x.DurationInMinutes),
            MovieSortOptions.DurationDesc => query.OrderByDescending(x => x.DurationInMinutes),

            _ => query.OrderBy(x => x.Title)
        };

        return query;
    }

    private static IQueryable<Movie> ApplyFilters(IQueryable<Movie> query, GetMoviesRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            query = query.Where(x => x.Title.Contains(request.Title));
        }

        if (!string.IsNullOrWhiteSpace(request.Director))
        {
            query = query.Where(x => x.Title.Contains(request.Director));
        }

        if (request.MinRating is not null)
        {
            query = query.Where(x => x.Rating >= request.MinRating);
        }

        if (request.MaxRating is not null)
        {
            query = query.Where(x => x.Rating <= request.MinRating);
        }

        if (request.MinDuration is not null)
        {
            query = query.Where(x => x.Rating >= request.MinDuration);
        }

        if (request.MaxDuration is not null)
        {
            query = query.Where(x => x.Rating >= request.MaxDuration);
        }

        if (!string.IsNullOrWhiteSpace(request.Genre))
        {
            // return only the movies whose genres contain the requested genre
            query = query.Where(x => x.Genres.Contains(request.Genre));
        }

        return query;
    }
}