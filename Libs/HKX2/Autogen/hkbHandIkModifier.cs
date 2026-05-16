using System.Xml.Linq;
namespace HKX2
{
    // hkbHandIkModifier Signatire: 0xef8bc2f7 size: 120 flags: FLAGS_NONE

    // m_hands m_class: hkbHandIkModifierHand Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_fadeInOutCurve m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 96 flags: FLAGS_NONE enum: BlendCurve
    // m_internalHandData m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 104 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbHandIkModifier : hkbModifier, IEquatable<hkbHandIkModifier?>
    {
        public IList<hkbHandIkModifierHand> m_hands { set; get; } = Array.Empty<hkbHandIkModifierHand>();
        public sbyte m_fadeInOutCurve { set; get; }
        public IList<object> m_internalHandData { set; get; } = Array.Empty<object>();

        public override uint Signature { set; get; } = 0xef8bc2f7;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_hands = des.ReadClassArray<hkbHandIkModifierHand>(br);
            m_fadeInOutCurve = br.ReadSByte();
            br.Position += 7;
            des.ReadEmptyArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_hands);
            bw.WriteSByte(m_fadeInOutCurve);
            bw.Position += 7;
            s.WriteVoidArray(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_hands = xd.ReadClassArray<hkbHandIkModifierHand>(xe, nameof(m_hands));
            m_fadeInOutCurve = xd.ReadFlag<BlendCurve, sbyte>(xe, nameof(m_fadeInOutCurve));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_hands), m_hands);
            xs.WriteEnum<BlendCurve, sbyte>(xe, nameof(m_fadeInOutCurve), m_fadeInOutCurve);
            xs.WriteSerializeIgnored(xe, nameof(m_internalHandData));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbHandIkModifier);
        }

        public bool Equals(hkbHandIkModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_hands.SequenceEqual(other.m_hands) &&
                   m_fadeInOutCurve.Equals(other.m_fadeInOutCurve) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_hands.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_fadeInOutCurve);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

