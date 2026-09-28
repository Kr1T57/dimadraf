using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            List<User> users = await _context.Users.ToListAsync();
            if(users.Count == 0)
            {
                return NotFound("Пользователей не найдено");
            }
            List<UserDTO> usersDTO = [];
            foreach(var user in users)
            {
                usersDTO.Add(user.ToDto());
            }
            return Ok(usersDTO);
        }

        [HttpGet("GetUser/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound("Пользователь не найден!");
            }
            return Ok(user.ToDto());
        }

        [HttpPost("Authorization")]
        public async Task<IActionResult> Authorization( [FromForm]string login,[FromForm] string password)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x=>x.Email == login);
            if(user == null)
            {
                return BadRequest("Неверный логин или пароль");
            }
            PasswordHasher<UserDTO> passwordHasher = new();
            if (passwordHasher.VerifyHashedPassword(user.ToDto(), user.Password, password) != PasswordVerificationResult.Success)
            {
                return BadRequest("Неверный логин или пароль");
            }
            return Ok(user.ToDto());
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] UserDTO userDTO)
        {
            if(await _context.Users.AnyAsync(x => x.Email == userDTO.Email))
            {
                return BadRequest("Пользователь с таким Email занят!");
            }
            if(await _context.Users.AnyAsync(x => x.Phone == userDTO.Phone))
            {
                return BadRequest("Пользователь с таким телефоном занят!");
            }
            int id = await _context.Users.AnyAsync()? await _context.Users.MaxAsync(x=>x.Id)+1 : 1;
            Role? role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "Клиент");
            if(role == null)
            {
                return BadRequest("Роль не найдена!");
            }
            PasswordHasher<UserDTO> passwordHasher = new();
            string password = passwordHasher.HashPassword(userDTO, userDTO.Password);
            User user = new()
            {
                Id = id,
                Name = userDTO.Name,
                Surname = userDTO.Surname,
                Patronymic = userDTO.Patronymic,
                Email = userDTO.Email,
                Password = password,
                Phone = userDTO.Phone,
                RoleId = role.Id,

            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user.ToDto());
        }

        [HttpPut("EditUser")]
        public async Task<IActionResult> EditUser([FromBody] UserDTO userDTO)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x=>x.Id == userDTO.Id);
            if(user == null)
            {
                return NotFound("Пользователь не найден!");
            }
            userDTO.Email = userDTO.Email.Trim();
            userDTO.Phone = userDTO.Phone.Trim();
            if (user.Email != userDTO.Email)
            {
                if (await _context.Users.AnyAsync(x => x.Email == userDTO.Email && x.Id != userDTO.Id))
                {
                    return BadRequest("Пользователь с таким Email занят!");
                }
            }
            if(user.Phone != userDTO.Phone)
            {
                if (await _context.Users.AnyAsync(x => x.Phone == userDTO.Phone && x.Id != userDTO.Id))
                {
                    return BadRequest("Пользователь с таким телефоном занят!");
                }
            }
            user.Name = userDTO.Name;
            user.Surname = userDTO.Surname;
            user.Patronymic = userDTO.Patronymic;
            user.Email = userDTO.Email;
            user.Phone = userDTO.Phone;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user.ToDto());
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user == null)
            {
                return NotFound("Пользователь не найден");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

