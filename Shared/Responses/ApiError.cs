﻿namespace Shared.Responses;

public class ApiError
{
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
}