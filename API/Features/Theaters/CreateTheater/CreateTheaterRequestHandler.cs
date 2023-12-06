using API.Data;
using API.Extensions;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.ServiceErrors;

namespace API.Features.Theaters.CreateTheater;

public class CreateTheaterRequestHandler(
    CinemaDbContext context,
    IValidator<CreateTheaterRequest> validator,
    ILogger<CreateTheaterRequestHandler> logger) : IRequestHandler<CreateTheaterRequest, ErrorOr<CreateTheaterResponse>>
{
    public async Task<ErrorOr<CreateTheaterResponse>> Handle(CreateTheaterRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Trying to create a new theater.\nRequest: {0}", request);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToErrorList();
            logger.LogInformation("Validation failed for {0}. Errors: {1}", nameof(CreateTheaterRequest),
                errors);
            return errors;
        }

        var newTheaterId = await GenerateTheaterId(request);
        logger.LogInformation("Generated new TheaterId: {0}", newTheaterId);

        var theater = TheaterMapper.ToTheaterObject(request, newTheaterId);

        var duplicateTheater = await context.Theaters.FindAsync(newTheaterId);
        if (duplicateTheater is not null)
        {
            logger.LogCritical("A theater with the same ID already exists. ID: {0}", newTheaterId);
            return Errors.Theater.TheaterAlreadyExists;
        }


        await context.Theaters.AddAsync(theater, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Theater added to database. ID: {0}", theater.TheaterId);
        return new CreateTheaterResponse(theater.TheaterId);
    }

    private async Task<string> GenerateTheaterId(CreateTheaterRequest request)
    {
        var lastTheaterOfSameType = await context.Theaters
            .LastOrDefaultAsync(x => x.ScreenType == request.ScreenType);

        // if no theater of the same type is found, then this should be the first.
        if (lastTheaterOfSameType is null)
        {
            // enum type has been validated already in CreateTheaterRequestValidator.cs.
            // we can safely use the ScreenType here.
            return $"TH-{request.ScreenType}-1";
        }

        var lastNumber = GetTheaterNumberFromId(lastTheaterOfSameType.TheaterId);
        return $"TH-{request.ScreenType}-{lastNumber + 1}";
    }

    private static int GetTheaterNumberFromId(string theaterId)
    {
        var idComponents = theaterId.Split('-');
        return int.Parse(idComponents[2]);
    }
}