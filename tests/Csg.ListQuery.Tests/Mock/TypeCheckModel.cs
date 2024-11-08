using System;

namespace Csg.ListQuery.Tests.Mock;

public class TypeCheckModel
{
    public byte Byte { get; set; }
    public short Int16 { get; set; }        
    public int Int32 { get; set; }
    public int? NullableInt32 { get; set; }
    public long Int64 { get; set; }
    public bool Bool { get; set; }
    public Guid Guid { get; set; }        
    public DateTime DateTime { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public string String { get; set; }
    public char Char { get; set; }
    public byte[] ByteArray { get; set; }
    public decimal Decimal { get; set; }
    public float Float { get; set; }
    public double Double { get; set; }        

    [Filterable]
    public Person Person { get; set;}
}