using System.Xml.Linq;
namespace HKX2
{
    // hkpPoweredChainMapperTarget Signatire: 0xf651c74d size: 16 flags: FLAGS_NONE

    // m_chain m_class: hkpPoweredChainData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_infoIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkpPoweredChainMapperTarget : IHavokObject, IEquatable<hkpPoweredChainMapperTarget?>
    {
        public hkpPoweredChainData? m_chain { set; get; }
        public int m_infoIndex { set; get; }

        public virtual uint Signature { set; get; } = 0xf651c74d;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_chain = des.ReadClassPointer<hkpPoweredChainData>(br);
            m_infoIndex = br.ReadInt32();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer(bw, m_chain);
            bw.WriteInt32(m_infoIndex);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_chain = xd.ReadClassPointer<hkpPoweredChainData>(xe, nameof(m_chain));
            m_infoIndex = xd.ReadInt32(xe, nameof(m_infoIndex));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassPointer(xe, nameof(m_chain), m_chain);
            xs.WriteNumber(xe, nameof(m_infoIndex), m_infoIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPoweredChainMapperTarget);
        }

        public bool Equals(hkpPoweredChainMapperTarget? other)
        {
            return other is not null &&
                   ((m_chain is null && other.m_chain is null) || (m_chain is not null && other.m_chain is not null && m_chain.Equals((IHavokObject)other.m_chain))) &&
                   m_infoIndex.Equals(other.m_infoIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_chain);
            hashcode.Add(m_infoIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

