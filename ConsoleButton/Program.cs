using System;
using System.IO;

class Program
{
    static string saveDirectory = "rajzok";
    static string currentFilePath = ""; // Store the file path of the currently loaded drawing
    static bool isNewDrawing = true;    // Track whether it's a new drawing or loaded one
    static char[,] drawing;             // Store the drawing

    static void Main()
    {
        // Create save directory if it doesn't exist
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
                    if (selectedIndex == 0) // New drawing
                    {
                        isNewDrawing = true;  // Mark as a new drawing
                        currentFilePath = ""; // Clear the current file path for a new drawing
                        RunDrawingOption();
                    }
                    else if (selectedIndex == 1) // Load saved drawing
                    {
                        LoadDrawing();
                    }
                    else if (selectedIndex == 2) // Delete drawing
                    {
                        DeleteDrawing();
                    }
                    else if (selectedIndex == 3) // Exit
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

        Console.WriteLine("Mentett rajzok:");
        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(files[i])}");
        }

        Console.Write("Válassz egy rajzot törlésre (szám): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= files.Length)
        {
            string fileToDelete = files[choice - 1];
            Console.WriteLine($"Biztosan törölni szeretnéd a(z) {Path.GetFileNameWithoutExtension(fileToDelete)} rajzot? (i/n)");
            var confirmKey = Console.ReadKey(true).Key;
            if (confirmKey == ConsoleKey.I)
            {
                File.Delete(fileToDelete);
                Console.WriteLine("Rajz törölve.");
            }
            else
            {
                Console.WriteLine("Törlés megszakítva.");
            }
        }
        else
        {
            Console.WriteLine("Érvénytelen választás.");
        }

        Console.ReadKey();
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
                    drawing[y, x] = currentChar; // Save the drawn character in the array
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
                        // Ask for a new filename if it's a new drawing
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

        // Initialize the existingDrawing array with blank spaces
        for (int i = 0; i < wh; i++)
        {
            for (int j = 0; j < ww; j++)
            {
                existingDrawing[i, j] = ' ';
            }
        }

        // Read the existing content if the file exists
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

        // Merge the existing content with the current drawing
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

        // Save the combined result back to the file
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

        Console.WriteLine("Mentett rajzok:");
        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(files[i])}");
        }

        Console.Write("Válassz egy rajzot (szám): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= files.Length)
        {
            currentFilePath = files[choice - 1]; // Store the selected file's path
            isNewDrawing = false; // Mark it as an existing drawing
            string[] lines = File.ReadAllLines(currentFilePath);

            // Initialize the drawing array with the saved content
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

            // Display the loaded drawing
            Console.Clear();
            for (int i = 0; i < Math.Min(wh, lines.Length); i++)
            {
                Console.WriteLine(lines[i]);
            }

            // Allow the user to continue editing the loaded drawing
            RunDrawingOption();
        }
        else
        {
            Console.WriteLine("Érvénytelen választás.");
        }

        Console.ReadKey();
    }
}
