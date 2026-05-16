using System.Xml.Linq;
namespace HKX2
{
    // hkpGenericConstraintData Signatire: 0xfa824640 size: 128 flags: FLAGS_NONE

    // m_atoms m_class: hkpBridgeAtoms Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_scheme m_class: hkpGenericConstraintDataScheme Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpGenericConstraintData : hkpConstraintData, IEquatable<hkpGenericConstraintData?>
    {
        public hkpBridgeAtoms m_atoms { set; get; } = new();
        public hkpGenericConstraintDataScheme m_scheme { set; get; } = new();

        public override uint Signature { set; get; } = 0xfa824640;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_atoms.Read(des, br);
            m_scheme.Read(des, br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_atoms.Write(s, bw);
            m_scheme.Write(s, bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_atoms = xd.ReadClass<hkpBridgeAtoms>(xe, nameof(m_atoms));
            m_scheme = xd.ReadClass<hkpGenericConstraintDataScheme>(xe, nameof(m_scheme));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpBridgeAtoms>(xe, nameof(m_atoms), m_atoms);
            xs.WriteClass(xe, nameof(m_scheme), m_scheme);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpGenericConstraintData);
        }

        public bool Equals(hkpGenericConstraintData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_atoms is null && other.m_atoms is null) || (m_atoms is not null && other.m_atoms is not null && m_atoms.Equals((IHavokObject)other.m_atoms))) &&
                   ((m_scheme is null && other.m_scheme is null) || (m_scheme is not null && other.m_scheme is not null && m_scheme.Equals((IHavokObject)other.m_scheme))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_atoms);
            hashcode.Add(m_scheme);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

