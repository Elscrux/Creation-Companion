using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMovingSurfaceModifierConstraintAtom Signatire: 0x79ab517d size: 64 flags: FLAGS_NONE

    // m_velocity m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpMovingSurfaceModifierConstraintAtom : hkpModifierConstraintAtom, IEquatable<hkpMovingSurfaceModifierConstraintAtom?>
    {
        public Vector4 m_velocity { set; get; }

        public override uint Signature { set; get; } = 0x79ab517d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_velocity = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_velocity);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_velocity = xd.ReadVector4(xe, nameof(m_velocity));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_velocity), m_velocity);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMovingSurfaceModifierConstraintAtom);
        }

        public bool Equals(hkpMovingSurfaceModifierConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_velocity.Equals(other.m_velocity) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_velocity);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

