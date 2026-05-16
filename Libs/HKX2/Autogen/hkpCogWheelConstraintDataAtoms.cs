using System.Xml.Linq;
namespace HKX2
{
    // hkpCogWheelConstraintDataAtoms Signatire: 0xf855ba44 size: 160 flags: FLAGS_NONE

    // m_transforms m_class: hkpSetLocalTransformsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_cogWheels m_class: hkpCogWheelConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    public partial class hkpCogWheelConstraintDataAtoms : IHavokObject, IEquatable<hkpCogWheelConstraintDataAtoms?>
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms { set; get; } = new();
        public hkpCogWheelConstraintAtom m_cogWheels { set; get; } = new();

        public virtual uint Signature { set; get; } = 0xf855ba44;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms.Read(des, br);
            m_cogWheels.Read(des, br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_transforms.Write(s, bw);
            m_cogWheels.Write(s, bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_transforms = xd.ReadClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms));
            m_cogWheels = xd.ReadClass<hkpCogWheelConstraintAtom>(xe, nameof(m_cogWheels));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms), m_transforms);
            xs.WriteClass(xe, nameof(m_cogWheels), m_cogWheels);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCogWheelConstraintDataAtoms);
        }

        public bool Equals(hkpCogWheelConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_transforms is null && other.m_transforms is null) || (m_transforms is not null && other.m_transforms is not null && m_transforms.Equals((IHavokObject)other.m_transforms))) &&
                   ((m_cogWheels is null && other.m_cogWheels is null) || (m_cogWheels is not null && other.m_cogWheels is not null && m_cogWheels.Equals((IHavokObject)other.m_cogWheels))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_transforms);
            hashcode.Add(m_cogWheels);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

