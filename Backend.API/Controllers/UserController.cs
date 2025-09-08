using Backend.Service.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Backend.Service.Contracts;
using Backend.Service.Extensions;
using Backend.Service.Models;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IRepositoryWrapper repository) : ControllerBase
    {
        // GET: api/User/search?query=somevalue
        [HttpGet("search")]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> SearchUser([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new ErrorDTO { Message = "Username is required" });

            var user = await repository.UserRepository.FindByUsername(username);
            if (user == null)
                return NotFound(new ErrorDTO { Message = "User not found" });

            return Ok(user.ToUserDto());
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserEntity user)
        {
            if (user == null || id != user.Id)
                return BadRequest(new ErrorDTO { Message = "Invalid user data" });

            var existingUser = await repository.UserRepository.GetUserById(id);
            if (existingUser == null)
                return NotFound(new ErrorDTO { Message = "User not found" });

            existingUser.Username = user.Username;
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;

            repository.UserRepository.UpdateUser(existingUser);
            await repository.Save();

            return Ok(existingUser.ToUserDto());
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await repository.UserRepository.GetUserById(id);
            if (user == null)
                return NotFound(new ErrorDTO { Message = "User not found" });

            repository.UserRepository.DeleteUser(user);
            await repository.Save();

            return NoContent();
        }
    }
}