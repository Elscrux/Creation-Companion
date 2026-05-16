using System.Xml.Linq;
namespace HKX2
{
    // hkbVariableInfo Signatire: 0x9e746ba2 size: 6 flags: FLAGS_NONE

    // m_role m_class: hkbRoleAttribute Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 4 flags: FLAGS_NONE enum: VariableType
    public partial class hkbVariableInfo : IHavokObject, IEquatable<hkbVariableInfo?>
    {
        public hkbRoleAttribute m_role { set; get; } = new();
        public sbyte m_type { set; get; } = default;

        public virtual uint Signature { set; get; } = 0x9e746ba2;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_role = new hkbRoleAttribute();
            m_role.Read(des, br);
            m_type = br.ReadSByte();
            br.Position += 1;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_role.Write(s, bw);
            s.WriteSByte(bw, m_type);
            bw.Position += 1;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_role = xd.ReadClass<hkbRoleAttribute>(xe, nameof(m_role));
            // XXX: inconsistent type, it just work.
            m_type = (sbyte)xd.ReadFlag<VariableType, uint>(xe, nameof(m_type));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass(xe, nameof(m_role), m_role);
            xs.WriteEnum<VariableType, sbyte>(xe, nameof(m_type), m_type);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbVariableInfo);
        }

        public bool Equals(hkbVariableInfo? other)
        {
            return other is not null &&
                   ((m_role is null && other.m_role is null) || (m_role is not null && m_role.Equals((IHavokObject)other.m_role))) &&
                   m_type == other.m_type &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_role);
            hashcode.Add(m_type);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

