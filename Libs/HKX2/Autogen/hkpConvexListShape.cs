using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpConvexListShape Signatire: 0x450b26e8 size: 128 flags: FLAGS_NONE

    // m_minDistanceToUseConvexHullForGetClosestPoints m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_aabbHalfExtents m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_aabbCenter m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_useCachedAabb m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_childShapes m_class: hkpConvexShape Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    public partial class hkpConvexListShape : hkpConvexShape, IEquatable<hkpConvexListShape?>
    {
        public float m_minDistanceToUseConvexHullForGetClosestPoints { set; get; }
        public Vector4 m_aabbHalfExtents { set; get; }
        public Vector4 m_aabbCenter { set; get; }
        public bool m_useCachedAabb { set; get; }
        public IList<hkpConvexShape> m_childShapes { set; get; } = Array.Empty<hkpConvexShape>();

        public override uint Signature { set; get; } = 0x450b26e8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_minDistanceToUseConvexHullForGetClosestPoints = br.ReadSingle();
            br.Position += 12;
            m_aabbHalfExtents = br.ReadVector4();
            m_aabbCenter = br.ReadVector4();
            m_useCachedAabb = br.ReadBoolean();
            br.Position += 7;
            m_childShapes = des.ReadClassPointerArray<hkpConvexShape>(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            bw.WriteSingle(m_minDistanceToUseConvexHullForGetClosestPoints);
            bw.Position += 12;
            bw.WriteVector4(m_aabbHalfExtents);
            bw.WriteVector4(m_aabbCenter);
            bw.WriteBoolean(m_useCachedAabb);
            bw.Position += 7;
            s.WriteClassPointerArray(bw, m_childShapes);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_minDistanceToUseConvexHullForGetClosestPoints = xd.ReadSingle(xe, nameof(m_minDistanceToUseConvexHullForGetClosestPoints));
            m_aabbHalfExtents = xd.ReadVector4(xe, nameof(m_aabbHalfExtents));
            m_aabbCenter = xd.ReadVector4(xe, nameof(m_aabbCenter));
            m_useCachedAabb = xd.ReadBoolean(xe, nameof(m_useCachedAabb));
            m_childShapes = xd.ReadClassPointerArray<hkpConvexShape>(xe, nameof(m_childShapes));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_minDistanceToUseConvexHullForGetClosestPoints), m_minDistanceToUseConvexHullForGetClosestPoints);
            xs.WriteVector4(xe, nameof(m_aabbHalfExtents), m_aabbHalfExtents);
            xs.WriteVector4(xe, nameof(m_aabbCenter), m_aabbCenter);
            xs.WriteBoolean(xe, nameof(m_useCachedAabb), m_useCachedAabb);
            xs.WriteClassPointerArray(xe, nameof(m_childShapes), m_childShapes);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConvexListShape);
        }

        public bool Equals(hkpConvexListShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_minDistanceToUseConvexHullForGetClosestPoints.Equals(other.m_minDistanceToUseConvexHullForGetClosestPoints) &&
                   m_aabbHalfExtents.Equals(other.m_aabbHalfExtents) &&
                   m_aabbCenter.Equals(other.m_aabbCenter) &&
                   m_useCachedAabb.Equals(other.m_useCachedAabb) &&
                   m_childShapes.SequenceEqual(other.m_childShapes) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_minDistanceToUseConvexHullForGetClosestPoints);
            hashcode.Add(m_aabbHalfExtents);
            hashcode.Add(m_aabbCenter);
            hashcode.Add(m_useCachedAabb);
            hashcode.Add(m_childShapes.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

