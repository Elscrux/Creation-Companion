using System.Xml.Linq;
namespace HKX2
{
    // hkpBridgeConstraintAtom Signatire: 0x87a4f31b size: 24 flags: FLAGS_NONE

    // m_buildJacobianFunc m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 8 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_constraintData m_class: hkpConstraintData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: NOT_OWNED|FLAGS_NONE enum: 
    public partial class hkpBridgeConstraintAtom : hkpConstraintAtom, IEquatable<hkpBridgeConstraintAtom?>
    {
        private object? m_buildJacobianFunc { set; get; }
        public hkpConstraintData? m_constraintData { set; get; }

        public override uint Signature { set; get; } = 0x87a4f31b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 6;
            des.ReadEmptyPointer(br);
            m_constraintData = des.ReadClassPointer<hkpConstraintData>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 6;
            s.WriteVoidPointer(bw);
            s.WriteClassPointer(bw, m_constraintData);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_constraintData = xd.ReadClassPointer<hkpConstraintData>(xe, nameof(m_constraintData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_buildJacobianFunc));
            xs.WriteClassPointer(xe, nameof(m_constraintData), m_constraintData);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpBridgeConstraintAtom);
        }

        public bool Equals(hkpBridgeConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_constraintData is null && other.m_constraintData is null) || (m_constraintData is not null && other.m_constraintData is not null && m_constraintData.Equals((IHavokObject)other.m_constraintData))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_constraintData);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

