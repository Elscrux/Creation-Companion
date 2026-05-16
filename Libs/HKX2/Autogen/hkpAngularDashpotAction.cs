using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpAngularDashpotAction Signatire: 0x35f4c487 size: 96 flags: FLAGS_NONE

    // m_rotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_strength m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_damping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    public partial class hkpAngularDashpotAction : hkpBinaryAction, IEquatable<hkpAngularDashpotAction?>
    {
        public Quaternion m_rotation { set; get; }
        public float m_strength { set; get; }
        public float m_damping { set; get; }

        public override uint Signature { set; get; } = 0x35f4c487;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rotation = des.ReadQuaternion(br);
            m_strength = br.ReadSingle();
            m_damping = br.ReadSingle();
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteQuaternion(bw, m_rotation);
            bw.WriteSingle(m_strength);
            bw.WriteSingle(m_damping);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rotation = xd.ReadQuaternion(xe, nameof(m_rotation));
            m_strength = xd.ReadSingle(xe, nameof(m_strength));
            m_damping = xd.ReadSingle(xe, nameof(m_damping));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteQuaternion(xe, nameof(m_rotation), m_rotation);
            xs.WriteFloat(xe, nameof(m_strength), m_strength);
            xs.WriteFloat(xe, nameof(m_damping), m_damping);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpAngularDashpotAction);
        }

        public bool Equals(hkpAngularDashpotAction? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rotation.Equals(other.m_rotation) &&
                   m_strength.Equals(other.m_strength) &&
                   m_damping.Equals(other.m_damping) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rotation);
            hashcode.Add(m_strength);
            hashcode.Add(m_damping);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

