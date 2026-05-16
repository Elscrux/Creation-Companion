using System.Xml.Linq;
namespace HKX2
{
    // hkpBridgeAtoms Signatire: 0xde152a4d size: 24 flags: FLAGS_NONE

    // m_bridgeAtom m_class: hkpBridgeConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkpBridgeAtoms : IHavokObject, IEquatable<hkpBridgeAtoms?>
    {
        public hkpBridgeConstraintAtom m_bridgeAtom { set; get; } = new();

        public virtual uint Signature { set; get; } = 0xde152a4d;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_bridgeAtom.Read(des, br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_bridgeAtom.Write(s, bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_bridgeAtom = xd.ReadClass<hkpBridgeConstraintAtom>(xe, nameof(m_bridgeAtom));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpBridgeConstraintAtom>(xe, nameof(m_bridgeAtom), m_bridgeAtom);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpBridgeAtoms);
        }

        public bool Equals(hkpBridgeAtoms? other)
        {
            return other is not null &&
                   ((m_bridgeAtom is null && other.m_bridgeAtom is null) || (m_bridgeAtom is not null && other.m_bridgeAtom is not null && m_bridgeAtom.Equals((IHavokObject)other.m_bridgeAtom))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_bridgeAtom);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

