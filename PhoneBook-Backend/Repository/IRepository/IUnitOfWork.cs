namespace PhoneBook_Backend.Repository.IRepository;

public interface IUnitOfWork
{
    IContactRepository Contact { get; }
    void Save();
}