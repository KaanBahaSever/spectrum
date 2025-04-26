using Microsoft.Win32;
using System;
using System.Management; // You need to add System.Management.dll reference

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string contextMenuPath = @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32";
        string defenderPath = @"SOFTWARE\Policies\Microsoft\Windows Defender";

        bool isClassicContextMenuActive = false;
        bool isDefenderDisabled = false;

        // Detect Classic Context Menu
        using (RegistryKey contextMenuKey = Registry.CurrentUser.OpenSubKey(contextMenuPath))
        {
            if (contextMenuKey != null)
            {
                object value = contextMenuKey.GetValue("");
                if (value is string str && str == "")
                {
                    isClassicContextMenuActive = true;
                }
            }
        }

        // Detect Windows Defender status
        using (RegistryKey defenderKey = Registry.LocalMachine.OpenSubKey(defenderPath))
        {
            if (defenderKey != null)
            {
                object value = defenderKey.GetValue("DisableAntiSpyware");
                if (value is int intValue && intValue == 1)
                {
                    isDefenderDisabled = true;
                }
            }
        }

        Console.WriteLine("🔍 Current Status:");
        Console.WriteLine("- Context Menu: " + (isClassicContextMenuActive ? "Classic" : "Modern"));
        Console.WriteLine("- Windows Defender: " + (isDefenderDisabled ? "Disabled" : "Enabled"));

        Console.WriteLine("\nSelect an operation:");
        Console.WriteLine("1 - Enable Classic Context Menu");
        Console.WriteLine("2 - Disable Classic Context Menu (Restore Modern Menu)");
        Console.WriteLine("3 - Disable Windows Defender");
        Console.WriteLine("4 - Enable Windows Defender");
        Console.Write("Your choice: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.CreateSubKey(contextMenuPath))
                    {
                        if (key != null)
                        {
                            key.SetValue("", "", RegistryValueKind.String);
                            Console.WriteLine("✅ Classic Context Menu enabled.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                break;

            case "2":
                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", false);
                    Console.WriteLine("✅ Classic Context Menu disabled. Modern Menu restored.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error or already removed: " + ex.Message);
                }
                break;

            case "3":
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(defenderPath))
                    {
                        if (key != null)
                        {
                            key.SetValue("DisableAntiSpyware", 1, RegistryValueKind.DWord);
                            Console.WriteLine("✅ Windows Defender disabled.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                break;

            case "4":
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(defenderPath))
                    {
                        if (key != null)
                        {
                            key.SetValue("DisableAntiSpyware", 0, RegistryValueKind.DWord);
                            Console.WriteLine("✅ Windows Defender enabled.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                break;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }

        Console.WriteLine("\n🔁 For changes to take full effect, you might need to restart your PC or restart Explorer (for context menu changes).");
        Console.ReadKey();
    }
}
