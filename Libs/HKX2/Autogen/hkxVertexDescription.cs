using System.Xml.Linq;
namespace HKX2
{
    // hkxVertexDescription Signatire: 0x2df6313d size: 16 flags: FLAGS_NONE

    // m_decls m_class: hkxVertexDescriptionElementDecl Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkxVertexDescription : IHavokObject, IEquatable<hkxVertexDescription?>
    {
        public IList<hkxVertexDescriptionElementDecl> m_decls { set; get; } = Array.Empty<hkxVertexDescriptionElementDecl>();

        public virtual uint Signature { set; get; } = 0x2df6313d;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_decls = des.ReadClassArray<hkxVertexDescriptionElementDecl>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassArray(bw, m_decls);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_decls = xd.ReadClassArray<hkxVertexDescriptionElementDecl>(xe, nameof(m_decls));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassArray(xe, nameof(m_decls), m_decls);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxVertexDescription);
        }

        public bool Equals(hkxVertexDescription? other)
        {
            return other is not null &&
                   m_decls.SequenceEqual(other.m_decls) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_decls.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

