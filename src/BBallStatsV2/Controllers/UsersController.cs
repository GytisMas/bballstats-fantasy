using BBallStats.Data;
using BBallStats2.Auth.Model;
using BBallStatsV2.Data.Entities;
using BBallStatsV2.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BBallStatsV2.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ForumDbContext _context;
        private readonly UserManager<ForumRestUser> _userManager;

        public UsersController(ForumDbContext context, UserManager<ForumRestUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = ForumRoles.Admin)]
        public async Task<ActionResult<IEnumerable<UserWithoutRolesDto>>> GetUsers(CancellationToken cancellationToken)
        {
            return await _context.Users
                .Select(u => new UserWithoutRolesDto(u.Id, u.UserName, u.Email))
                .ToListAsync(cancellationToken);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserWithoutRolesDto>> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new UserDto(user.Id, user.UserName, user.Email, roles));
        }

        [HttpGet("{userId}/balance")]
        public async Task<ActionResult<int>> GetUserBalance(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var userBalance = await _context.Transactions
                .Where(t => t.SenderId == userId || t.RecipientId == userId)
                .SumAsync(t => t.RecipientId == userId ? t.Amount : -t.Amount);

            return Ok(userBalance);
        }

        [HttpPost]
        [Authorize(Roles = ForumRoles.Admin)]
        public async Task<ActionResult<UserWithoutRolesDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var user = await _userManager.FindByNameAsync(createUserDto.Username);
            if (user != null)
                return UnprocessableEntity("User name already taken");

            if (!HttpContext.User.IsInRole(ForumRoles.Admin))
            {
                return Forbid();
            }

            var newUser = new ForumRestUser()
            {
                UserName = createUserDto.Username,
                Email = createUserDto.Email
            };

            var createUserResult = await _userManager.CreateAsync(newUser, createUserDto.Password);
            if (!createUserResult.Succeeded)
            {
                var Descriptions = string.Join("\n", createUserResult.Errors.Select(e => e.Description));
                return UnprocessableEntity("User not created successfully - \n" + Descriptions);
            }

            List<string> roles = new List<string>();

            if ((createUserDto.Role & 1) != 0)
                roles.Add(ForumRoles.Admin);
            if ((createUserDto.Role & 2) != 0)
                roles.Add(ForumRoles.Moderator);
            if ((createUserDto.Role & 4) != 0)
                roles.Add(ForumRoles.Curator);
            if ((createUserDto.Role & 8) != 0)
                roles.Add(ForumRoles.Regular);
            await _userManager.AddToRolesAsync(newUser, roles);

            var registrationBonus = new Transaction()
            {
                TransactionType = TransactionType.Bonus,
                Amount = 100000,
                Recipient = newUser,
                Date = DateTime.UtcNow
            };
            _context.Transactions.Add(registrationBonus);
            await _context.SaveChangesAsync();

            return Created($"/api/Users/{newUser.Id}", new UserDto(newUser.Id, newUser.UserName, newUser.Email, roles));
        }

        [HttpPut("{userId}")]
        [Authorize]
        public async Task<ActionResult<UserWithoutRolesDto>> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Admin))
            {
                return Forbid();
            }

            if (updateUserDto.OldPassword != "" && updateUserDto.NewPassword != "")
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, updateUserDto.OldPassword, updateUserDto.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    var Descriptions = string.Join("\n", changePasswordResult.Errors.Select(e => e.Description));
                    return UnprocessableEntity("Password not changed successfully - \n" + Descriptions);
                }
            } else if (updateUserDto.OldPassword != "" || updateUserDto.NewPassword != "")
            {
                return UnprocessableEntity("Old or new password missing");
            }

            user.Email = updateUserDto.Email;
            await _userManager.UpdateAsync(user);
            List<string> roles = new List<string>();

            if ((updateUserDto.Role & 1) != 0)
                roles.Add(ForumRoles.Admin);
            if ((updateUserDto.Role & 2) != 0)
                roles.Add(ForumRoles.Moderator);
            if ((updateUserDto.Role & 4) != 0)
                roles.Add(ForumRoles.Curator);
            if ((updateUserDto.Role & 8) != 0)
                roles.Add(ForumRoles.Regular);

            await _userManager.RemoveFromRolesAsync(user, ForumRoles.All);
            await _userManager.AddToRolesAsync(user, roles);

            return Ok(new UserWithoutRolesDto(user.Id, user.UserName, user.Email));
        }


        [HttpDelete("{userId}")]
        [Authorize(Roles = ForumRoles.Admin)]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            if (!HttpContext.User.IsInRole(ForumRoles.Admin))
            {
                return Forbid();
            }

            var transactions = await _context.Transactions
                .Where(t => t.SenderId == userId || t.RecipientId == userId)
                .ToListAsync();
            foreach (var transaction in transactions)
            {

                if (transaction.SenderId == userId)
                {
                    transaction.SenderId = null;
                }
                if (transaction.RecipientId == userId)
                {
                    transaction.RecipientId = null;
                }

                if (transaction.SenderId == null && transaction.RecipientId == null)
                {
                    _context.Transactions.Remove(transaction);
                }
            }

            await _context.SaveChangesAsync();
            await _userManager.DeleteAsync(user);

            return NoContent();
        }
    }
}
