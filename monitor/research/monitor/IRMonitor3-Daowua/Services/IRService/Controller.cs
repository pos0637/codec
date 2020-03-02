using Common;

namespace IRService
{
    public class Controller
    {
        public static string login(string username, string password)
        {
            Tracker.LogI("foo");
            return "success";
        }

        public static void logout()
        {
            Tracker.LogI("bar");
        }
    }
}
