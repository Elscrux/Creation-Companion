using System.Xml.Linq;
namespace HKX2
{
    // hkpSetupStabilizationAtom Signatire: 0xf05d137e size: 16 flags: FLAGS_NONE

    // m_enabled m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_maxAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_padding m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 8 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkpSetupStabilizationAtom : hkpConstraintAtom, IEquatable<hkpSetupStabilizationAtom?>
    {
        public bool m_enabled { set; get; }
        public float m_maxAngle { set; get; }
        public byte[] m_padding = new byte[8];

        public override uint Signature { set; get; } = 0xf05d137e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_enabled = br.ReadBoolean();
            br.Position += 1;
            m_maxAngle = br.ReadSingle();
            m_padding = des.ReadByteCStyleArray(br, 8);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_enabled);
            bw.Position += 1;
            bw.WriteSingle(m_maxAngle);
            s.WriteByteCStyleArray(bw, m_padding);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_enabled = xd.ReadBoolean(xe, nameof(m_enabled));
            m_maxAngle = xd.ReadSingle(xe, nameof(m_maxAngle));
            m_padding = xd.ReadByteCStyleArray(xe, nameof(m_padding), 8);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_enabled), m_enabled);
            xs.WriteFloat(xe, nameof(m_maxAngle), m_maxAngle);
            xs.WriteNumberArray(xe, nameof(m_padding), m_padding);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSetupStabilizationAtom);
        }

        public bool Equals(hkpSetupStabilizationAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_enabled.Equals(other.m_enabled) &&
                   m_maxAngle.Equals(other.m_maxAngle) &&
                   m_padding.SequenceEqual(other.m_padding) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_enabled);
            hashcode.Add(m_maxAngle);
            hashcode.Add(m_padding.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

