using System.Xml.Linq;
namespace HKX2
{
    // hkpPointToPlaneConstraintDataAtoms Signatire: 0x749bc260 size: 160 flags: FLAGS_NONE

    // m_transforms m_class: hkpSetLocalTransformsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_lin m_class: hkpLinConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    public partial class hkpPointToPlaneConstraintDataAtoms : IHavokObject, IEquatable<hkpPointToPlaneConstraintDataAtoms?>
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms { set; get; } = new();
        public hkpLinConstraintAtom m_lin { set; get; } = new();

        public virtual uint Signature { set; get; } = 0x749bc260;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms.Read(des, br);
            m_lin.Read(des, br);
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_transforms.Write(s, bw);
            m_lin.Write(s, bw);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_transforms = xd.ReadClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms));
            m_lin = xd.ReadClass<hkpLinConstraintAtom>(xe, nameof(m_lin));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms), m_transforms);
            xs.WriteClass(xe, nameof(m_lin), m_lin);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPointToPlaneConstraintDataAtoms);
        }

        public bool Equals(hkpPointToPlaneConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_transforms is null && other.m_transforms is null) || (m_transforms is not null && other.m_transforms is not null && m_transforms.Equals((IHavokObject)other.m_transforms))) &&
                   ((m_lin is null && other.m_lin is null) || (m_lin is not null && other.m_lin is not null && m_lin.Equals((IHavokObject)other.m_lin))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_transforms);
            hashcode.Add(m_lin);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

