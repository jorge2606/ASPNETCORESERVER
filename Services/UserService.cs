using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;
using server.IServices;
using server.Helpers.WebApi.Helpers;

namespace server.Services
{
    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }



        public User Authenticate(string username, string password)
        {
            var user = _context.AllUsers.SingleOrDefault(x => x.Usuario == username);
            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public void Delete(Guid id)
        {
            var user = _context.AllUsers.Find(id);
            _context.AllUsers.Remove(user);
            _context.SaveChanges();
        }

        public void Update(User userParam)
        {
            var user = _context.AllUsers.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Usuario != user.Usuario)
            {
                // username has changed so check if the new username is already taken
                if (_context.AllUsers.Any(x => x.Usuario == userParam.Usuario))
                    throw new AppException("El usuario " + userParam.Usuario + " ya existe en la db.");
            }

            // update user properties
            user.Dni = userParam.Dni;
            user.Usuario = userParam.Usuario;
            user.Password = userParam.Password;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(userParam.Password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(userParam.Password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.AllUsers.Update(user);
            _context.SaveChanges();
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public User Create(User user, string password)
        {
            
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.AllUsers.Any(x => x.Usuario == user.Usuario))
                throw new AppException("Username \"" + user.Usuario + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.AllUsers.Add(user);
            _context.SaveChanges();

            return user;
        }

    }
}
