using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpLinearParametricCurve Signatire: 0xd7b3be03 size: 80 flags: FLAGS_NONE

    // m_smoothingFactor m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_closedLoop m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_dirNotParallelToTangentAlongWholePath m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_points m_class:  Type.TYPE_ARRAY Type.TYPE_VECTOR4 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_distance m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpLinearParametricCurve : hkpParametricCurve, IEquatable<hkpLinearParametricCurve?>
    {
        public float m_smoothingFactor { set; get; }
        public bool m_closedLoop { set; get; }
        public Vector4 m_dirNotParallelToTangentAlongWholePath { set; get; }
        public IList<Vector4> m_points { set; get; } = Array.Empty<Vector4>();
        public IList<float> m_distance { set; get; } = Array.Empty<float>();

        public override uint Signature { set; get; } = 0xd7b3be03;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_smoothingFactor = br.ReadSingle();
            m_closedLoop = br.ReadBoolean();
            br.Position += 11;
            m_dirNotParallelToTangentAlongWholePath = br.ReadVector4();
            m_points = des.ReadVector4Array(br);
            m_distance = des.ReadSingleArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_smoothingFactor);
            bw.WriteBoolean(m_closedLoop);
            bw.Position += 11;
            bw.WriteVector4(m_dirNotParallelToTangentAlongWholePath);
            s.WriteVector4Array(bw, m_points);
            s.WriteSingleArray(bw, m_distance);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_smoothingFactor = xd.ReadSingle(xe, nameof(m_smoothingFactor));
            m_closedLoop = xd.ReadBoolean(xe, nameof(m_closedLoop));
            m_dirNotParallelToTangentAlongWholePath = xd.ReadVector4(xe, nameof(m_dirNotParallelToTangentAlongWholePath));
            m_points = xd.ReadVector4Array(xe, nameof(m_points));
            m_distance = xd.ReadSingleArray(xe, nameof(m_distance));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_smoothingFactor), m_smoothingFactor);
            xs.WriteBoolean(xe, nameof(m_closedLoop), m_closedLoop);
            xs.WriteVector4(xe, nameof(m_dirNotParallelToTangentAlongWholePath), m_dirNotParallelToTangentAlongWholePath);
            xs.WriteVector4Array(xe, nameof(m_points), m_points);
            xs.WriteFloatArray(xe, nameof(m_distance), m_distance);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpLinearParametricCurve);
        }

        public bool Equals(hkpLinearParametricCurve? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_smoothingFactor.Equals(other.m_smoothingFactor) &&
                   m_closedLoop.Equals(other.m_closedLoop) &&
                   m_dirNotParallelToTangentAlongWholePath.Equals(other.m_dirNotParallelToTangentAlongWholePath) &&
                   m_points.SequenceEqual(other.m_points) &&
                   m_distance.SequenceEqual(other.m_distance) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_smoothingFactor);
            hashcode.Add(m_closedLoop);
            hashcode.Add(m_dirNotParallelToTangentAlongWholePath);
            hashcode.Add(m_points.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_distance.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

