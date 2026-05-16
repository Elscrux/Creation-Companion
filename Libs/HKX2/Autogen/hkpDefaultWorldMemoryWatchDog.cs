using System.Xml.Linq;
namespace HKX2
{
    // hkpDefaultWorldMemoryWatchDog Signatire: 0x77d6b19f size: 24 flags: FLAGS_NONE

    // m_freeHeapMemoryRequested m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpDefaultWorldMemoryWatchDog : hkWorldMemoryAvailableWatchDog, IEquatable<hkpDefaultWorldMemoryWatchDog?>
    {
        public int m_freeHeapMemoryRequested { set; get; }

        public override uint Signature { set; get; } = 0x77d6b19f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_freeHeapMemoryRequested = br.ReadInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_freeHeapMemoryRequested);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_freeHeapMemoryRequested = xd.ReadInt32(xe, nameof(m_freeHeapMemoryRequested));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_freeHeapMemoryRequested), m_freeHeapMemoryRequested);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpDefaultWorldMemoryWatchDog);
        }

        public bool Equals(hkpDefaultWorldMemoryWatchDog? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_freeHeapMemoryRequested.Equals(other.m_freeHeapMemoryRequested) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_freeHeapMemoryRequested);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

