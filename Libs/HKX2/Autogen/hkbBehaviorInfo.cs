using System.Xml.Linq;
namespace HKX2
{
    // hkbBehaviorInfo Signatire: 0xf7645395 size: 48 flags: FLAGS_NONE

    // m_characterId m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_data m_class: hkbBehaviorGraphData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_idToNamePairs m_class: hkbBehaviorInfoIdToNamePair Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkbBehaviorInfo : hkReferencedObject, IEquatable<hkbBehaviorInfo?>
    {
        public ulong m_characterId { set; get; }
        public hkbBehaviorGraphData? m_data { set; get; }
        public IList<hkbBehaviorInfoIdToNamePair> m_idToNamePairs { set; get; } = Array.Empty<hkbBehaviorInfoIdToNamePair>();

        public override uint Signature { set; get; } = 0xf7645395;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_data = des.ReadClassPointer<hkbBehaviorGraphData>(br);
            m_idToNamePairs = des.ReadClassArray<hkbBehaviorInfoIdToNamePair>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_characterId);
            s.WriteClassPointer(bw, m_data);
            s.WriteClassArray(bw, m_idToNamePairs);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_characterId = xd.ReadUInt64(xe, nameof(m_characterId));
            m_data = xd.ReadClassPointer<hkbBehaviorGraphData>(xe, nameof(m_data));
            m_idToNamePairs = xd.ReadClassArray<hkbBehaviorInfoIdToNamePair>(xe, nameof(m_idToNamePairs));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_characterId), m_characterId);
            xs.WriteClassPointer(xe, nameof(m_data), m_data);
            xs.WriteClassArray(xe, nameof(m_idToNamePairs), m_idToNamePairs);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBehaviorInfo);
        }

        public bool Equals(hkbBehaviorInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_characterId.Equals(other.m_characterId) &&
                   ((m_data is null && other.m_data is null) || (m_data is not null && other.m_data is not null && m_data.Equals((IHavokObject)other.m_data))) &&
                   m_idToNamePairs.SequenceEqual(other.m_idToNamePairs) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_characterId);
            hashcode.Add(m_data);
            hashcode.Add(m_idToNamePairs.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

