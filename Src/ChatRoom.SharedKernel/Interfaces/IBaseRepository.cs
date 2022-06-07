namespace ChatRoom.SharedKernel.Interfaces;

public interface IBaseRepository<T>
{
    Task<List<T>> GetAll();
    Task<T> GetById(Guid id);
    Task<bool> Create(T model);
    Task<bool> Delete(Guid id);
}