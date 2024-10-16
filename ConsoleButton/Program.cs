﻿using System;
using System.IO;

class Program
{
    static string saveDirectory = "rajzok";
    static string currentFilePath = "";
    static bool isNewDrawing = true;
    static char[,] drawing;

    static void Main()
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        string[] menuItems = { " Új Rajz", "Rajz Szerkesztés", "Törlés", "Kilépés" };
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

                Console.WriteLine(new string(' ', padding) + "----------------------");
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
                Console.WriteLine(new string(' ', padding) + "----------------------");
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
                        isNewDrawing = true;
                        currentFilePath = "";
                        RunDrawingOption();
                    }
                    else if (selectedIndex == 1)
                    {
                        LoadDrawing();
                    }
                    else if (selectedIndex == 2)
                    {
                        DeleteDrawing();
                    }
                    else if (selectedIndex == 3)
                    {
                        Environment.Exit(0);
                    }
                    break;
            }
        } while (true);
    }

    static void DeleteDrawing()
    {
        Console.Clear();
        string[] files = Directory.GetFiles(saveDirectory, "*.txt");

        if (files.Length == 0)
        {
            Console.WriteLine("Nincs mentett rajz.");
            Console.ReadKey();
            return;
        }

        int selectedIndex = 0;
        int maxLength = files.Length > 0 ? files.Max(f => Path.GetFileNameWithoutExtension(f).Length) : 0;

        do
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int padding = (windowWidth - maxLength - 6) / 2;
            int topPadding = (windowHeight - (files.Length * 3)) / 2;

            Console.SetCursorPosition((windowWidth - "Törlés".Length) / 2, topPadding - 2);
            Console.WriteLine("Törlés");

            Console.SetCursorPosition(0, topPadding);
            

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]).PadRight(maxLength);

                Console.WriteLine(new string(' ', padding) + "----------------------");
                if (i == selectedIndex)
                {
                    Console.Write(new string(' ', padding) + "|  ");
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(fileName);
                    Console.ResetColor();
                    Console.WriteLine("        |");
                }
                else
                {
                    Console.WriteLine(new string(' ', padding) + $"|  {fileName}        |");
                }
                Console.WriteLine(new string(' ', padding) + "----------------------");
            }

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = files.Length - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex++;
                    if (selectedIndex >= files.Length) selectedIndex = 0;
                    break;

                case ConsoleKey.Enter:
                    string fileToDelete = files[selectedIndex];
                    Console.WriteLine($"Biztosan törölni szeretnéd a(z) {Path.GetFileNameWithoutExtension(fileToDelete)} rajzot? (i/n)");
                    var confirmKey = Console.ReadKey(true).Key;
                    if (confirmKey == ConsoleKey.I)
                    {
                        File.Delete(fileToDelete);
                        Console.WriteLine("Rajz törölve.");
                        files = Directory.GetFiles(saveDirectory, "*.txt");
                        if (files.Length == 0)
                        {
                            Console.WriteLine("Nincs több mentett rajz.");
                            Console.ReadKey();
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Törlés megszakítva.");
                    }
                    break;

                case ConsoleKey.Escape:
                    return;
            }
        } while (true);
    }

    static void LoadDrawing()
    {
        Console.Clear();
        string[] files = Directory.GetFiles(saveDirectory, "*.txt");

        if (files.Length == 0)
        {
            Console.WriteLine("Nincs mentett rajz.");
            Console.ReadKey();
            return;
        }

        int selectedIndex = 0;
        int maxLength = files.Length > 0 ? files.Max(f => Path.GetFileNameWithoutExtension(f).Length) : 0;

        do
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int padding = (windowWidth - maxLength - 6) / 2;
            int topPadding = (windowHeight - (files.Length * 3)) / 2;

            Console.SetCursorPosition((windowWidth - "Mentett rajzok".Length) / 2, topPadding - 2);
            Console.WriteLine("Mentett rajzok:");

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]).PadRight(maxLength);

                Console.WriteLine(new string(' ', padding) + "----------------------");
                if (i == selectedIndex)
                {
                    Console.Write(new string(' ', padding) + "|  ");
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(fileName);
                    Console.ResetColor();
                    Console.WriteLine("        |");
                }
                else
                {
                    Console.WriteLine(new string(' ', padding) + $"|  {fileName}        |");
                }
                Console.WriteLine(new string(' ', padding) + "----------------------");
            }

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = files.Length - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex++;
                    if (selectedIndex >= files.Length) selectedIndex = 0;
                    break;

                case ConsoleKey.Enter:
                    currentFilePath = files[selectedIndex];
                    isNewDrawing = false;
                    string[] lines = File.ReadAllLines(currentFilePath);

                    int ww = Console.WindowWidth;
                    int wh = Console.WindowHeight;
                    drawing = new char[wh, ww];
                    for (int i = 0; i < Math.Min(wh, lines.Length); i++)
                    {
                        for (int j = 0; j < Math.Min(ww, lines[i].Length); j++)
                        {
                            drawing[i, j] = lines[i][j];
                        }
                    }

                    Console.Clear();
                    for (int i = 0; i < Math.Min(wh, lines.Length); i++)
                    {
                        Console.WriteLine(lines[i]);
                    }
                    RunDrawingOption();
                    return;

                case ConsoleKey.Escape:
                    return;
            }
        } while (true);
    }

    static void RunDrawingOption()
    {
        int x = 0, y = 0;
        ConsoleKey key;
        int ww = Console.WindowWidth;
        int wh = Console.WindowHeight;
        bool isRunning = true;
        char currentChar = '█';
        ConsoleColor currentColor = ConsoleColor.Gray;

        drawing = new char[wh, ww];
        for (int i = 0; i < wh; i++)
        {
            for (int j = 0; j < ww; j++)
            {
                drawing[i, j] = ' ';
            }
        }

        do
        {
            Console.SetCursorPosition(x, y);
            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && y > 0) y--;
            if (key == ConsoleKey.DownArrow && y < wh - 1) y++;
            if (key == ConsoleKey.LeftArrow && x > 0) x--;
            if (key == ConsoleKey.RightArrow && x < ww - 1) x++;

            switch (key)
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
                    drawing[y, x] = currentChar;
                    break;
            }

            if (key == ConsoleKey.Escape)
            {
                isRunning = false;
                Console.Clear();
                Console.WriteLine("Szeretnéd elmenteni a rajzot? (i/n)");
                var saveKey = Console.ReadKey(true).Key;
                if (saveKey == ConsoleKey.I)
                {
                    if (isNewDrawing)
                    {
                        Console.WriteLine("Adj meg egy fájlnevet a mentéshez:");
                        string fileName = Console.ReadLine();
                        currentFilePath = Path.Combine(saveDirectory, fileName + ".txt");
                    }
                    else
                    {
                        Console.WriteLine("Szeretnéd csak a mostani munkádat elmenteni? (i/n)");
                        var overwriteKey = Console.ReadKey(true).Key;
                        if (overwriteKey != ConsoleKey.N)
                        {
                            Console.WriteLine("Adj meg egy új fájlnevet:");
                            string newFileName = Console.ReadLine();
                            currentFilePath = Path.Combine(saveDirectory, newFileName + ".txt");
                        }
                    }
                    SaveDrawing(currentFilePath);
                }
            }

            Console.SetCursorPosition(0, wh - 1);
            Console.ResetColor();
            Console.Write($"Szín: {currentColor}, Karakter: {currentChar}   ");

        } while (isRunning);
    }

    static void SaveDrawing(string filePath)
    {
        int ww = Console.WindowWidth;
        int wh = Console.WindowHeight;
        char[,] existingDrawing = new char[wh, ww];

        for (int i = 0; i < wh; i++)
        {
            for (int j = 0; j < ww; j++)
            {
                existingDrawing[i, j] = ' ';
            }
        }

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < Math.Min(wh, lines.Length); i++)
            {
                for (int j = 0; j < Math.Min(ww, lines[i].Length); j++)
                {
                    existingDrawing[i, j] = lines[i][j];
                }
            }
        }

        for (int i = 0; i < wh; i++)
        {
            for (int j = 0; j < ww; j++)
            {
                if (drawing[i, j] != ' ')
                {
                    existingDrawing[i, j] = drawing[i, j];
                }
            }
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < wh; i++)
            {
                for (int j = 0; j < ww; j++)
                {
                    writer.Write(existingDrawing[i, j]);
                }
                writer.WriteLine();
            }
        }

        Console.WriteLine($"Rajz elmentve: {filePath}");
        Console.ReadKey();
    }
}
