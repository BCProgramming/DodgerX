using System;
using System.IO;
using System.Text.RegularExpressions;
namespace DodgerX
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static DodgerX GameObject = null;

        public static void Main(string[] args)
        {

            


            GameObject = new DodgerX();

            try
            {
                GameObject.Run();
            }
            finally
            {
                GameObject.Dispose();
            }

        }
    }
#endif
}

