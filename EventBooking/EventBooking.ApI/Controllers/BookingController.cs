using EventBooking.ApI.DTOs;
using EventBooking.BLL.Repositories.Contract;
using EventBooking.DAL.Models;
using EventBooking.DAL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.ApI.Controllers
{
    [Authorize(Roles = SD.CustomerRole)]
    public class BookingController : BaseAPIControllercs
    {
        private readonly IGenericRepository<Booking> bookingRepository;
        private readonly IGenericRepository<Event> eventRepository;

        public BookingController(IGenericRepository<Booking> bookingRepository,IGenericRepository<Event> eventRepository)
        {
            this.bookingRepository = bookingRepository;
            this.eventRepository = eventRepository;
        }

        [HttpGet("GetAllBookings")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
        {
            var bookings = await bookingRepository.GetAllAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound("No bookings found");
            }
            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                EventId = b.EventId,
                UserId = b.UserId,
                BookingDate = b.BookingDate,
                NumberOfTickets = b.NumberOfTickets,
                TotalPrice = b.TotalPrice
            }).ToList();
            return Ok(bookingDtos);
        }
        [HttpGet("GetBookingById/{id}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null )
            {
                return NotFound("Booking not found");
            }
            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                EventId = booking.EventId,
                UserId = booking.UserId,
                BookingDate = booking.BookingDate,
                NumberOfTickets = booking.NumberOfTickets,
                TotalPrice = booking.TotalPrice
            };
            return Ok(bookingDto);
        }
        [HttpPost("CreateBooking")]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingCreateDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var eventToBook = await eventRepository.GetByIdAsync(bookingDto.EventId);
            if (eventToBook == null || eventToBook.IsDeleted)
            {
                return NotFound("Event not found");
            }
            if (eventToBook.AvailableTickets < bookingDto.NumberOfTickets)
            {
                return BadRequest("Not enough tickets available");
            }
            var booking = new Booking
            {
                EventId = bookingDto.EventId,
                UserId = bookingDto.UserId,
                BookingDate = DateTime.UtcNow,
                NumberOfTickets = bookingDto.NumberOfTickets,
                TotalPrice = eventToBook.Price * bookingDto.NumberOfTickets,
            };
            eventToBook.AvailableTickets -= bookingDto.NumberOfTickets;
            await eventRepository.UpdateAsync(eventToBook);
            await bookingRepository.AddAsync(booking);
            await bookingRepository.SaveChangesAsync();
            var bookingResponse = new BookingDto
            {
                Id = booking.Id,
                EventId = booking.EventId,
                UserId = booking.UserId,
                BookingDate = booking.BookingDate,
                NumberOfTickets = booking.NumberOfTickets,
                TotalPrice = booking.TotalPrice
            };

            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, bookingResponse);

        }

        [HttpPut("UpdateBooking/{id}")]
        public async Task<IActionResult> UpdateBooking(int id,[FromBody] BookingCreateDto bookingDto)
        {
            if (bookingDto == null)
            {
                return BadRequest("Booking data is null");
            }
            if (id != bookingDto.Id)
            {
                return BadRequest("Booking ID mismatch");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound("Booking not found");
            }
            var eventToBook = await eventRepository.GetByIdAsync(booking.EventId);
            if (eventToBook == null || eventToBook.IsDeleted)
            {
                return NotFound("Event not found");
            }
            if (eventToBook.AvailableTickets + booking.NumberOfTickets < bookingDto.NumberOfTickets)
            {
                return BadRequest("Not enough tickets available");
            }
            eventToBook.AvailableTickets += booking.NumberOfTickets - bookingDto.NumberOfTickets;
            await eventRepository.UpdateAsync(eventToBook);
            booking.UserId = bookingDto.UserId;
            booking.NumberOfTickets = bookingDto.NumberOfTickets;
            booking.TotalPrice = eventToBook.Price * bookingDto.NumberOfTickets;
            await bookingRepository.UpdateAsync(booking);
            await bookingRepository.SaveChangesAsync();
            var bookingResponse = new BookingDto
            {
                Id = booking.Id,
                EventId = booking.EventId,
                UserId = booking.UserId,
                BookingDate = booking.BookingDate,
                NumberOfTickets = booking.NumberOfTickets,
                TotalPrice = booking.TotalPrice
            };
            return Ok();
        }
        [HttpDelete("DeleteBooking/{id}")]
        public async Task<IActionResult>  DeleteBooking(int id)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound("Booking not found");
            }
            var eventToBook = await eventRepository.GetByIdAsync(booking.EventId);
            if (eventToBook == null || eventToBook.IsDeleted)
            {
                return NotFound("Event not found");
            }
            eventToBook.AvailableTickets += booking.NumberOfTickets;
            await eventRepository.UpdateAsync(eventToBook);
            await bookingRepository.DeleteAsync(booking);
            await bookingRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
