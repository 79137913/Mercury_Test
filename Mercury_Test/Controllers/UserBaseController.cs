using EntityFrameworkPaginateCore;
using Mercury_Test.Data;
using Mercury_Test.DBContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Mercury_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBaseController : ControllerBase
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserBaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<IdentityResult>> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    LastName = model.LastName,
                    Birthday = model.Birthday,
                    Gender = model.Gender
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (db.Users.Count() == 1) //First Time Role config
                    {
                        if (await _roleManager.RoleExistsAsync("Admin") == false)
                        {
                            IdentityRole adminRole = new IdentityRole("Admin");
                            IdentityRole userRole = new IdentityRole("User");
                            await _roleManager.CreateAsync(adminRole);
                            await _roleManager.CreateAsync(userRole);
                        }

                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    
                }
                return result;
            } else
            {
                return BadRequest(model);
            }
        }
        [HttpPost("Login")]
        public async Task<ActionResult<SignInResult>> Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                return result;
            }
            else
            {
                return BadRequest(model);
            }
        }

        [HttpPost("Logout")]
        public async Task<ActionResult<bool>> Logout()
        {
            if (ModelState.IsValid)
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Object>>> UserList(int pageNumber = 1, int pageSize = 10, string emailfilter = "", string namefilter = "", string lastnamefilter = "", DateTime? bornbefore = null, DateTime? bornafter = null, int? gender = null)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if(role == "Admin" || role == "User")
            {
                var filters = new Filters<ApplicationUser>();
                filters.Add(!string.IsNullOrEmpty(emailfilter), x => x.Email.Contains(emailfilter));
                filters.Add(!string.IsNullOrEmpty(namefilter), x => x.Name.Contains(namefilter));
                filters.Add(!string.IsNullOrEmpty(lastnamefilter), x => x.LastName.Contains(lastnamefilter));
                filters.Add(bornafter.HasValue, x => x.Birthday.HasValue && x.Birthday > bornafter.Value);
                filters.Add(bornbefore.HasValue, x => x.Birthday.HasValue && x.Birthday < bornbefore.Value);
                filters.Add(gender.HasValue, x=> x.Gender.HasValue && x.Gender == gender.Value);

                //could add sorts but not required on Test Task
                var sorts = new Sorts<ApplicationUser>();
                //sorts.Add(sortBy == "gender", x => x.Gender);
                Page<ApplicationUser> paginatedResult = db.Users.Paginate(pageNumber,
                                  pageSize, sorts, filters);

                List<Object> users = new List<Object>();
                foreach (var user in paginatedResult.Results)
                {
                    users.Add(new
                    {
                        user.Id,
                        user.Name,
                        user.LastName,
                        user.Email,
                        user.Birthday,
                        user.Gender
                    });
                }
                return users;
            } else
            {
                return Unauthorized();
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteUser(string id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin")
            {
                var user = await db.Users.FindAsync(id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return true;
                } else
                {
                    string error = "User not found";
                    return BadRequest(new { error });
                }

            }
            else
            {
                return Unauthorized();
            }

        }


        [HttpPut("ModifyPassword")]
        public async Task<ActionResult<IdentityResult>> ModifyPassword(ModifyPassModel model)
        {
            if (User.FindFirstValue(ClaimTypes.Role)==null) { return Unauthorized(); }
            if (ModelState.IsValid)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                ApplicationUser user = await _userManager.FindByEmailAsync(email);
                IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
                return result;
            }
            else
            {
                return BadRequest(model);
            }
        }

        [HttpPut("ModifyUser")]
        public async Task<ActionResult<Boolean>> ModifyUser(ModifyUserModel model)
        {
            if (User.FindFirstValue(ClaimTypes.Role) == null) { return Unauthorized(); }
            if (ModelState.IsValid)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                ApplicationUser user = await _userManager.FindByEmailAsync(email);
                user.Birthday = model.Birthday;
                user.Gender = model.Gender;
                user.Name = model.Name;
                user.LastName = model.LastName;

                db.Entry(user).State = EntityState.Modified;

                await db.SaveChangesAsync();

                return true;
            }
            else
            {
                return BadRequest(model);
            }
        }


    }
}
