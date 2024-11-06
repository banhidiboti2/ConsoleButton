using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

class Program
{
    static string saveDirectory = "rajzok";
    static char[,] drawing = new char[Console.WindowHeight, Console.WindowWidth];
    static string currentFilePath = "";
    static bool isNewDrawing = true;

    public class DrawingElement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ConsoleColor Color { get; set; }
        public char Character { get; set; }
    }

    static ConsoleColor[,] colorDrawing = new ConsoleColor[Console.WindowHeight, Console.WindowWidth];

    static string connectionString = "Data Source=drawingDatabase.db;Version=3;";


    static void Main()
    {

        
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        InitializeDatabase();
        //SaveDrawingToDatabase("SampleDrawing");

        string[] menuItems = { " Új Rajz", "Rajz Szerkesztés", "Törlés", "Kilépés" };
        int selectedIndex = 0;

        int maxLength = menuItems.Max(item => item.Length);

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


    static void InitializeDatabase()
    {
        if (!File.Exists("drawingDatabase.db"))
        {
            SQLiteConnection.CreateFile("drawingDatabase.db");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createDrawingsTable = @"
                    CREATE TABLE IF NOT EXISTS Drawings (
                        DrawingID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT UNIQUE NOT NULL
                    );";

                string createPointsTable = @"
                    CREATE TABLE IF NOT EXISTS Points (
                        PointID INTEGER PRIMARY KEY AUTOINCREMENT,
                        DrawingID INTEGER NOT NULL,
                        X INTEGER NOT NULL,
                        Y INTEGER NOT NULL,
                        Color TEXT NOT NULL,
                        Character TEXT NOT NULL,
                        FOREIGN KEY (DrawingID) REFERENCES Drawings(DrawingID)
                    );";

                using (var command = new SQLiteCommand(createDrawingsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createPointsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
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
        int maxLength = files.Max(f => Path.GetFileNameWithoutExtension(f).Length);

        do
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int padding = (windowWidth - maxLength - 6) / 2;
            int topPadding = Math.Max(0, (windowHeight - (files.Length * 3)) / 2);

            Console.SetCursorPosition((windowWidth - "Törlés".Length) / 2, Math.Max(0, topPadding - 2));
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
        int maxLength = files.Max(f => Path.GetFileNameWithoutExtension(f).Length);

        do
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int padding = (windowWidth - maxLength - 6) / 2;
            int topPadding = Math.Max(0, (windowHeight - (files.Length * 3)) / 2);

            Console.SetCursorPosition((windowWidth - "Mentett rajzok".Length) / 2, Math.Max(0, topPadding - 2));
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
                    LoadDrawingFromFile(currentFilePath);
                    RunDrawingOption(); 
                    return;

                case ConsoleKey.Escape:
                    return;
            }
        } while (true);
    }

    static void LoadDrawingFromFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        int ww = Console.WindowWidth;
        int wh = Console.WindowHeight;
        drawing = new char[wh, ww];
        colorDrawing = new ConsoleColor[wh, ww];

        for (int i = 0; i < wh; i++)
        {
            for (int j = 0; j < ww; j++)
            {
                drawing[i, j] = ' ';
                colorDrawing[i, j] = ConsoleColor.Gray;
            }
        }

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            ConsoleColor color;
            Enum.TryParse(parts[2], out color);
            char character = parts[3][0];

            drawing[y, x] = character;
            colorDrawing[y, x] = color;

            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(character);
            Console.ResetColor();
        }
    }

    static void DisplayDrawing(string drawingName)
    {
        Console.Clear();

        int ww = Console.WindowWidth;
        int wh = Console.WindowHeight;

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            int drawingID;
            using (var command = new SQLiteCommand("SELECT DrawingID FROM Drawings WHERE Name = @Name;", connection))
            {
                command.Parameters.AddWithValue("@Name", drawingName);
                drawingID = Convert.ToInt32(command.ExecuteScalar());
            }

            using (var command = new SQLiteCommand("SELECT X, Y, Color, Character FROM Points WHERE DrawingID = @DrawingID;", connection))
            {
                command.Parameters.AddWithValue("@DrawingID", drawingID);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int x = reader.GetInt32(0);
                        int y = reader.GetInt32(1);
                        ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), reader.GetString(2));
                        char character = reader.GetString(3)[0];

                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = color;
                        Console.Write(character);
                    }
                }
            }

            Console.ResetColor();
            Console.SetCursorPosition(0, wh - 1);
            Console.ReadKey();
        }
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

        if (isNewDrawing)
        {
            drawing = new char[wh, ww];
            for (int i = 0; i < wh; i++)
            {
                for (int j = 0; j < ww; j++)
                {
                    drawing[i, j] = ' ';
                }
            }
        }
        else
        {
            DisplayDrawing(currentFilePath); 
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
                    break;

                case ConsoleKey.D2:
                    currentColor = ConsoleColor.Green;
                    break;

                case ConsoleKey.D3:
                    currentColor = ConsoleColor.Blue;
                    break;

                case ConsoleKey.D4:
                    currentColor = ConsoleColor.Gray;
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
                    Console.ForegroundColor = currentColor;
                    Console.Write(currentChar);
                    Console.ResetColor();
                    drawing[y, x] = currentChar;
                    colorDrawing[y, x] = currentColor;
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
                    Console.WriteLine("Adj meg egy fájlnevet a mentéshez:");
                    string fileName = Console.ReadLine() ?? "default";
                    SaveDrawing(Path.Combine(saveDirectory, fileName + ".txt"));
                }
            }

            Console.SetCursorPosition(0, wh - 1);
            Console.ResetColor();
            Console.Write($"Szín: {currentColor}, Karakter: {currentChar}   ");

        } while (isRunning);
    }



    static void SaveDrawingToDatabase(string drawingName)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Adatbázis kapcsolat létrejött.");

            int drawingID;

            using (var insertDrawingCommand = new SQLiteCommand("INSERT OR IGNORE INTO Drawings (Name) VALUES (@Name);", connection))
            {
                insertDrawingCommand.Parameters.AddWithValue("@Name", drawingName);
                insertDrawingCommand.ExecuteNonQuery();
                Console.WriteLine("Rajz hozzáadása (ha még nem létezik).");
            }

            using (var selectDrawingCommand = new SQLiteCommand("SELECT DrawingID FROM Drawings WHERE Name = @Name;", connection))
            {
                selectDrawingCommand.Parameters.AddWithValue("@Name", drawingName);
                drawingID = Convert.ToInt32(selectDrawingCommand.ExecuteScalar());
                Console.WriteLine("Rajz ID lekérve: " + drawingID);
            }

            using (var transaction = connection.BeginTransaction())
            {
                using (var deletePointsCommand = new SQLiteCommand("DELETE FROM Points WHERE DrawingID = @DrawingID;", connection))
                {
                    deletePointsCommand.Parameters.AddWithValue("@DrawingID", drawingID);
                    deletePointsCommand.ExecuteNonQuery();
                    Console.WriteLine("Előző pontok törölve.");
                }

                using (var insertPointCommand = new SQLiteCommand(
                    "INSERT INTO Points (DrawingID, X, Y, Color, Character) VALUES (@DrawingID, @X, @Y, @Color, @Character);", connection))
                {
                    insertPointCommand.Parameters.AddWithValue("@DrawingID", drawingID);

                    for (int y = 0; y < Console.WindowHeight; y++)
                    {
                        for (int x = 0; x < Console.WindowWidth; x++)
                        {
                            char character = drawing[y, x];
                            ConsoleColor color = colorDrawing[y, x];

                            if (character != ' ')
                            {
                                insertPointCommand.Parameters.AddWithValue("@X", x);
                                insertPointCommand.Parameters.AddWithValue("@Y", y);
                                insertPointCommand.Parameters.AddWithValue("@Color", color.ToString());
                                insertPointCommand.Parameters.AddWithValue("@Character", character.ToString());

                                insertPointCommand.ExecuteNonQuery();
                                Console.WriteLine($"Pont mentve: RajzID={drawingID}, X={x}, Y={y}, Szín={color}, Karakter={character}");

                                insertPointCommand.Parameters.Clear();
                                insertPointCommand.Parameters.AddWithValue("@DrawingID", drawingID);
                            }
                        }
                    }
                }

                transaction.Commit();
                Console.WriteLine("Minden pont elmentve és tranzakció lezárva.");
            }
        }
        Console.WriteLine($"Rajz mentve az adatbázisba: {drawingName}");
    }




    static void SaveDrawing(string filePath)
    {
        int ww = Console.WindowWidth;
        int wh = Console.WindowHeight;

        List<DrawingElement> elements = new List<DrawingElement>();

        for (int i = 0; i < wh; i++)
        {
            for (int j = 0; j < ww; j++)
            {
                if (drawing[i, j] != ' ') 
                {
                    elements.Add(new DrawingElement
                    {
                        X = j,
                        Y = i,
                        Color = colorDrawing[i, j], 
                        Character = drawing[i, j]
                    });
                }
            }
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var element in elements)
            {
                writer.WriteLine($"{element.X},{element.Y},{element.Color},{element.Character}");
            }
        }

        Console.WriteLine($"Rajz elmentve: {filePath}");
    }


 
}