using System.Xml.Linq;
namespace HKX2
{
    // hkbHandIkControlsModifier Signatire: 0x9f0488bb size: 96 flags: FLAGS_NONE

    // m_hands m_class: hkbHandIkControlsModifierHand Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkbHandIkControlsModifier : hkbModifier, IEquatable<hkbHandIkControlsModifier?>
    {
        public IList<hkbHandIkControlsModifierHand> m_hands { set; get; } = Array.Empty<hkbHandIkControlsModifierHand>();

        public override uint Signature { set; get; } = 0x9f0488bb;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_hands = des.ReadClassArray<hkbHandIkControlsModifierHand>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_hands);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_hands = xd.ReadClassArray<hkbHandIkControlsModifierHand>(xe, nameof(m_hands));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_hands), m_hands);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbHandIkControlsModifier);
        }

        public bool Equals(hkbHandIkControlsModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_hands.SequenceEqual(other.m_hands) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_hands.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

