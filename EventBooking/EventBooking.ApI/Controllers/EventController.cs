using System.Security.Claims;
using EventBooking.ApI.DTOs;
using EventBooking.ApI.Helper;
using EventBooking.BLL.Repositories.Contract;
using EventBooking.DAL.Models;
using EventBooking.DAL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.ApI.Controllers
{
    [Authorize(Roles =SD.AdminRole)]
    public class EventController : BaseAPIControllercs
    {
        private readonly IGenericRepository<Event> eventRepository;
        private readonly IGenericRepository<Category> categoryRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public EventController(IGenericRepository<Event> eventRepository, IGenericRepository<Category> categoryRepository , IWebHostEnvironment webHostEnvironment)
        {
            this.eventRepository = eventRepository;
            this.categoryRepository = categoryRepository;
            this.webHostEnvironment = webHostEnvironment;
            HandlerPhotos.Initialize(webHostEnvironment);
        }

        [HttpGet("GetAllEvents")]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"User ID: {userId}");
            var events = await eventRepository.GetAllAsync(includeProperties: "Category,EventPhotos");
            var eventDtos = events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                Venue = e.Venue,
                Price = e.Price,
                //ImageUrl = e.ImageUrl,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name ,
                Photos = e.EventPhotos.Select(ep => new EventPhotoDto
                {
                    Id = ep.Id,
                    Url = $"{Request.Scheme}://{Request.Host}{ep.PhotoLink}"
                }).ToList()
            });
            return Ok(eventDtos);
        }
        [HttpGet("GetEventById/{id}")]
        public async Task<ActionResult<Event>> GetEventById(int id)
        {
            var eventItem = await eventRepository.GetByIdWithIncludeAsync(id,includeProperties: "Category,EventPhotos");
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
                //ImageUrl = eventItem.ImageUrl,
                CategoryId = eventItem.CategoryId,
                CategoryName = eventItem.Category?.Name,
                Photos = eventItem.EventPhotos.Select(ep => new EventPhotoDto
                {
                    Id = ep.Id,
                    Url = $"{Request.Scheme}://{Request.Host}{ep.PhotoLink}"
                }).ToList()
            };
            return Ok(eventDto);
        }
        [HttpPost("CreateEvent")]
        public async Task<ActionResult<Event>> CreateEvent([FromForm] EventCreateDto eventDto)
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
                CategoryId = eventDto.CategoryId,
            };
            await eventRepository.AddAsync(eventItem);
            if (eventDto.Photos != null && eventDto.Photos.Any())
            {
                var photoPaths = HandlerPhotos.UploadPhotos(eventDto.Photos, "events");
                foreach (var photoPath in photoPaths)
                {
                    eventItem.EventPhotos.Add(new EventPhoto
                    {
                        PhotoLink = photoPath,
                        EventId = eventItem.Id
                    });
                }
            }
            await eventRepository.SaveChangesAsync();
            var returnDto = new EventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                Date = eventItem.Date,
                Venue = eventItem.Venue,
                Price = eventItem.Price,
                CategoryId = eventItem.CategoryId,
                CategoryName = category.Name,
                Photos = eventItem.EventPhotos.Select(ep => new EventPhotoDto
                {
                    Id = ep.Id,
                    Url = $"{Request.Scheme}://{Request.Host}{ep.PhotoLink}"
                }).ToList()
            };
            return CreatedAtAction(nameof(GetEventById), new { id = eventItem.Id }, returnDto);
        }
        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventUpdateDto eventDto)
        {
            if (eventDto == null)
            {
                return BadRequest("Event is null");
            }
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingEvent = await eventRepository.GetByIdWithIncludeAsync(id: id, includeProperties: "Category,EventPhotos");
            if (existingEvent == null)
            {
                return NotFound("Event not found");
            }
            existingEvent.Title = eventDto.Title ?? existingEvent.Title;
            existingEvent.Description = eventDto.Description ?? existingEvent.Description;
            existingEvent.Date = eventDto.Date != default ? eventDto.Date : existingEvent.Date;
            existingEvent.Venue = eventDto.Venue ?? existingEvent.Venue;
            existingEvent.Price = eventDto.Price != default ? eventDto.Price : existingEvent.Price;
            existingEvent.CategoryId = eventDto.CategoryId != default ? eventDto.CategoryId : existingEvent.CategoryId;
            existingEvent.ModifiedDate = DateTime.UtcNow;
            #region Handle Response
            //await eventRepository.UpdateAsync(existingEvent);

            //await eventRepository.SaveChangesAsync();
            //var returnDto = new EventDto
            //{
            //    Id = existingEvent.Id,
            //    Title = existingEvent.Title,
            //    Description = existingEvent.Description,
            //    Date = existingEvent.Date,
            //    Venue = existingEvent.Venue,
            //    Price = existingEvent.Price,
            //    //ImageUrl = existingEvent.ImageUrl,
            //    CategoryId = existingEvent.CategoryId,
            //    CategoryName = (await categoryRepository.GetByIdAsync(existingEvent.CategoryId))?.Name,
            //    Photos = existingEvent.EventPhotos.Select(ep => new EventPhotoDto
            //    {
            //        Id = ep.Id,
            //        Url = $"{Request.Scheme}://{Request.Host}{ep.PhotoLink}"
            //    }).ToList()
            //};
            ////return NoContent();
            //return Ok(new { Message = "Event Updated Successfully", Event = returnDto });
            #endregion

            if (eventDto.PhotosToAdd != null && eventDto.PhotosToAdd.Any())
            {
                var photoNames = HandlerPhotos.UploadPhotos(eventDto.PhotosToAdd, "events");
                foreach (var photoName in photoNames)
                {
                    existingEvent.EventPhotos.Add(new EventPhoto
                    {
                        PhotoLink = $"/images/events/{photoName}",
                        EventId = existingEvent.Id
                    });
                }
            }
            var photoIdsToDelete = eventDto.PhotoIdsToDelete;
            if (photoIdsToDelete != null && photoIdsToDelete.Any())
            {
                var photosToDelete = existingEvent.EventPhotos.Where(ep => photoIdsToDelete.Contains(ep.Id)).ToList();
                foreach (var photo in photosToDelete)
                {

                    var oldFileName = Path.GetFileName(photo.PhotoLink);
                    HandlerPhotos.DeletePhoto("events", oldFileName);
                    existingEvent.EventPhotos.Remove(photo);

                    #region Using Function DeletePhoto0
                    //string fileName = photo.PhotoLink.Split('/').Last(); 
                    //bool isDeleted = HandlerPhotos.DeletePhoto0("events", fileName);
                    //if (isDeleted)
                    //{
                    //    existingEvent.EventPhotos.Remove(photo);
                    //}
                    //else
                    //{
                    //    Console.WriteLine($"Failed to delete file: {photo.PhotoLink}");
                    //} 
                    #endregion
                }
            }
            await eventRepository.UpdateAsync(existingEvent);
            await eventRepository.SaveChangesAsync();

            var category = await categoryRepository.GetByIdAsync(existingEvent.CategoryId);
            var returnDto = new EventDto
            {
                Id = existingEvent.Id,
                Title = existingEvent.Title,
                Description = existingEvent.Description,
                Date = existingEvent.Date,
                Venue = existingEvent.Venue,
                Price = existingEvent.Price,
                CategoryId = existingEvent.CategoryId,
                CategoryName = category?.Name,
                Photos = existingEvent.EventPhotos.Select(ep => new EventPhotoDto
                {
                    Id = ep.Id,
                    Url = $"{Request.Scheme}://{Request.Host}{ep.PhotoLink}"
                }).ToList()
            };

            return Ok(new { Message = "Event Updated Successfully", Event = returnDto });
        }
        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await eventRepository.GetByIdWithIncludeAsync(id:id,includeProperties: "EventPhotos");
            if (eventItem == null)
            {
                return NotFound("Event not found");
            }



            #region Using Function DeletePhoto0
            if (eventItem.EventPhotos != null && eventItem.EventPhotos.Any())
            {
                var photosToDelete = eventItem.EventPhotos.ToList();
                foreach (var photo in photosToDelete)
                {
                    //string fileName = photo.PhotoLink.Split('/').Last();
                    var oldFileName = Path.GetFileName(photo.PhotoLink);
                    bool isDeleted = HandlerPhotos.DeletePhoto0("events", oldFileName);
                    if (isDeleted)
                    {
                        
                        eventItem.EventPhotos.Remove(photo);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to delete file: {photo.PhotoLink}");
                    }
                }
            }
            #endregion

            await eventRepository.DeleteAsync(eventItem);
            await eventRepository.SaveChangesAsync();
            return NoContent();
        }

    }
}
