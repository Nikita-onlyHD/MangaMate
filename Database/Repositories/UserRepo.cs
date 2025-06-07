using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using MangaMate.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MangaMate.Database.Repositories
{
    internal static class UserRepo
    {
        public static async Task<bool> AddUserAsync(User user, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            try
            {
                context.Users.Add(user);
                await context.SaveChangesAsync(token);
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public static async Task<User?> GetUser(string login, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            try
            {
                return await context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Login == login
                        , token);
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }

        public static async Task<bool> UpdateUserAsync(
            string login,
            CancellationToken token,
            string? newLogin = null,
            string? newPassword = null,
            byte[]? newAvatar = null)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            try
            {
                var user = await context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Login == login
                        , token);

                if (user == null)
                    return false;

                if (newLogin != null)
                    user.Login = newLogin;

                if (newPassword != null)
                    user.Password = newPassword;

                if (newAvatar != null)
                    user.Avatar = newAvatar;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}
