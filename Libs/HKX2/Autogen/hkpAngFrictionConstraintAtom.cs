using System.Xml.Linq;
namespace HKX2
{
    // hkpAngFrictionConstraintAtom Signatire: 0xf313aa80 size: 12 flags: FLAGS_NONE

    // m_isEnabled m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_firstFrictionAxis m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 3 flags: FLAGS_NONE enum: 
    // m_numFrictionAxes m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_maxFrictionTorque m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkpAngFrictionConstraintAtom : hkpConstraintAtom, IEquatable<hkpAngFrictionConstraintAtom?>
    {
        public byte m_isEnabled { set; get; }
        public byte m_firstFrictionAxis { set; get; }
        public byte m_numFrictionAxes { set; get; }
        public float m_maxFrictionTorque { set; get; }

        public override uint Signature { set; get; } = 0xf313aa80;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_firstFrictionAxis = br.ReadByte();
            m_numFrictionAxes = br.ReadByte();
            br.Position += 3;
            m_maxFrictionTorque = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_firstFrictionAxis);
            bw.WriteByte(m_numFrictionAxes);
            bw.Position += 3;
            bw.WriteSingle(m_maxFrictionTorque);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isEnabled = xd.ReadByte(xe, nameof(m_isEnabled));
            m_firstFrictionAxis = xd.ReadByte(xe, nameof(m_firstFrictionAxis));
            m_numFrictionAxes = xd.ReadByte(xe, nameof(m_numFrictionAxes));
            m_maxFrictionTorque = xd.ReadSingle(xe, nameof(m_maxFrictionTorque));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_isEnabled), m_isEnabled);
            xs.WriteNumber(xe, nameof(m_firstFrictionAxis), m_firstFrictionAxis);
            xs.WriteNumber(xe, nameof(m_numFrictionAxes), m_numFrictionAxes);
            xs.WriteFloat(xe, nameof(m_maxFrictionTorque), m_maxFrictionTorque);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpAngFrictionConstraintAtom);
        }

        public bool Equals(hkpAngFrictionConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isEnabled.Equals(other.m_isEnabled) &&
                   m_firstFrictionAxis.Equals(other.m_firstFrictionAxis) &&
                   m_numFrictionAxes.Equals(other.m_numFrictionAxes) &&
                   m_maxFrictionTorque.Equals(other.m_maxFrictionTorque) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_isEnabled);
            hashcode.Add(m_firstFrictionAxis);
            hashcode.Add(m_numFrictionAxes);
            hashcode.Add(m_maxFrictionTorque);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

