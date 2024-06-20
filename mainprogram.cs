using System;
using System.IO;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Starting Formula1 Database Program...");

        using (var stream = new FileStream("formula1.db", FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            var blockStorage = new BlockStorage(stream, 4096);
            var recordStorage = new RecordStorage(blockStorage);
            var database = new Formula1Database(recordStorage);

            var driver = new DriverModel
            {
                Id = Guid.NewGuid(),
                Name = "Lewis Hamilton",
                Age = 36,
                Nationality = "British",
                ProfilePicture = new byte[] { /* some byte data */ }
            };

            database.InsertDriver(driver);

            var foundDriver = database.FindDriver(driver.Id);
            Console.WriteLine($"Found driver: {foundDriver.Name}");
        }
    }
}