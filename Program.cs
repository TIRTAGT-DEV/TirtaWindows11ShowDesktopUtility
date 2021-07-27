using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace TirtaWindows11ShowDesktopUtility
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Window Manager user32.dll IntPTR
        /// </summary>
        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;

        /// <summary>
        /// Hide selected window
        /// </summary>
        const int SW_HIDE = 0;

        /// <summary>
        /// Show selected window
        /// </summary>
        const int SW_SHOW = 5;

        /// <summary>
        /// String that will be written into the marker files, it's basically just a placeholder.
        /// </summary>
        const string StateMarkerContent = "Yo!";

        /// <summary>
        /// Remmeber state in order to perform Show/Hide on the desktop.
        /// False = Hide Only, True = Hide and Show alternating.
        /// </summary>
        const Boolean RememberState = false;

        /// <summary>
        /// Current Application directory.
        /// </summary>
        static string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            // Minimize the console app
            ShowWindow(GetConsoleWindow(), SW_HIDE);

            if (args.Contains("--setup")) {
                // If there is a --setup argument

                // Show the console app
                ShowWindow(GetConsoleWindow(), SW_SHOW);

                // Make a marker that we are in setup mode
                try
                {
                    // Create and Write a setup marker file
                    System.IO.FileStream a = System.IO.File.Create(ApplicationPath + @"SetupMode.TW11SDU", Encoding.ASCII.GetByteCount(StateMarkerContent), System.IO.FileOptions.RandomAccess);
                    a.Write(Encoding.ASCII.GetBytes(StateMarkerContent), 0, Encoding.ASCII.GetByteCount(StateMarkerContent));
                    a.Flush();
                    a.Dispose();
                }
                catch (Exception _)
                {
                    Console.WriteLine("Got exception while trying to make setup marker, Exception message : " + _.Message);
                }

                // Let user know the instructions
                Console.WriteLine("==== Currently Running in Setup Mode ====");
                Console.WriteLine("Setup Mode is unlocked, please re-run the exe by double-clicking.");
                Console.WriteLine("Don't forget to close this current instance, by pressing enter or clicking the 'X' button.");
                Console.WriteLine("\n\nAnother utility from 'TIRTAGT Developer' => https://github.com/TIRTAGT-DEV");
                Console.ReadLine();
                return;
            }
            else if (System.IO.File.Exists(ApplicationPath + @"SetupMode.TW11SDU"))
            {
                // Show the console app
                ShowWindow(GetConsoleWindow(), SW_SHOW);

                // Delete the Setup marker, to avoid looping into the setup again and again.
                try
                {
                    System.IO.File.Delete(ApplicationPath + @"SetupMode.TW11SDU");
                }
                catch (Exception _)
                {
                    Console.WriteLine("Got exception while trying to delete setup marker, Exception message : " + _.Message);
                }
                Console.WriteLine("==== Currently Running in Setup Mode ====");
                Console.WriteLine("Please make a shortcut to run this app in order to minimize your window.");
                Console.WriteLine("After that, close this app by clicking the 'X' icon or pressing enter.");
                Console.WriteLine("\n\nAnother utility from 'TIRTAGT Developer' => https://github.com/TIRTAGT-DEV");
                Console.ReadLine();
                return;
            }

            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);

            // Check if the state marker file exist and we should Remember the state.
            if (System.IO.File.Exists(ApplicationPath + @"ReverseMode.TW11SDU") && RememberState)
            {
                // Instead of showing the desktop, show the foreground application application.
                SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);

                // Delete the state marker, so that in the next execution, we will show the desktop.
                try
                {
                    System.IO.File.Delete(ApplicationPath + @"ReverseMode.TW11SDU");
                }
                catch (Exception _)
                {
                    Console.WriteLine("Got exception while trying to delete state marker, Exception message : " + _.Message);
                }
                // Quit the app instantly
                return;
            }
            else
            {
                // Show the desktop, and hide all current foreground application.
                SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);

                // If Remember state option was enabled.
                if (RememberState) {
                    #pragma warning disable CS0162 // Ignore Unreachable code detected
                    // Write a state marker so that in the next execution, we will bring all apps to foreground.
                    try
                    {
                        System.IO.FileStream a = System.IO.File.Create(ApplicationPath + @"ReverseMode.TW11SDU", Encoding.ASCII.GetByteCount(StateMarkerContent), System.IO.FileOptions.RandomAccess);
                        a.Write(Encoding.ASCII.GetBytes(StateMarkerContent), 0, Encoding.ASCII.GetByteCount(StateMarkerContent));
                        a.Flush();
                        a.Dispose();
                    }
                    catch (Exception _)
                    {
                        Console.WriteLine("Got exception while trying to create state marker, Exception message : " + _.Message);
                    }
                    #pragma warning restore CS0162 // Ignore Unreachable code detected
                }

                // Quit the app instantly
                return;
            }
        }
    }
}
