using System;
using System.Collections.Generic;
using System.IO;

public class BlockStorage : IBlockStorage
{
    private Stream _stream;
    private int _blockSize;
    private int _blockContentSize;
    private int _blockHeaderSize;

    public BlockStorage(Stream stream, int blockSize)
    {
        _stream = stream;
        _blockSize = blockSize;
        _blockHeaderSize = 16;
        _blockContentSize = _blockSize - _blockHeaderSize;

        if (_stream.Length % _blockSize != 0)
        {
            _stream.SetLength((_stream.Length / _blockSize + 1) * _blockSize);
        }
    }

    public int BlockContentSize => _blockContentSize;
    public int BlockHeaderSize => _blockHeaderSize;
    public int BlockSize => _blockSize;

    public IBlock Find(uint blockId)
    {
        return new Block(this, blockId);
    }

    public IBlock CreateNew()
    {
        _stream.SetLength(_stream.Length + _blockSize);
        return new Block(this, (uint)(_stream.Length / _blockSize - 1));
    }

    internal void ReadBlock(uint blockId, byte[] buffer)
    {
        _stream.Seek(blockId * _blockSize, SeekOrigin.Begin);
        _stream.Read(buffer, 0, _blockSize);
    }

    internal void WriteBlock(uint blockId, byte[] buffer)
    {
        _stream.Seek(blockId * _blockSize, SeekOrigin.Begin);
        _stream.Write(buffer, 0, _blockSize);
        _stream.Flush();
    }
}

public class Block : IBlock
{
    private BlockStorage _storage;
    private uint _blockId;
    private byte[] _buffer;
    private bool _isDirty;

    public Block(BlockStorage storage, uint blockId)
    {
        _storage = storage;
        _blockId = blockId;
        _buffer = new byte[_storage.BlockSize];
        _storage.ReadBlock(blockId, _buffer);
        _isDirty = false;
    }

    public uint Id => _blockId;

    public long GetHeader(int field)
    {
        int offset = field * sizeof(long);
        return BitConverter.ToInt64(_buffer, offset);
    }

    public void SetHeader(int field, long value)
    {
        int offset = field * sizeof(long);
        BitConverter.GetBytes(value).CopyTo(_buffer, offset);
        _isDirty = true;
    }

    public void Read(byte[] dst, int dstOffset, int srcOffset, int count)
    {
        Array.Copy(_buffer, _storage.BlockHeaderSize + srcOffset, dst, dstOffset, count);
    }

    public void Write(byte[] src, int srcOffset, int dstOffset, int count)
    {
        Array.Copy(src, srcOffset, _buffer, _storage.BlockHeaderSize + dstOffset, count);
        _isDirty = true;
    }

    public void Dispose()
    {
        if (_isDirty)
        {
            _storage.WriteBlock(_blockId, _buffer);
        }
    }
}

public class RecordStorage : IRecordStorage
{
    private IBlockStorage _blockStorage;

    public RecordStorage(IBlockStorage blockStorage)
    {
        _blockStorage = blockStorage;
    }

    public void Update(uint recordId, byte[] data)
    {
        Delete(recordId);
        Create(data);
    }

    public byte[] Find(uint recordId)
    {
        List<byte> result = new List<byte>();
        IBlock block = _blockStorage.Find(recordId);
        while (block != null)
        {
            int contentLength = (int)block.GetHeader(2);
            byte[] buffer = new byte[contentLength];
            block.Read(buffer, 0, 0, contentLength);
            result.AddRange(buffer);

            uint nextBlockId = (uint)block.GetHeader(0);
            block = nextBlockId == 0 ? null : _blockStorage.Find(nextBlockId);
        }
        return result.ToArray();
    }

    public uint Create()
    {
        IBlock block = _blockStorage.CreateNew();
        return block.Id;
    }

    public uint Create(byte[] data)
    {
        int offset = 0;
        IBlock firstBlock = _blockStorage.CreateNew();
        IBlock currentBlock = firstBlock;
        while (offset < data.Length)
        {
            int chunkSize = Math.Min(data.Length - offset, _blockStorage.BlockContentSize);
            currentBlock.Write(data, offset, 0, chunkSize);
            currentBlock.SetHeader(2, chunkSize);

            offset += chunkSize;
            if (offset < data.Length)
            {
                IBlock nextBlock = _blockStorage.CreateNew();
                currentBlock.SetHeader(0, nextBlock.Id);
                currentBlock.Dispose();
                currentBlock = nextBlock;
            }
        }
        currentBlock.Dispose();
        return firstBlock.Id;
    }

    public uint Create(Func<uint, byte[]> dataGenerator)
    {
        uint recordId = Create();
        byte[] data = dataGenerator(recordId);
        Update(recordId, data);
        return recordId;
    }

    public void Delete(uint recordId)
    {
        IBlock block = _blockStorage.Find(recordId);
        while (block != null)
        {
            block.SetHeader(4, 1);
            uint nextBlockId = (uint)block.GetHeader(0);
            block.Dispose();
            block = nextBlockId == 0 ? null : _blockStorage.Find(nextBlockId);
        }
    }
}
