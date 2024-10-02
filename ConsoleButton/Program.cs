using System;

class Program
{
    static void Main()
    {
        string[] menuItems = { "Belépés", "Regisztráció", "Exit" };
        int selectedIndex = 0;

        int maxLength = 0;
        foreach (var item in menuItems)
        {
            if (item.Length > maxLength)
            {
                maxLength = item.Length;
            }
        }

        while (true)
        {
            Console.Clear();
            for (int i = 0; i < menuItems.Length; i++)
            {
                string paddedItem = menuItems[i].PadRight(maxLength);

                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("------------------");
                    Console.WriteLine($"|  {paddedItem}  |");
                    Console.WriteLine("------------------");
                }
                else
                {
                    Console.WriteLine("------------------");
                    Console.WriteLine($"|  {paddedItem}  |");
                    Console.WriteLine("------------------");
                }
                Console.ResetColor();
            }

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = menuItems.Length - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex++;
                    if (selectedIndex >= menuItems.Length) selectedIndex = 0;
                    break;

                case ConsoleKey.Enter:
                    Console.Clear();
                    Console.WriteLine($"Selected: {menuItems[selectedIndex]}");
                    return;  
            }

        }
    }
}
