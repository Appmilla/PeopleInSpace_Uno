using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace PeopleInSpace_Uno.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new PeopleInSpace_Uno.App(), args);
            host.Run();
        }
    }
}
