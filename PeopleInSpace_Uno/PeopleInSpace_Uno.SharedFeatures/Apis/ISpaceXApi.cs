using Refit;
using System.Threading.Tasks;

namespace PeopleInSpace_Uno.SharedFeatures.Apis
{
    public interface ISpaceXApi
    {        
        [Get("/crew")]
        Task<string> GetAllCrew();
    }
}
