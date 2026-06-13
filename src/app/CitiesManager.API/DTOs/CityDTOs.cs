namespace CitiesManager.API.DTOs;

public class CityDto
{
    public Guid Id { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string? Country { get; set; }
}

 public class CreateCityDto
{
    public string CityName { get; set; } = string.Empty;
    public string? Country { get; set; }
}

public class UpdateCityDto
{
    public Guid Id { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string? Country { get; set; }
}
