using System.Xml.Linq;
namespace HKX2
{
    // hkpAngConstraintAtom Signatire: 0x35bb3cd0 size: 4 flags: FLAGS_NONE

    // m_firstConstrainedAxis m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_numConstrainedAxes m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 3 flags: FLAGS_NONE enum: 
    public partial class hkpAngConstraintAtom : hkpConstraintAtom, IEquatable<hkpAngConstraintAtom?>
    {
        public byte m_firstConstrainedAxis { set; get; }
        public byte m_numConstrainedAxes { set; get; }

        public override uint Signature { set; get; } = 0x35bb3cd0;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_firstConstrainedAxis = br.ReadByte();
            m_numConstrainedAxes = br.ReadByte();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_firstConstrainedAxis);
            bw.WriteByte(m_numConstrainedAxes);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_firstConstrainedAxis = xd.ReadByte(xe, nameof(m_firstConstrainedAxis));
            m_numConstrainedAxes = xd.ReadByte(xe, nameof(m_numConstrainedAxes));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_firstConstrainedAxis), m_firstConstrainedAxis);
            xs.WriteNumber(xe, nameof(m_numConstrainedAxes), m_numConstrainedAxes);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpAngConstraintAtom);
        }

        public bool Equals(hkpAngConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_firstConstrainedAxis.Equals(other.m_firstConstrainedAxis) &&
                   m_numConstrainedAxes.Equals(other.m_numConstrainedAxes) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_firstConstrainedAxis);
            hashcode.Add(m_numConstrainedAxes);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

