using System.Xml.Linq;
namespace HKX2
{
    // hkpMoppCodeReindexedTerminal Signatire: 0x6ed8ac06 size: 8 flags: FLAGS_NONE

    // m_origShapeKey m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_reindexedShapeKey m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkpMoppCodeReindexedTerminal : IHavokObject, IEquatable<hkpMoppCodeReindexedTerminal?>
    {
        public uint m_origShapeKey { set; get; }
        public uint m_reindexedShapeKey { set; get; }

        public virtual uint Signature { set; get; } = 0x6ed8ac06;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_origShapeKey = br.ReadUInt32();
            m_reindexedShapeKey = br.ReadUInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_origShapeKey);
            bw.WriteUInt32(m_reindexedShapeKey);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_origShapeKey = xd.ReadUInt32(xe, nameof(m_origShapeKey));
            m_reindexedShapeKey = xd.ReadUInt32(xe, nameof(m_reindexedShapeKey));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_origShapeKey), m_origShapeKey);
            xs.WriteNumber(xe, nameof(m_reindexedShapeKey), m_reindexedShapeKey);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMoppCodeReindexedTerminal);
        }

        public bool Equals(hkpMoppCodeReindexedTerminal? other)
        {
            return other is not null &&
                   m_origShapeKey.Equals(other.m_origShapeKey) &&
                   m_reindexedShapeKey.Equals(other.m_reindexedShapeKey) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_origShapeKey);
            hashcode.Add(m_reindexedShapeKey);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

