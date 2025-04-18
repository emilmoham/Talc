using Microsoft.EntityFrameworkCore;
using Talc.Models.Entities;

namespace Talc.Repositories;

public class UserRepository{
  private readonly ApplicationContext _context;
  
  public UserRepository(ApplicationContext context){
    _context = context;
  }

  public User AddUser(User userToAdd) {
    _context.Users.Add(userToAdd);
    return userToAdd;
  }

  public async Task<User?> GetUserByEmailAsync(string email) {
    string normalizedEmail = email.ToLower().Trim();
    return await _context.Users
      .SingleOrDefaultAsync(u => u.Email == normalizedEmail);
  }

  public async Task<User?> GetUserByIdAsync(Guid userId)
  {
    return await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
  }

  public User UpdateUser(User userToUpdate) 
  {
    _context.Users.Update(userToUpdate);
    return userToUpdate;
  }

  public async Task<bool> SaveChangesAsync() 
  {
    return await _context.SaveChangesAsync() > 0;
  }
}