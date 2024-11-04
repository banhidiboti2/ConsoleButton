using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Drawing
{
    [Key]
    public int DrawingId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public List<Point> Points { get; set; } = new List<Point>();
}

public class Point
{
    [Key]
    public int PointId { get; set; }

    [Required]
    public int X { get; set; }

    [Required]
    public int Y { get; set; }

    public int DrawingId { get; set; }

    public Drawing Drawing { get; set; } = null!;
}

public class DrawingContext : DbContext
{
    public DbSet<Drawing> Drawings { get; set; }
    public DbSet<Point> Points { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=your_server_name;Database=DrawingDB;Trusted_Connection=True;");
    }
}

class DrawingProgram
{
    static void Main()
    {
        using (var context = new DrawingContext())
        {
            context.Database.EnsureCreated();

            // Creating a new Drawing with Points
            var drawing = new Drawing { Name = "Sample Drawing" };
            drawing.Points.Add(new Point { X = 10, Y = 20 });
            drawing.Points.Add(new Point { X = 15, Y = 25 });

            context.Drawings.Add(drawing);
            context.SaveChanges();

            // Fetch and display saved drawings with points
            foreach (var d in context.Drawings.Include(d => d.Points))
            {
                Console.WriteLine($"Drawing: {d.Name}");
                foreach (var p in d.Points)
                {
                    Console.WriteLine($"  Point: ({p.X}, {p.Y})");
                }
            }
        }
    }
}
