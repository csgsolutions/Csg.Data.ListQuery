namespace Csg.ListQuery.Server;

public struct PageInfo
{
    private int _offset;

    public PageInfo(int offset)
    {
        _offset = offset;
    }

    public int Offset => _offset;
}