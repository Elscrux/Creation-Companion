using System.Xml.Linq;
namespace HKX2
{
    // hkp2dAngConstraintAtom Signatire: 0xdcdb8b8b size: 4 flags: FLAGS_NONE

    // m_freeRotationAxis m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    public partial class hkp2dAngConstraintAtom : hkpConstraintAtom, IEquatable<hkp2dAngConstraintAtom?>
    {
        public byte m_freeRotationAxis { set; get; }

        public override uint Signature { set; get; } = 0xdcdb8b8b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_freeRotationAxis = br.ReadByte();
            br.Position += 1;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_freeRotationAxis);
            bw.Position += 1;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_freeRotationAxis = xd.ReadByte(xe, nameof(m_freeRotationAxis));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_freeRotationAxis), m_freeRotationAxis);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkp2dAngConstraintAtom);
        }

        public bool Equals(hkp2dAngConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_freeRotationAxis.Equals(other.m_freeRotationAxis) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_freeRotationAxis);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

