using System.Xml.Linq;
namespace HKX2
{
    // hkxAnimatedFloat Signatire: 0xce8b2fbd size: 40 flags: FLAGS_NONE

    // m_floats m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_hint m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: Hint
    public partial class hkxAnimatedFloat : hkReferencedObject, IEquatable<hkxAnimatedFloat?>
    {
        public IList<float> m_floats { set; get; } = Array.Empty<float>();
        public byte m_hint { set; get; }

        public override uint Signature { set; get; } = 0xce8b2fbd;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_floats = des.ReadSingleArray(br);
            m_hint = br.ReadByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteSingleArray(bw, m_floats);
            bw.WriteByte(m_hint);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_floats = xd.ReadSingleArray(xe, nameof(m_floats));
            m_hint = xd.ReadFlag<Hint, byte>(xe, nameof(m_hint));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloatArray(xe, nameof(m_floats), m_floats);
            xs.WriteEnum<Hint, byte>(xe, nameof(m_hint), m_hint);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxAnimatedFloat);
        }

        public bool Equals(hkxAnimatedFloat? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_floats.SequenceEqual(other.m_floats) &&
                   m_hint.Equals(other.m_hint) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_floats.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_hint);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

