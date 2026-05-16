using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMeshShape Signatire: 0x3bf12c0f size: 128 flags: FLAGS_NONE

    // m_scaling m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_numBitsForSubpartIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_subparts m_class: hkpMeshShapeSubpart Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_weldingInfo m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_weldingType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 104 flags: FLAGS_NONE enum: WeldingType
    // m_radius m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 108 flags: FLAGS_NONE enum: 
    // m_pad m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 3 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkpMeshShape : hkpShapeCollection, IEquatable<hkpMeshShape?>
    {
        public Vector4 m_scaling { set; get; }
        public int m_numBitsForSubpartIndex { set; get; }
        public IList<hkpMeshShapeSubpart> m_subparts { set; get; } = Array.Empty<hkpMeshShapeSubpart>();
        public IList<ushort> m_weldingInfo { set; get; } = Array.Empty<ushort>();
        public byte m_weldingType { set; get; }
        public float m_radius { set; get; }
        public int[] m_pad = new int[3];

        public override uint Signature { set; get; } = 0x3bf12c0f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_scaling = br.ReadVector4();
            m_numBitsForSubpartIndex = br.ReadInt32();
            br.Position += 4;
            m_subparts = des.ReadClassArray<hkpMeshShapeSubpart>(br);
            m_weldingInfo = des.ReadUInt16Array(br);
            m_weldingType = br.ReadByte();
            br.Position += 3;
            m_radius = br.ReadSingle();
            m_pad = des.ReadInt32CStyleArray(br, 3);
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_scaling);
            bw.WriteInt32(m_numBitsForSubpartIndex);
            bw.Position += 4;
            s.WriteClassArray(bw, m_subparts);
            s.WriteUInt16Array(bw, m_weldingInfo);
            bw.WriteByte(m_weldingType);
            bw.Position += 3;
            bw.WriteSingle(m_radius);
            s.WriteInt32CStyleArray(bw, m_pad);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_scaling = xd.ReadVector4(xe, nameof(m_scaling));
            m_numBitsForSubpartIndex = xd.ReadInt32(xe, nameof(m_numBitsForSubpartIndex));
            m_subparts = xd.ReadClassArray<hkpMeshShapeSubpart>(xe, nameof(m_subparts));
            m_weldingInfo = xd.ReadUInt16Array(xe, nameof(m_weldingInfo));
            m_weldingType = xd.ReadFlag<WeldingType, byte>(xe, nameof(m_weldingType));
            m_radius = xd.ReadSingle(xe, nameof(m_radius));
            m_pad = xd.ReadInt32CStyleArray(xe, nameof(m_pad), 3);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_scaling), m_scaling);
            xs.WriteNumber(xe, nameof(m_numBitsForSubpartIndex), m_numBitsForSubpartIndex);
            xs.WriteClassArray(xe, nameof(m_subparts), m_subparts);
            xs.WriteNumberArray(xe, nameof(m_weldingInfo), m_weldingInfo);
            xs.WriteEnum<WeldingType, byte>(xe, nameof(m_weldingType), m_weldingType);
            xs.WriteFloat(xe, nameof(m_radius), m_radius);
            xs.WriteNumberArray(xe, nameof(m_pad), m_pad);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMeshShape);
        }

        public bool Equals(hkpMeshShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_scaling.Equals(other.m_scaling) &&
                   m_numBitsForSubpartIndex.Equals(other.m_numBitsForSubpartIndex) &&
                   m_subparts.SequenceEqual(other.m_subparts) &&
                   m_weldingInfo.SequenceEqual(other.m_weldingInfo) &&
                   m_weldingType.Equals(other.m_weldingType) &&
                   m_radius.Equals(other.m_radius) &&
                   m_pad.SequenceEqual(other.m_pad) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_scaling);
            hashcode.Add(m_numBitsForSubpartIndex);
            hashcode.Add(m_subparts.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_weldingInfo.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_weldingType);
            hashcode.Add(m_radius);
            hashcode.Add(m_pad.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

