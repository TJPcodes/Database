using System;
using System.IO;

public class DriverModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; }
    public byte[] ProfilePicture { get; set; }
}

public class TeamModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Base { get; set; }
    public byte[] Logo { get; set; }
}

public class RaceModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
}

public class ResultModel
{
    public Guid Id { get; set; }
    public Guid RaceId { get; set; }
    public Guid DriverId { get; set; }
    public Guid TeamId { get; set; }
    public int Position { get; set; }
    public TimeSpan Time { get; set; }
}