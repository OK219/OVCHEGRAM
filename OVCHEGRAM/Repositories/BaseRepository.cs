using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OVCHEGRAM.Models;
using OVCHEGRAM.DBModels;

namespace OVCHEGRAM.Repositories;

public abstract class BaseRepository<T>(OvchegramDbContext dbContext) : IRepository<T> where T : class
{
    public void Add(T entry, bool shouldSave = true)
    {
        dbContext.Set<T>().Add(entry);
        if (shouldSave) dbContext.SaveChanges();
    }

    public async Task AddAsync(T entry, bool shouldSave = true)
    {
        await dbContext.Set<T>().AddAsync(entry);
        if (shouldSave) await dbContext.SaveChangesAsync();
    }

    public void Update(T entry, bool shouldSave = true)
    {
        dbContext.Set<T>().Update(entry);
        if (shouldSave) dbContext.SaveChanges();
    }

    public async Task UpdateAsync(T entry, bool shouldSave = true)
    {
        dbContext.Set<T>().Update(entry);
        if (shouldSave) await dbContext.SaveChangesAsync();
    }
}