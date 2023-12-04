namespace API.Models.Enums;

public enum GenresEnum
{
    Action,
    Adventure,
    Animation,
    Comedy,
    Crime,
    Documentary,
    Drama,
    Family,
    Fantasy,
    History,
    Horror,
    Music,
    Mystery,
    Romance,
    ScienceFiction,
    Thriller,
    TvMovie,
    War,
    Western
}

public static class Genres
{
    public const string Action = nameof(Action);
    public const string Adventure = nameof(Adventure);
    public const string Animation = nameof(Animation);
    public const string Comedy = nameof(Comedy);
    public const string Crime = nameof(Crime);
    public const string Documentary = nameof(Documentary);
    public const string Drama = nameof(Drama);
    public const string Family = nameof(Family);
    public const string Fantasy = nameof(Fantasy);
    public const string History = nameof(History);
    public const string Horror = nameof(Horror);
    public const string Music = nameof(Music);
    public const string Mystery = nameof(Mystery);
    public const string Romance = nameof(Romance);
    public const string ScienceFiction = nameof(ScienceFiction);
    public const string Thriller = nameof(Thriller);
    public const string TvMovie = nameof(TvMovie);
    public const string War = nameof(War);
    public const string Western = nameof(Western);

    public static readonly string[] AllGenres = Enum.GetNames(typeof(GenresEnum));
}