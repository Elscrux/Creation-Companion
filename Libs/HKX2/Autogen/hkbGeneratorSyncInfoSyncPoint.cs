using System.Xml.Linq;
namespace HKX2
{
    // hkbGeneratorSyncInfoSyncPoint Signatire: 0xb597cf92 size: 8 flags: FLAGS_NONE

    // m_id m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_time m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkbGeneratorSyncInfoSyncPoint : IHavokObject, IEquatable<hkbGeneratorSyncInfoSyncPoint?>
    {
        public int m_id { set; get; }
        public float m_time { set; get; }

        public virtual uint Signature { set; get; } = 0xb597cf92;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_id = br.ReadInt32();
            m_time = br.ReadSingle();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_id);
            bw.WriteSingle(m_time);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_id = xd.ReadInt32(xe, nameof(m_id));
            m_time = xd.ReadSingle(xe, nameof(m_time));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_id), m_id);
            xs.WriteFloat(xe, nameof(m_time), m_time);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbGeneratorSyncInfoSyncPoint);
        }

        public bool Equals(hkbGeneratorSyncInfoSyncPoint? other)
        {
            return other is not null &&
                   m_id.Equals(other.m_id) &&
                   m_time.Equals(other.m_time) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_id);
            hashcode.Add(m_time);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

