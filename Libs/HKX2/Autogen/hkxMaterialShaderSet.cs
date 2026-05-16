using System.Xml.Linq;
namespace HKX2
{
    // hkxMaterialShaderSet Signatire: 0x154650f3 size: 32 flags: FLAGS_NONE

    // m_shaders m_class: hkxMaterialShader Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkxMaterialShaderSet : hkReferencedObject, IEquatable<hkxMaterialShaderSet?>
    {
        public IList<hkxMaterialShader> m_shaders { set; get; } = Array.Empty<hkxMaterialShader>();

        public override uint Signature { set; get; } = 0x154650f3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_shaders = des.ReadClassPointerArray<hkxMaterialShader>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_shaders);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_shaders = xd.ReadClassPointerArray<hkxMaterialShader>(xe, nameof(m_shaders));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_shaders), m_shaders);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxMaterialShaderSet);
        }

        public bool Equals(hkxMaterialShaderSet? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_shaders.SequenceEqual(other.m_shaders) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_shaders.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

