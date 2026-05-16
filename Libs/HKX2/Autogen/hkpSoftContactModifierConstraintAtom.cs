using System.Xml.Linq;
namespace HKX2
{
    // hkpSoftContactModifierConstraintAtom Signatire: 0xecb34e27 size: 64 flags: FLAGS_NONE

    // m_tau m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_maxAcceleration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 52 flags: FLAGS_NONE enum: 
    public partial class hkpSoftContactModifierConstraintAtom : hkpModifierConstraintAtom, IEquatable<hkpSoftContactModifierConstraintAtom?>
    {
        public float m_tau { set; get; }
        public float m_maxAcceleration { set; get; }

        public override uint Signature { set; get; } = 0xecb34e27;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_tau = br.ReadSingle();
            m_maxAcceleration = br.ReadSingle();
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_maxAcceleration);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_tau = xd.ReadSingle(xe, nameof(m_tau));
            m_maxAcceleration = xd.ReadSingle(xe, nameof(m_maxAcceleration));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_tau), m_tau);
            xs.WriteFloat(xe, nameof(m_maxAcceleration), m_maxAcceleration);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSoftContactModifierConstraintAtom);
        }

        public bool Equals(hkpSoftContactModifierConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_tau.Equals(other.m_tau) &&
                   m_maxAcceleration.Equals(other.m_maxAcceleration) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_tau);
            hashcode.Add(m_maxAcceleration);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

