using System.Xml.Linq;
namespace HKX2
{
    // hkbManualSelectorGenerator Signatire: 0xd932fab8 size: 96 flags: FLAGS_NONE

    // m_generators m_class: hkbGenerator Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_selectedGeneratorIndex m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_currentGeneratorIndex m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 89 flags: FLAGS_NONE enum: 
    public partial class hkbManualSelectorGenerator : hkbGenerator, IEquatable<hkbManualSelectorGenerator?>
    {
        public IList<hkbGenerator> m_generators { set; get; } = Array.Empty<hkbGenerator>();
        public sbyte m_selectedGeneratorIndex { set; get; }
        public sbyte m_currentGeneratorIndex { set; get; }

        public override uint Signature { set; get; } = 0xd932fab8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_generators = des.ReadClassPointerArray<hkbGenerator>(br);
            m_selectedGeneratorIndex = br.ReadSByte();
            m_currentGeneratorIndex = br.ReadSByte();
            br.Position += 6;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_generators);
            bw.WriteSByte(m_selectedGeneratorIndex);
            bw.WriteSByte(m_currentGeneratorIndex);
            bw.Position += 6;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_generators = xd.ReadClassPointerArray<hkbGenerator>(xe, nameof(m_generators));
            m_selectedGeneratorIndex = xd.ReadSByte(xe, nameof(m_selectedGeneratorIndex));
            m_currentGeneratorIndex = xd.ReadSByte(xe, nameof(m_currentGeneratorIndex));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_generators), m_generators);
            xs.WriteNumber(xe, nameof(m_selectedGeneratorIndex), m_selectedGeneratorIndex);
            xs.WriteNumber(xe, nameof(m_currentGeneratorIndex), m_currentGeneratorIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbManualSelectorGenerator);
        }

        public bool Equals(hkbManualSelectorGenerator? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_generators.SequenceEqual(other.m_generators) &&
                   m_selectedGeneratorIndex.Equals(other.m_selectedGeneratorIndex) &&
                   m_currentGeneratorIndex.Equals(other.m_currentGeneratorIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_generators.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_selectedGeneratorIndex);
            hashcode.Add(m_currentGeneratorIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

