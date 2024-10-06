using System;
using System.IO;

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
                    else if (selectedIndex == 1)
                    {
                        SaveDrawing();
                    }
                    else if (selectedIndex == 2)
                    {
                        LoadDrawing();
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
        ConsoleColor currentColor = ConsoleColor.Gray;

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
                    currentColor = ConsoleColor.Red;
                    Console.ForegroundColor = currentColor;
                    break;

                case ConsoleKey.D2:
                    currentColor = ConsoleColor.Green;
                    Console.ForegroundColor = currentColor;
                    break;

                case ConsoleKey.D3:
                    currentColor = ConsoleColor.Blue;
                    Console.ForegroundColor = currentColor;
                    break;

                case ConsoleKey.D4:
                    currentColor = ConsoleColor.Gray;
                    Console.ForegroundColor = currentColor;
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

                case ConsoleKey.D8:
                    currentChar = '█';
                    break;

                case ConsoleKey.Spacebar:
                    Console.SetCursorPosition(x, y);
                    Console.Write(currentChar);
                    break;
            }

            if (x == ww - 6 && y == wh - 1 && gomb == ConsoleKey.Enter)
            {
                SaveDrawing();
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

            Console.SetCursorPosition(ww - 6, wh - 1);
            Console.BackgroundColor = (x == ww - 6 && y == wh - 1) ? ConsoleColor.Gray : ConsoleColor.Black;
            Console.ForegroundColor = (x == ww - 6 && y == wh - 1) ? ConsoleColor.Black : ConsoleColor.White;
            Console.Write("Mentés");
            Console.ResetColor();

            Console.SetCursorPosition(0, wh - 1);
            Console.ResetColor();
            Console.Write($"Szín: {currentColor}, Karakter: {currentChar}   ");

        } while (isRunning);
    }

    static void SaveDrawing()
    {
        Console.SetCursorPosition(30, Console.WindowHeight - 1);
        Console.WriteLine("Mentve");

        using (StreamWriter writer = new StreamWriter("drawing.txt"))
        {
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                for (int j = 0; j < Console.WindowWidth; j++)
                {
                    Console.SetCursorPosition(j, i);
                    char c = (char)Console.Read();
                    writer.Write(c);
                }
                writer.WriteLine();
            }
        }

        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.WriteLine("Elmentve");
    }

    static void LoadDrawing()
    {
        Console.Clear();

        if (File.Exists("drawing.txt"))
        {
            string[] buffer = File.ReadAllLines("drawing.txt");
            for (int i = 0; i < buffer.Length; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(buffer[i]);
            }
            Console.WriteLine(" ");
            Console.WriteLine("Rajz betöltve.");
        }
        else
        {
            Console.WriteLine("Nincs mentett rajz.");
        }
        Console.ReadKey();
    }
}
