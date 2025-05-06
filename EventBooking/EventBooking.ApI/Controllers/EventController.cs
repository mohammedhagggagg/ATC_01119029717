using EventBooking.ApI.DTOs;
using EventBooking.BLL.Repositories.Contract;
using EventBooking.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.ApI.Controllers
{
   
    public class EventController : BaseAPIControllercs
    {
        private readonly IGenericRepository<Event> eventRepository;
        private readonly IGenericRepository<Category> categoryRepository;

        public EventController(IGenericRepository<Event> eventRepository, IGenericRepository<Category> categoryRepository)
        {
            this.eventRepository = eventRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet("GetAllEvents")]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var events = await eventRepository.GetAllAsync(includeProperties: "Category");
            var eventDtos = events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                Venue = e.Venue,
                Price = e.Price,
                ImageUrl = e.ImageUrl,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name 
            });
            return Ok(eventDtos);
        }
        [HttpGet("GetEventById/{id}")]
        public async Task<ActionResult<Event>> GetEventById(int id)
        {
            var eventItem = await eventRepository.GetByIdWithIncludeAsync(id,includeProperties: "Category");
            if (eventItem == null)
            {
                return BadRequest(BadRequest("Event not found"));
            }
            var eventDto = new EventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                Date = eventItem.Date,
                Venue = eventItem.Venue,
                Price = eventItem.Price,
                ImageUrl = eventItem.ImageUrl,
                CategoryId = eventItem.CategoryId,
                CategoryName = eventItem.Category?.Name
            };
            return Ok(eventDto);
        }
        [HttpPost("CreateEvent")]
        public async Task<ActionResult<Event>> CreateEvent(EventDto eventDto)
            {
            if (eventDto == null)
            {
                return BadRequest("Event is null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await categoryRepository.GetByIdAsync(eventDto.CategoryId);
            if (category == null)
            {
                return BadRequest("Invalid CategoryId");
            }
            var eventItem = new Event
            {
                Title = eventDto.Title,
                Description = eventDto.Description,
                Date = eventDto.Date,
                Venue = eventDto.Venue,
                Price = eventDto.Price,
                ImageUrl = eventDto.ImageUrl,
                CategoryId = eventDto.CategoryId,
            };

            await eventRepository.AddAsync(eventItem);
            await eventRepository.SaveChangesAsync();
            var returnDto = new EventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                Date = eventItem.Date,
                Venue = eventItem.Venue,
                Price = eventItem.Price,
                ImageUrl = eventItem.ImageUrl,
                CategoryId = eventItem.CategoryId,
                CategoryName = category.Name
            };
            return CreatedAtAction(nameof(GetEventById), new { id = eventItem.Id }, returnDto);
        }
        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventDto eventDto)
        {
            if (eventDto == null)
            {
                return BadRequest("Event is null");
            }
            if (id != eventDto.Id)
            {
                return BadRequest("Event ID mismatch");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingEvent = await eventRepository.GetByIdAsync(id);
            if (existingEvent == null)
            {
                return NotFound("Event not found");
            }
            existingEvent.Title = eventDto.Title;
            existingEvent.Description = eventDto.Description;
            existingEvent.Date = eventDto.Date;
            existingEvent.Venue = eventDto.Venue;
            existingEvent.Price = eventDto.Price;
            existingEvent.ImageUrl = eventDto.ImageUrl;
            existingEvent.CategoryId = eventDto.CategoryId;
            existingEvent.ModifiedDate = DateTime.UtcNow;
            await eventRepository.UpdateAsync(existingEvent);
            await eventRepository.SaveChangesAsync();
            var returnDto = new EventDto
            {
                Id = existingEvent.Id,
                Title = existingEvent.Title,
                Description = existingEvent.Description,
                Date = existingEvent.Date,
                Venue = existingEvent.Venue,
                Price = existingEvent.Price,
                ImageUrl = existingEvent.ImageUrl,
                CategoryId = existingEvent.CategoryId,
                CategoryName = (await categoryRepository.GetByIdAsync(existingEvent.CategoryId))?.Name
            };
            //return NoContent();
            return Ok(new {Message="Event Updated Successfully",returnDto});
        }
        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await eventRepository.GetByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound("Event not found");
            }
            await eventRepository.DeleteAsync(eventItem);
            await eventRepository.SaveChangesAsync();
            return NoContent();
        }

    }
}
