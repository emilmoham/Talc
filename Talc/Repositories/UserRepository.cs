using Microsoft.EntityFrameworkCore;
using Talc.Models.Entities;

namespace Talc.Repositories;

public class UserRepository{
  private readonly ApplicationContext _context;
  

  public UserRepository(ApplicationContext context){
    _context = context;
  }

  public async Task<User> AddUserAsync(User userToAdd) {
    await _context.Users.AddAsync(userToAdd);
    return userToAdd;
  }

  public async Task<User?> GetUserAsync(string email) {
    string normalizedEmail = email.ToLower().Trim();
    return await _context.Users
      .SingleOrDefaultAsync(u => u.Email == normalizedEmail);
  }

  public async Task<bool> SaveChangesAsync() 
  {
    return await _context.SaveChangesAsync() > 0;
  }
}