using System;
using System.Collections.Generic;

public interface IFormula1Database
{
    void InsertDriver(DriverModel driver);
    void DeleteDriver(DriverModel driver);
    void UpdateDriver(DriverModel driver);
    DriverModel FindDriver(Guid id);
    IEnumerable<DriverModel> FindDriversByAge(int age);

    void InsertTeam(TeamModel team);
    void DeleteTeam(TeamModel team);
    void UpdateTeam(TeamModel team);
    TeamModel FindTeam(Guid id);

    void InsertRace(RaceModel race);
    void DeleteRace(RaceModel race);
    void UpdateRace(RaceModel race);
    RaceModel FindRace(Guid id);

    void InsertResult(ResultModel result);
    void DeleteResult(ResultModel result);
    void UpdateResult(ResultModel result);
    ResultModel FindResult(Guid id);
    IEnumerable<ResultModel> FindResultsByRace(Guid raceId);
    IEnumerable<ResultModel> FindResultsByDriver(Guid driverId);
}

public interface IBlockStorage
{
    int BlockContentSize { get; }
    int BlockHeaderSize { get; }
    int BlockSize { get; }
    IBlock Find(uint blockId);
    IBlock CreateNew();
}

public interface IBlock : IDisposable
{
    uint Id { get; }
    long GetHeader(int field);
    void SetHeader(int field, long value);
    void Read(byte[] dst, int dstOffset, int srcOffset, int count);
    void Write(byte[] src, int srcOffset, int dstOffset, int count);
}


public interface IRecordStorage
{
    void Update(uint recordId, byte[] data);
    byte[] Find(uint recordId);
    uint Create();
    uint Create(byte[] data);
    uint Create(Func<uint, byte[]> dataGenerator);
    void Delete(uint recordId);
}

public interface IIndex<K, V>
{
    void Insert(K key, V value);
    Tuple<K, V> Get(K key);
    IEnumerable<Tuple<K, V>> LargerThanOrEqualTo(K key);
    IEnumerable<Tuple<K, V>> LargerThan(K key);
    IEnumerable<Tuple<K, V>> LessThanOrEqualTo(K key);
    IEnumerable<Tuple<K, V>> LessThan(K key);
    bool Delete(K key, V value, IComparer<V> valueComparer = null);
    bool Delete(K key);
}

