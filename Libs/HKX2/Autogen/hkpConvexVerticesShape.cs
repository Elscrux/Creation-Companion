using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpConvexVerticesShape Signatire: 0x28726ad8 size: 144 flags: FLAGS_NONE

    // m_aabbHalfExtents m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_aabbCenter m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_rotatedVertices m_class: hkpConvexVerticesShapeFourVectors Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_numVertices m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_externalObject m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 104 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_getFaceNormals m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_planeEquations m_class:  Type.TYPE_ARRAY Type.TYPE_VECTOR4 arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_connectivity m_class: hkpConvexVerticesConnectivity Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 136 flags: FLAGS_NONE enum: 
    public partial class hkpConvexVerticesShape : hkpConvexShape, IEquatable<hkpConvexVerticesShape?>
    {
        public Vector4 m_aabbHalfExtents { set; get; }
        public Vector4 m_aabbCenter { set; get; }
        public IList<hkpConvexVerticesShapeFourVectors> m_rotatedVertices { set; get; } = Array.Empty<hkpConvexVerticesShapeFourVectors>();
        public int m_numVertices { set; get; }
        private object? m_externalObject { set; get; }
        private object? m_getFaceNormals { set; get; }
        public IList<Vector4> m_planeEquations { set; get; } = Array.Empty<Vector4>();
        public hkpConvexVerticesConnectivity? m_connectivity { set; get; }

        public override uint Signature { set; get; } = 0x28726ad8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_aabbHalfExtents = br.ReadVector4();
            m_aabbCenter = br.ReadVector4();
            m_rotatedVertices = des.ReadClassArray<hkpConvexVerticesShapeFourVectors>(br);
            m_numVertices = br.ReadInt32();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            m_planeEquations = des.ReadVector4Array(br);
            m_connectivity = des.ReadClassPointer<hkpConvexVerticesConnectivity>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            bw.WriteVector4(m_aabbHalfExtents);
            bw.WriteVector4(m_aabbCenter);
            s.WriteClassArray(bw, m_rotatedVertices);
            bw.WriteInt32(m_numVertices);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVector4Array(bw, m_planeEquations);
            s.WriteClassPointer(bw, m_connectivity);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_aabbHalfExtents = xd.ReadVector4(xe, nameof(m_aabbHalfExtents));
            m_aabbCenter = xd.ReadVector4(xe, nameof(m_aabbCenter));
            m_rotatedVertices = xd.ReadClassArray<hkpConvexVerticesShapeFourVectors>(xe, nameof(m_rotatedVertices));
            m_numVertices = xd.ReadInt32(xe, nameof(m_numVertices));
            m_planeEquations = xd.ReadVector4Array(xe, nameof(m_planeEquations));
            m_connectivity = xd.ReadClassPointer<hkpConvexVerticesConnectivity>(xe, nameof(m_connectivity));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_aabbHalfExtents), m_aabbHalfExtents);
            xs.WriteVector4(xe, nameof(m_aabbCenter), m_aabbCenter);
            xs.WriteClassArray(xe, nameof(m_rotatedVertices), m_rotatedVertices);
            xs.WriteNumber(xe, nameof(m_numVertices), m_numVertices);
            xs.WriteSerializeIgnored(xe, nameof(m_externalObject));
            xs.WriteSerializeIgnored(xe, nameof(m_getFaceNormals));
            xs.WriteVector4Array(xe, nameof(m_planeEquations), m_planeEquations);
            xs.WriteClassPointer(xe, nameof(m_connectivity), m_connectivity);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConvexVerticesShape);
        }

        public bool Equals(hkpConvexVerticesShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_aabbHalfExtents.Equals(other.m_aabbHalfExtents) &&
                   m_aabbCenter.Equals(other.m_aabbCenter) &&
                   m_rotatedVertices.SequenceEqual(other.m_rotatedVertices) &&
                   m_numVertices.Equals(other.m_numVertices) &&
                   m_planeEquations.SequenceEqual(other.m_planeEquations) &&
                   ((m_connectivity is null && other.m_connectivity is null) || (m_connectivity is not null && other.m_connectivity is not null && m_connectivity.Equals((IHavokObject)other.m_connectivity))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_aabbHalfExtents);
            hashcode.Add(m_aabbCenter);
            hashcode.Add(m_rotatedVertices.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_numVertices);
            hashcode.Add(m_planeEquations.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_connectivity);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

