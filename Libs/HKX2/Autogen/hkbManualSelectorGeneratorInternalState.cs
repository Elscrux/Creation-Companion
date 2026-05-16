using System.Xml.Linq;
namespace HKX2
{
    // hkbManualSelectorGeneratorInternalState Signatire: 0x492c6137 size: 24 flags: FLAGS_NONE

    // m_currentGeneratorIndex m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbManualSelectorGeneratorInternalState : hkReferencedObject, IEquatable<hkbManualSelectorGeneratorInternalState?>
    {
        public sbyte m_currentGeneratorIndex { set; get; }

        public override uint Signature { set; get; } = 0x492c6137;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_currentGeneratorIndex = br.ReadSByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSByte(m_currentGeneratorIndex);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_currentGeneratorIndex = xd.ReadSByte(xe, nameof(m_currentGeneratorIndex));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_currentGeneratorIndex), m_currentGeneratorIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbManualSelectorGeneratorInternalState);
        }

        public bool Equals(hkbManualSelectorGeneratorInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_currentGeneratorIndex.Equals(other.m_currentGeneratorIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_currentGeneratorIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

