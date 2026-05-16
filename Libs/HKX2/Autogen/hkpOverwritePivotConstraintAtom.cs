using System.Xml.Linq;
namespace HKX2
{
    // hkpOverwritePivotConstraintAtom Signatire: 0x1f11b467 size: 4 flags: FLAGS_NONE

    // m_copyToPivotBFromPivotA m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    public partial class hkpOverwritePivotConstraintAtom : hkpConstraintAtom, IEquatable<hkpOverwritePivotConstraintAtom?>
    {
        public byte m_copyToPivotBFromPivotA { set; get; }

        public override uint Signature { set; get; } = 0x1f11b467;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_copyToPivotBFromPivotA = br.ReadByte();
            br.Position += 1;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_copyToPivotBFromPivotA);
            bw.Position += 1;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_copyToPivotBFromPivotA = xd.ReadByte(xe, nameof(m_copyToPivotBFromPivotA));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_copyToPivotBFromPivotA), m_copyToPivotBFromPivotA);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpOverwritePivotConstraintAtom);
        }

        public bool Equals(hkpOverwritePivotConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_copyToPivotBFromPivotA.Equals(other.m_copyToPivotBFromPivotA) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_copyToPivotBFromPivotA);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

