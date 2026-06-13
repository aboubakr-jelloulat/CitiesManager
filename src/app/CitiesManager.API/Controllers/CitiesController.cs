using CitiesManager.Domain.Entities;
using CitiesManager.Infrastructure.Context;
using CitiesManager.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CitiesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CitiesController> _logger;

    public CitiesController(ApplicationDbContext dbContext, ILogger<CitiesController> logger)
    {
        _db = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all cities
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities()
    {
        try
        {
            _logger.LogInformation("Getting all cities");
            
            var cities = await _db.Cities
                .AsNoTracking().Select(c => new CityDto
                {
                    Id = c.Id,
                    CityName = c.CityName,
                    Country = c.Country
                }).ToListAsync();

            return Ok(cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cities");
            return StatusCode(500, "An error occurred while retrieving cities");
        }
    }

    /// <summary>
    /// Get city by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CityDto>> GetCityById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid city ID");
        }

        try
        {
            _logger.LogInformation("Getting city with ID: {Id}", id);
            
            var city = await _db.Cities.AsNoTracking().Where(c => c.Id == id)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    CityName = c.CityName,
                    Country = c.Country
                }).FirstOrDefaultAsync();

            if (city is null)
            {
                _logger.LogWarning("City not found with ID: {Id}", id);
                return NotFound($"City with ID {id} was not found");
            }

            return Ok(city);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting city with ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the city");
        }
    }

    /// <summary>
    /// Create a new city
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CityDto>> AddCity([FromBody] CreateCityDto createCityDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(createCityDto.CityName))
        {
            return BadRequest("City name is required");
        }

        try
        {
            _logger.LogInformation("Creating new city: {CityName}", createCityDto.CityName);
            
            var city = new City
            {
                Id = Guid.NewGuid(),
                CityName = createCityDto.CityName.Trim(),
                Country = createCityDto.Country?.Trim()
            };

            await _db.Cities.AddAsync(city);
            await _db.SaveChangesAsync();

            var cityDto = new CityDto
            {
                Id = city.Id,
                CityName = city.CityName,
                Country = city.Country
            };

            return CreatedAtAction(nameof(GetCityById), new { id = city.Id }, cityDto);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating city");
            return StatusCode(500, "An error occurred while saving the city");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating city");
            return StatusCode(500, "An error occurred while creating the city");
        }
    }

    /// <summary>
    /// Update an existing city
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityDto updateCityDto)
    {
        if (id != updateCityDto.Id)
        {
            return BadRequest("ID in URL does not match ID in request body");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(updateCityDto.CityName))
        {
            return BadRequest("City name is required");
        }

        try
        {
            _logger.LogInformation("Updating city with ID: {Id}", id);
            
            var cityFromDb = await _db.Cities.FindAsync(id);
            
            if (cityFromDb is null)
            {
                _logger.LogWarning("City not found for update with ID: {Id}", id);
                return NotFound($"City with ID {id} was not found");
            }

            
            cityFromDb.CityName = updateCityDto.CityName.Trim();
            cityFromDb.Country = updateCityDto.Country?.Trim();

            _db.Cities.Update(cityFromDb);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error while updating city with ID: {Id}", id);
            return StatusCode(409, "The city was modified by another user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating city with ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the city");
        }
    }

    /// <summary>
    /// Delete a city
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid city ID");
        }

        try
        {
            _logger.LogInformation("Deleting city with ID: {Id}", id);
            
            var city = await _db.Cities.FindAsync(id);
            
            if (city is null)
            {
                _logger.LogWarning("City not found for deletion with ID: {Id}", id);
                return NotFound($"City with ID {id} was not found");
            }

            _db.Cities.Remove(city);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting city with ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the city");
        }
    }

}

