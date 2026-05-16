using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpTriSampledHeightFieldCollection Signatire: 0xc291ddde size: 96 flags: FLAGS_NONE

    // m_heightfield m_class: hkpSampledHeightFieldShape Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_childSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_radius m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_weldingInfo m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_triangleExtrusion m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkpTriSampledHeightFieldCollection : hkpShapeCollection, IEquatable<hkpTriSampledHeightFieldCollection?>
    {
        public hkpSampledHeightFieldShape? m_heightfield { set; get; }
        private int m_childSize { set; get; }
        public float m_radius { set; get; }
        public IList<ushort> m_weldingInfo { set; get; } = Array.Empty<ushort>();
        public Vector4 m_triangleExtrusion { set; get; }

        public override uint Signature { set; get; } = 0xc291ddde;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_heightfield = des.ReadClassPointer<hkpSampledHeightFieldShape>(br);
            m_childSize = br.ReadInt32();
            m_radius = br.ReadSingle();
            m_weldingInfo = des.ReadUInt16Array(br);
            m_triangleExtrusion = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_heightfield);
            bw.WriteInt32(m_childSize);
            bw.WriteSingle(m_radius);
            s.WriteUInt16Array(bw, m_weldingInfo);
            bw.WriteVector4(m_triangleExtrusion);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_heightfield = xd.ReadClassPointer<hkpSampledHeightFieldShape>(xe, nameof(m_heightfield));
            m_radius = xd.ReadSingle(xe, nameof(m_radius));
            m_weldingInfo = xd.ReadUInt16Array(xe, nameof(m_weldingInfo));
            m_triangleExtrusion = xd.ReadVector4(xe, nameof(m_triangleExtrusion));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_heightfield), m_heightfield);
            xs.WriteSerializeIgnored(xe, nameof(m_childSize));
            xs.WriteFloat(xe, nameof(m_radius), m_radius);
            xs.WriteNumberArray(xe, nameof(m_weldingInfo), m_weldingInfo);
            xs.WriteVector4(xe, nameof(m_triangleExtrusion), m_triangleExtrusion);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpTriSampledHeightFieldCollection);
        }

        public bool Equals(hkpTriSampledHeightFieldCollection? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_heightfield is null && other.m_heightfield is null) || (m_heightfield is not null && other.m_heightfield is not null && m_heightfield.Equals((IHavokObject)other.m_heightfield))) &&
                   m_radius.Equals(other.m_radius) &&
                   m_weldingInfo.SequenceEqual(other.m_weldingInfo) &&
                   m_triangleExtrusion.Equals(other.m_triangleExtrusion) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_heightfield);
            hashcode.Add(m_radius);
            hashcode.Add(m_weldingInfo.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_triangleExtrusion);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

