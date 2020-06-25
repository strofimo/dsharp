using System;
using System.Html;

namespace TodoApplication
{
    public class Program
    {
        private static IApplication application;

        public static void Main(string[] args)
        {
            application = new TodoApplication();
            application.Run();

            Window.AddEventListener("onbeforeunload", HandleApplicationExit);
        }

        private static void HandleApplicationExit(ElementEvent e)
        {
            application.Dispose();
            application = null;
        }
    }
}
