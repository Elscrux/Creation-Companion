using System.Xml.Linq;
namespace HKX2
{
    // hkbHandIkDriverInfo Signatire: 0xc299090a size: 40 flags: FLAGS_NONE

    // m_hands m_class: hkbHandIkDriverInfoHand Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_fadeInOutCurve m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: BlendCurve
    public partial class hkbHandIkDriverInfo : hkReferencedObject, IEquatable<hkbHandIkDriverInfo?>
    {
        public IList<hkbHandIkDriverInfoHand> m_hands { set; get; } = Array.Empty<hkbHandIkDriverInfoHand>();
        public sbyte m_fadeInOutCurve { set; get; }

        public override uint Signature { set; get; } = 0xc299090a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_hands = des.ReadClassArray<hkbHandIkDriverInfoHand>(br);
            m_fadeInOutCurve = br.ReadSByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_hands);
            bw.WriteSByte(m_fadeInOutCurve);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_hands = xd.ReadClassArray<hkbHandIkDriverInfoHand>(xe, nameof(m_hands));
            m_fadeInOutCurve = xd.ReadFlag<BlendCurve, sbyte>(xe, nameof(m_fadeInOutCurve));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_hands), m_hands);
            xs.WriteEnum<BlendCurve, sbyte>(xe, nameof(m_fadeInOutCurve), m_fadeInOutCurve);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbHandIkDriverInfo);
        }

        public bool Equals(hkbHandIkDriverInfo? other)
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

