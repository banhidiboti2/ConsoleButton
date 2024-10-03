using System;

class Program
{
    static void Main()
    {
        string[] menuItems = { "Rajzolás", "Mentés", "Mentett Rajz", "Kilépés" };
        int selectedIndex = 0;

        int maxLength = 0;
        foreach (var item in menuItems)
        {
            if (item.Length > maxLength)
            {
                maxLength = item.Length;
            }
        }

        do
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int padding = (windowWidth - maxLength - 6) / 2;
            int topPadding = (windowHeight - (menuItems.Length * 3)) / 2;

            Console.SetCursorPosition(0, topPadding);

            for (int i = 0; i < menuItems.Length; i++)
            {
                string paddedItem = menuItems[i].PadRight(maxLength);

                Console.WriteLine(new string(' ', padding) + "------------------");
                if (i == selectedIndex)
                {
                    Console.Write(new string(' ', padding) + "|  ");
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(paddedItem);
                    Console.ResetColor();
                    Console.WriteLine("  |");
                }
                else
                {
                    Console.WriteLine(new string(' ', padding) + $"|  {paddedItem}  |");
                }
                Console.WriteLine(new string(' ', padding) + "------------------");
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
                    if (selectedIndex == 0)
                    {
                        RunFirstOptionCode();
                    }
                    return;
            }
        } while (true);
    }

    static void RunFirstOptionCode()
    {

        int x = 0, y = 0;
        ConsoleKey gomb;
        int ww = Console.WindowWidth;
        int wh = Console.WindowHeight;
        bool isRunning = true;
        char currentChar = '█';

        do
        {
            Console.SetCursorPosition(x, y);
            gomb = Console.ReadKey(true).Key;

            if (gomb == ConsoleKey.UpArrow && y > 0) y--;
            if (gomb == ConsoleKey.DownArrow && y < wh - 1) y++;
            if (gomb == ConsoleKey.LeftArrow && x > 0) x--;
            if (gomb == ConsoleKey.RightArrow && x < ww - 1) x++;


            switch (gomb)
            {
                case ConsoleKey.D1:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case ConsoleKey.D2:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case ConsoleKey.D3:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;

                case ConsoleKey.D4:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case ConsoleKey.D5:
                    currentChar = '▓';
                    break;

                case ConsoleKey.D6:
                    currentChar = '▒';
                    break;

                case ConsoleKey.D7:
                    currentChar = '░';
                    break;

                case ConsoleKey.Spacebar:
                    Console.SetCursorPosition(x, y);
                    Console.Write(currentChar);
                    break;
            }


            if (gomb == ConsoleKey.Tab)
            {
                while (true)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write('█');

                    gomb = Console.ReadKey(true).Key;

                    if (gomb == ConsoleKey.UpArrow && y > 0) y--;
                    if (gomb == ConsoleKey.DownArrow && y < wh - 1) y++;
                    if (gomb == ConsoleKey.LeftArrow && x > 0) x--;
                    if (gomb == ConsoleKey.RightArrow && x < ww - 1) x++;

                    if (gomb == ConsoleKey.Spacebar) break;
                }
            }

            if (gomb == ConsoleKey.Escape)
            {
                isRunning = false;
            }

        } while (isRunning);
    }
}
