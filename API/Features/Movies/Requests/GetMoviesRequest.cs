﻿using API.Features.Movies.Responses;
using ErrorOr;
using MediatR;
using Shared.Requests;
using Shared.Responses;

namespace API.Features.Movies.Requests;

public class GetMoviesRequest : QueryStringParameters, IRequest<ErrorOr<PagedResponse<GetMovieResponse>>>
{
    public string? Title { get; set; }
    public string? Director { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public int? MinDuration { get; set; }
    public int? MaxDuration { get; set; }
    public string? Genre { get; set; }
    public string Sort { get; set; } = MovieSortOptions.TitleAsc.ToString();
}