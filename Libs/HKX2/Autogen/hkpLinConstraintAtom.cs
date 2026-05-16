using System.Xml.Linq;
namespace HKX2
{
    // hkpLinConstraintAtom Signatire: 0x7b6b0210 size: 4 flags: FLAGS_NONE

    // m_axisIndex m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    public partial class hkpLinConstraintAtom : hkpConstraintAtom, IEquatable<hkpLinConstraintAtom?>
    {
        public byte m_axisIndex { set; get; }

        public override uint Signature { set; get; } = 0x7b6b0210;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_axisIndex = br.ReadByte();
            br.Position += 1;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_axisIndex);
            bw.Position += 1;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_axisIndex = xd.ReadByte(xe, nameof(m_axisIndex));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_axisIndex), m_axisIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpLinConstraintAtom);
        }

        public bool Equals(hkpLinConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_axisIndex.Equals(other.m_axisIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_axisIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

