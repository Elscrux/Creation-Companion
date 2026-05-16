using System.Xml.Linq;
namespace HKX2
{
    // hkpLinSoftConstraintAtom Signatire: 0x52b27d69 size: 12 flags: FLAGS_NONE

    // m_axisIndex m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_tau m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_damping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkpLinSoftConstraintAtom : hkpConstraintAtom, IEquatable<hkpLinSoftConstraintAtom?>
    {
        public byte m_axisIndex { set; get; }
        public float m_tau { set; get; }
        public float m_damping { set; get; }

        public override uint Signature { set; get; } = 0x52b27d69;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_axisIndex = br.ReadByte();
            br.Position += 1;
            m_tau = br.ReadSingle();
            m_damping = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_axisIndex);
            bw.Position += 1;
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_damping);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_axisIndex = xd.ReadByte(xe, nameof(m_axisIndex));
            m_tau = xd.ReadSingle(xe, nameof(m_tau));
            m_damping = xd.ReadSingle(xe, nameof(m_damping));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_axisIndex), m_axisIndex);
            xs.WriteFloat(xe, nameof(m_tau), m_tau);
            xs.WriteFloat(xe, nameof(m_damping), m_damping);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpLinSoftConstraintAtom);
        }

        public bool Equals(hkpLinSoftConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_axisIndex.Equals(other.m_axisIndex) &&
                   m_tau.Equals(other.m_tau) &&
                   m_damping.Equals(other.m_damping) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_axisIndex);
            hashcode.Add(m_tau);
            hashcode.Add(m_damping);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

