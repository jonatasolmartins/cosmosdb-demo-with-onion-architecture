namespace ChatRoom.Infrastructure.Database.Implementations;

public class BaseRepository<T>
{
    public Task<List<T>> GetAll()
    {
        return null;
    }

    public Task<T> GetById(Guid id)
    {
        return null;
    }

    public Task<bool> Create(T model)
    {
        return null;
    }

    public Task<bool> Delete(Guid id)
    {
        return null;
    }
}