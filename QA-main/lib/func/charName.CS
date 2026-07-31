public class NicknameService
{
    private readonly INicknameRepository _repository; 

    public NicknameService(INicknameRepository repository)
    {
        _repository = repository;
    }
}