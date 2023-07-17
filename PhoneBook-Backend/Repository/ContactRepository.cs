using PhoneBook_Backend.Data;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Repository.IRepository;

namespace PhoneBook_Backend.Repository;

public class ContactRepository : Repository<Contact>, IContactRepository
{
    private ApplicationDbContext _db;

    public ContactRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Contact obj)
    {
        _db.Contacts.Update(obj);
    }
}