using PhoneBook_Backend.Data;
using PhoneBook_Backend.Repository.IRepository;
using PhoneBook_Backend.Models;

namespace PhoneBook_Backend.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _db;
    public IContactRepository Contact { get; private set; }


    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Contact = new ContactRepository(_db);
    }


    public void Save()
    {
        _db.SaveChanges();
    }
}