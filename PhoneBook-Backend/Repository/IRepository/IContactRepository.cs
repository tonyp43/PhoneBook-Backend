using PhoneBook_Backend.Models;

namespace PhoneBook_Backend.Repository.IRepository;

public interface IContactRepository : IRepository<Contact>
{
    void Update(Contact obj);
}