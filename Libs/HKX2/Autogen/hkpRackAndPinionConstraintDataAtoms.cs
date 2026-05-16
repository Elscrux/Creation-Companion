using System.Xml.Linq;
namespace HKX2
{
    // hkpRackAndPinionConstraintDataAtoms Signatire: 0xa58a9659 size: 160 flags: FLAGS_NONE

    // m_transforms m_class: hkpSetLocalTransformsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_rackAndPinion m_class: hkpRackAndPinionConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    public partial class hkpRackAndPinionConstraintDataAtoms : IHavokObject, IEquatable<hkpRackAndPinionConstraintDataAtoms?>
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms { set; get; } = new();
        public hkpRackAndPinionConstraintAtom m_rackAndPinion { set; get; } = new();

        public virtual uint Signature { set; get; } = 0xa58a9659;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms.Read(des, br);
            m_rackAndPinion.Read(des, br);
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_transforms.Write(s, bw);
            m_rackAndPinion.Write(s, bw);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_transforms = xd.ReadClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms));
            m_rackAndPinion = xd.ReadClass<hkpRackAndPinionConstraintAtom>(xe, nameof(m_rackAndPinion));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms), m_transforms);
            xs.WriteClass(xe, nameof(m_rackAndPinion), m_rackAndPinion);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpRackAndPinionConstraintDataAtoms);
        }

        public bool Equals(hkpRackAndPinionConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_transforms is null && other.m_transforms is null) || (m_transforms is not null && other.m_transforms is not null && m_transforms.Equals((IHavokObject)other.m_transforms))) &&
                   ((m_rackAndPinion is null && other.m_rackAndPinion is null) || (m_rackAndPinion is not null && other.m_rackAndPinion is not null && m_rackAndPinion.Equals((IHavokObject)other.m_rackAndPinion))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_transforms);
            hashcode.Add(m_rackAndPinion);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

