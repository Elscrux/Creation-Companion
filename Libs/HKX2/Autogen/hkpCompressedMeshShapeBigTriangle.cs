using System.Xml.Linq;
namespace HKX2
{
    // hkpCompressedMeshShapeBigTriangle Signatire: 0xcbfc95a4 size: 16 flags: FLAGS_NONE

    // m_a m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_b m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_c m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_material m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_weldingInfo m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_transformIndex m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 14 flags: FLAGS_NONE enum: 
    public partial class hkpCompressedMeshShapeBigTriangle : IHavokObject, IEquatable<hkpCompressedMeshShapeBigTriangle?>
    {
        public ushort m_a { set; get; }
        public ushort m_b { set; get; }
        public ushort m_c { set; get; }
        public uint m_material { set; get; }
        public ushort m_weldingInfo { set; get; }
        public ushort m_transformIndex { set; get; }

        public virtual uint Signature { set; get; } = 0xcbfc95a4;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_a = br.ReadUInt16();
            m_b = br.ReadUInt16();
            m_c = br.ReadUInt16();
            br.Position += 2;
            m_material = br.ReadUInt32();
            m_weldingInfo = br.ReadUInt16();
            m_transformIndex = br.ReadUInt16();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_a);
            bw.WriteUInt16(m_b);
            bw.WriteUInt16(m_c);
            bw.Position += 2;
            bw.WriteUInt32(m_material);
            bw.WriteUInt16(m_weldingInfo);
            bw.WriteUInt16(m_transformIndex);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_a = xd.ReadUInt16(xe, nameof(m_a));
            m_b = xd.ReadUInt16(xe, nameof(m_b));
            m_c = xd.ReadUInt16(xe, nameof(m_c));
            m_material = xd.ReadUInt32(xe, nameof(m_material));
            m_weldingInfo = xd.ReadUInt16(xe, nameof(m_weldingInfo));
            m_transformIndex = xd.ReadUInt16(xe, nameof(m_transformIndex));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_a), m_a);
            xs.WriteNumber(xe, nameof(m_b), m_b);
            xs.WriteNumber(xe, nameof(m_c), m_c);
            xs.WriteNumber(xe, nameof(m_material), m_material);
            xs.WriteNumber(xe, nameof(m_weldingInfo), m_weldingInfo);
            xs.WriteNumber(xe, nameof(m_transformIndex), m_transformIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCompressedMeshShapeBigTriangle);
        }

        public bool Equals(hkpCompressedMeshShapeBigTriangle? other)
        {
            return other is not null &&
                   m_a.Equals(other.m_a) &&
                   m_b.Equals(other.m_b) &&
                   m_c.Equals(other.m_c) &&
                   m_material.Equals(other.m_material) &&
                   m_weldingInfo.Equals(other.m_weldingInfo) &&
                   m_transformIndex.Equals(other.m_transformIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_a);
            hashcode.Add(m_b);
            hashcode.Add(m_c);
            hashcode.Add(m_material);
            hashcode.Add(m_weldingInfo);
            hashcode.Add(m_transformIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

