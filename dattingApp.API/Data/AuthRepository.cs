using System;
using System.Linq;
using System.Threading.Tasks;
using dattingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace dattingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string passowrd)
        {
            var user=await _context.Users.FirstOrDefaultAsync(x=>x.Username==username);
            if(user==null)
                return null;
            if(!VerifyPasswordHash(passowrd,user.PasswordHash,user.PasswordSalt))
                return null;
            return user;
        }

        private bool VerifyPasswordHash(string passowrd, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passowrd));
                for(int i=0;i<computedHash.Length;i++)
                {
                    if(computedHash[i]!=passwordHash[i])
                    return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passowrdHash,passowrdSalt;
            CreatePasswordHash(password,out passowrdHash,out passowrdSalt);
            user.PasswordHash=passowrdHash;
            user.PasswordSalt=passowrdSalt;
             await _context.Users.AddAsync(user);
             await _context.SaveChangesAsync();
             return user;
        }

        private void CreatePasswordHash(string password, out byte[] passowrdHash, out byte[] passowrdSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512())
            {
                passowrdSalt=hmac.Key;
                passowrdHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x=>x.Username==username))
                return true;
            return false;
        }
    }
}