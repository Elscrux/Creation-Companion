using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkxMaterial Signatire: 0x2954537a size: 176 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_stages m_class: hkxMaterialTextureStage Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_diffuseColor m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_ambientColor m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_specularColor m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_emissiveColor m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_subMaterials m_class: hkxMaterial Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_extraData m_class: hkReferencedObject Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_properties m_class: hkxMaterialProperty Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    public partial class hkxMaterial : hkxAttributeHolder, IEquatable<hkxMaterial?>
    {
        public string m_name { set; get; } = "";
        public IList<hkxMaterialTextureStage> m_stages { set; get; } = Array.Empty<hkxMaterialTextureStage>();
        public Vector4 m_diffuseColor { set; get; }
        public Vector4 m_ambientColor { set; get; }
        public Vector4 m_specularColor { set; get; }
        public Vector4 m_emissiveColor { set; get; }
        public IList<hkxMaterial> m_subMaterials { set; get; } = Array.Empty<hkxMaterial>();
        public hkReferencedObject? m_extraData { set; get; }
        public IList<hkxMaterialProperty> m_properties { set; get; } = Array.Empty<hkxMaterialProperty>();

        public override uint Signature { set; get; } = 0x2954537a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_stages = des.ReadClassArray<hkxMaterialTextureStage>(br);
            br.Position += 8;
            m_diffuseColor = br.ReadVector4();
            m_ambientColor = br.ReadVector4();
            m_specularColor = br.ReadVector4();
            m_emissiveColor = br.ReadVector4();
            m_subMaterials = des.ReadClassPointerArray<hkxMaterial>(br);
            m_extraData = des.ReadClassPointer<hkReferencedObject>(br);
            m_properties = des.ReadClassArray<hkxMaterialProperty>(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            s.WriteClassArray(bw, m_stages);
            bw.Position += 8;
            bw.WriteVector4(m_diffuseColor);
            bw.WriteVector4(m_ambientColor);
            bw.WriteVector4(m_specularColor);
            bw.WriteVector4(m_emissiveColor);
            s.WriteClassPointerArray(bw, m_subMaterials);
            s.WriteClassPointer(bw, m_extraData);
            s.WriteClassArray(bw, m_properties);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_name = xd.ReadString(xe, nameof(m_name));
            m_stages = xd.ReadClassArray<hkxMaterialTextureStage>(xe, nameof(m_stages));
            m_diffuseColor = xd.ReadVector4(xe, nameof(m_diffuseColor));
            m_ambientColor = xd.ReadVector4(xe, nameof(m_ambientColor));
            m_specularColor = xd.ReadVector4(xe, nameof(m_specularColor));
            m_emissiveColor = xd.ReadVector4(xe, nameof(m_emissiveColor));
            m_subMaterials = xd.ReadClassPointerArray<hkxMaterial>(xe, nameof(m_subMaterials));
            m_extraData = xd.ReadClassPointer<hkReferencedObject>(xe, nameof(m_extraData));
            m_properties = xd.ReadClassArray<hkxMaterialProperty>(xe, nameof(m_properties));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteClassArray(xe, nameof(m_stages), m_stages);
            xs.WriteVector4(xe, nameof(m_diffuseColor), m_diffuseColor);
            xs.WriteVector4(xe, nameof(m_ambientColor), m_ambientColor);
            xs.WriteVector4(xe, nameof(m_specularColor), m_specularColor);
            xs.WriteVector4(xe, nameof(m_emissiveColor), m_emissiveColor);
            xs.WriteClassPointerArray(xe, nameof(m_subMaterials), m_subMaterials);
            xs.WriteClassPointer(xe, nameof(m_extraData), m_extraData);
            xs.WriteClassArray(xe, nameof(m_properties), m_properties);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxMaterial);
        }

        public bool Equals(hkxMaterial? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_name == other.m_name &&
                   m_stages.SequenceEqual(other.m_stages) &&
                   m_diffuseColor.Equals(other.m_diffuseColor) &&
                   m_ambientColor.Equals(other.m_ambientColor) &&
                   m_specularColor.Equals(other.m_specularColor) &&
                   m_emissiveColor.Equals(other.m_emissiveColor) &&
                   m_subMaterials.SequenceEqual(other.m_subMaterials) &&
                   ((m_extraData is null && other.m_extraData is null) || (m_extraData is not null && other.m_extraData is not null && m_extraData.Equals((IHavokObject)other.m_extraData))) &&
                   m_properties.SequenceEqual(other.m_properties) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_name);
            hashcode.Add(m_stages.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_diffuseColor);
            hashcode.Add(m_ambientColor);
            hashcode.Add(m_specularColor);
            hashcode.Add(m_emissiveColor);
            hashcode.Add(m_subMaterials.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_extraData);
            hashcode.Add(m_properties.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

