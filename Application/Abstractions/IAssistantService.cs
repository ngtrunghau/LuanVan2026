using badmintion.DTO;

namespace badmintion.Interface
{
    public interface IAssistantService
    {
        Task<dynamic> Ask(AssistantAskRequest model);
    }
}
