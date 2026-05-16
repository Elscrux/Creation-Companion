using System.Xml.Linq;
namespace HKX2
{
    // hkpStiffSpringConstraintAtom Signatire: 0x6c128096 size: 8 flags: FLAGS_NONE

    // m_length m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkpStiffSpringConstraintAtom : hkpConstraintAtom, IEquatable<hkpStiffSpringConstraintAtom?>
    {
        public float m_length { set; get; }

        public override uint Signature { set; get; } = 0x6c128096;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 2;
            m_length = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 2;
            bw.WriteSingle(m_length);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_length = xd.ReadSingle(xe, nameof(m_length));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_length), m_length);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpStiffSpringConstraintAtom);
        }

        public bool Equals(hkpStiffSpringConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_length.Equals(other.m_length) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_length);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

