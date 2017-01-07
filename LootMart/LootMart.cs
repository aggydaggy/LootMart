using System;

namespace LootMart
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class LootMart
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new LootMartGame())
                game.Run();
        }
    }
}
