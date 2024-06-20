using System;
using System.Collections.Generic;
using System.Text.Json;

public class Formula1Database : IFormula1Database
{
    private IRecordStorage _recordStorage;
    private IIndex<Guid, uint> _driverIndex;
    private IIndex<Guid, uint> _teamIndex;
    private IIndex<Guid, uint> _raceIndex;
    private IIndex<Guid, uint> _resultIndex;

    public Formula1Database(IRecordStorage recordStorage)
    {
        _recordStorage = recordStorage;
        _driverIndex = new BTree<Guid, uint>(3); // Degree of 3 for B-Tree
        _teamIndex = new BTree<Guid, uint>(3);
        _raceIndex = new BTree<Guid, uint>(3);
        _resultIndex = new BTree<Guid, uint>(3);
    }

    public void InsertDriver(DriverModel driver)
    {
        var data = Serialize(driver);
        var recordId = _recordStorage.Create(data);
        _driverIndex.Insert(driver.Id, recordId);
    }

    public void DeleteDriver(DriverModel driver)
    {
        if (_driverIndex.Get(driver.Id) is var entry && entry != null)
        {
            _recordStorage.Delete(entry.Item2);
            _driverIndex.Delete(driver.Id);
        }
    }

    public void UpdateDriver(DriverModel driver)
    {
        DeleteDriver(driver);
        InsertDriver(driver);
    }

    public DriverModel FindDriver(Guid id)
    {
        if (_driverIndex.Get(id) is var entry && entry != null)
        {
            var data = _recordStorage.Find(entry.Item2);
            return Deserialize<DriverModel>(data);
        }
        return null;
    }

    public IEnumerable<DriverModel> FindDriversByAge(int age)
    {
        List<DriverModel> drivers = new List<DriverModel>();
        foreach (var entry in _driverIndex.LargerThanOrEqualTo(Guid.Empty))
        {
            var data = _recordStorage.Find(entry.Item2);
            var driver = Deserialize<DriverModel>(data);
            if (driver.Age == age)
            {
                drivers.Add(driver);
            }
        }
        return drivers;
    }

    public void InsertTeam(TeamModel team)
    {
        var data = Serialize(team);
        var recordId = _recordStorage.Create(data);
        _teamIndex.Insert(team.Id, recordId);
    }

    public void DeleteTeam(TeamModel team)
    {
        if (_teamIndex.Get(team.Id) is var entry && entry != null)
        {
            _recordStorage.Delete(entry.Item2);
            _teamIndex.Delete(team.Id);
        }
    }

    public void UpdateTeam(TeamModel team)
    {
        DeleteTeam(team);
        InsertTeam(team);
    }

    public TeamModel FindTeam(Guid id)
    {
        if (_teamIndex.Get(id) is var entry && entry != null)
        {
            var data = _recordStorage.Find(entry.Item2);
            return Deserialize<TeamModel>(data);
        }
        return null;
    }

    public void InsertRace(RaceModel race)
    {
        var data = Serialize(race);
        var recordId = _recordStorage.Create(data);
        _raceIndex.Insert(race.Id, recordId);
    }

    public void DeleteRace(RaceModel race)
    {
        if (_raceIndex.Get(race.Id) is var entry && entry != null)
        {
            _recordStorage.Delete(entry.Item2);
            _raceIndex.Delete(race.Id);
        }
    }

    public void UpdateRace(RaceModel race)
    {
        DeleteRace(race);
        InsertRace(race);
    }

    public RaceModel FindRace(Guid id)
    {
        if (_raceIndex.Get(id) is var entry && entry != null)
        {
            var data = _recordStorage.Find(entry.Item2);
            return Deserialize<RaceModel>(data);
        }
        return null;
    }

    public void InsertResult(ResultModel result)
    {
        var data = Serialize(result);
        var recordId = _recordStorage.Create(data);
        _resultIndex.Insert(result.Id, recordId);
    }

    public void DeleteResult(ResultModel result)
    {
        if (_resultIndex.Get(result.Id) is var entry && entry != null)
        {
            _recordStorage.Delete(entry.Item2);
            _resultIndex.Delete(result.Id);
        }
    }

    public void UpdateResult(ResultModel result)
    {
        DeleteResult(result);
        InsertResult(result);
    }

    public ResultModel FindResult(Guid id)
    {
        if (_resultIndex.Get(id) is var entry && entry != null)
        {
            var data = _recordStorage.Find(entry.Item2);
            return Deserialize<ResultModel>(data);
        }
        return null;
    }

    public IEnumerable<ResultModel> FindResultsByRace(Guid raceId)
    {
        List<ResultModel> results = new List<ResultModel>();
        foreach (var entry in _resultIndex.LargerThanOrEqualTo(Guid.Empty))
        {
            var data = _recordStorage.Find(entry.Item2);
            var result = Deserialize<ResultModel>(data);
            if (result.RaceId == raceId)
            {
                results.Add(result);
            }
        }
        return results;
    }

    public IEnumerable<ResultModel> FindResultsByDriver(Guid driverId)
    {
        List<ResultModel> results = new List<ResultModel>();
        foreach (var entry in _resultIndex.LargerThanOrEqualTo(Guid.Empty))
        {
            var data = _recordStorage.Find(entry.Item2);
            var result = Deserialize<ResultModel>(data);
            if (result.DriverId == driverId)
            {
                results.Add(result);
            }
        }
        return results;
    }

    private byte[] Serialize<T>(T obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    private T Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(data);
    }
}
