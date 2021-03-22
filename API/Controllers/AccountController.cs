using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using DAL;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        // private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._mapper = mapper;
            this._tokenService = tokenService;
            // this._context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);

            // using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            // user.PasswordSalt = hmac.Key;
            // _context.Users.Add(user);
            // await _context.SaveChangesAsync();

            return new UserDto()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username)) return BadRequest("Please enter the valid username");

            var user = await _userManager.Users.Include(x => x.Photos)
                                               .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            // using var hmac = new HMACSHA512(user.PasswordSalt);

            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // for (int i = 0; i < computedHash.Length; i++)
            // {
            //     if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            // }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized("Username or password is wrong");

            return new UserDto()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.SingleOrDefaultAsync(user => user.UserName == username) != null;
        }
    }
}