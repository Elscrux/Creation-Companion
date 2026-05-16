using System.Xml.Linq;
namespace HKX2
{
    // hkpLinLimitConstraintAtom Signatire: 0xa44d1b07 size: 12 flags: FLAGS_NONE

    // m_axisIndex m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_min m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_max m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkpLinLimitConstraintAtom : hkpConstraintAtom, IEquatable<hkpLinLimitConstraintAtom?>
    {
        public byte m_axisIndex { set; get; }
        public float m_min { set; get; }
        public float m_max { set; get; }

        public override uint Signature { set; get; } = 0xa44d1b07;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_axisIndex = br.ReadByte();
            br.Position += 1;
            m_min = br.ReadSingle();
            m_max = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_axisIndex);
            bw.Position += 1;
            bw.WriteSingle(m_min);
            bw.WriteSingle(m_max);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_axisIndex = xd.ReadByte(xe, nameof(m_axisIndex));
            m_min = xd.ReadSingle(xe, nameof(m_min));
            m_max = xd.ReadSingle(xe, nameof(m_max));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_axisIndex), m_axisIndex);
            xs.WriteFloat(xe, nameof(m_min), m_min);
            xs.WriteFloat(xe, nameof(m_max), m_max);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpLinLimitConstraintAtom);
        }

        public bool Equals(hkpLinLimitConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_axisIndex.Equals(other.m_axisIndex) &&
                   m_min.Equals(other.m_min) &&
                   m_max.Equals(other.m_max) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_axisIndex);
            hashcode.Add(m_min);
            hashcode.Add(m_max);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

