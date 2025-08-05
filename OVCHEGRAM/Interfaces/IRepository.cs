using System.Linq.Expressions;
using OVCHEGRAM.Migrations;

namespace OVCHEGRAM.Models;

public interface IRepository<T>
{
    public void Add(T entry, bool shouldSave = true);
    public Task AddAsync(T entry, bool shouldSave = true);
    public void Update(T entry, bool shouldSave = true);
    public Task UpdateAsync(T entry, bool shouldSave = true);
}