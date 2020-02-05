using Common;

namespace IRMonitor2
{
    public class Controller
    {
        public class Test
        {
            public string a;
            public int b;
        }

        public static void foo(string a, int b)
        {
            Tracker.LogI("foo");
        }

        public static void bar(string a, Test b)
        {
            Tracker.LogI("bar");
        }
    }
}
