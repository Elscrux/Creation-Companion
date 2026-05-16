using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpShapeInfo Signatire: 0xea7f1d08 size: 128 flags: FLAGS_NONE

    // m_shape m_class: hkpShape Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_isHierarchicalCompound m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_hkdShapesCollected m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 25 flags: FLAGS_NONE enum: 
    // m_childShapeNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_childTransforms m_class:  Type.TYPE_ARRAY Type.TYPE_TRANSFORM arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_transform m_class:  Type.TYPE_TRANSFORM Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpShapeInfo : hkReferencedObject, IEquatable<hkpShapeInfo?>
    {
        public hkpShape? m_shape { set; get; }
        public bool m_isHierarchicalCompound { set; get; }
        public bool m_hkdShapesCollected { set; get; }
        public IList<string> m_childShapeNames { set; get; } = Array.Empty<string>();
        public IList<Matrix4x4> m_childTransforms { set; get; } = Array.Empty<Matrix4x4>();
        public Matrix4x4 m_transform { set; get; }

        public override uint Signature { set; get; } = 0xea7f1d08;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_isHierarchicalCompound = br.ReadBoolean();
            m_hkdShapesCollected = br.ReadBoolean();
            br.Position += 6;
            m_childShapeNames = des.ReadStringPointerArray(br);
            m_childTransforms = des.ReadTransformArray(br);
            m_transform = des.ReadTransform(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_shape);
            bw.WriteBoolean(m_isHierarchicalCompound);
            bw.WriteBoolean(m_hkdShapesCollected);
            bw.Position += 6;
            s.WriteStringPointerArray(bw, m_childShapeNames);
            s.WriteTransformArray(bw, m_childTransforms);
            s.WriteTransform(bw, m_transform);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_shape = xd.ReadClassPointer<hkpShape>(xe, nameof(m_shape));
            m_isHierarchicalCompound = xd.ReadBoolean(xe, nameof(m_isHierarchicalCompound));
            m_hkdShapesCollected = xd.ReadBoolean(xe, nameof(m_hkdShapesCollected));
            m_childShapeNames = xd.ReadStringArray(xe, nameof(m_childShapeNames));
            m_childTransforms = xd.ReadTransformArray(xe, nameof(m_childTransforms));
            m_transform = xd.ReadTransform(xe, nameof(m_transform));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_shape), m_shape);
            xs.WriteBoolean(xe, nameof(m_isHierarchicalCompound), m_isHierarchicalCompound);
            xs.WriteBoolean(xe, nameof(m_hkdShapesCollected), m_hkdShapesCollected);
            xs.WriteStringArray(xe, nameof(m_childShapeNames), m_childShapeNames);
            xs.WriteTransformArray(xe, nameof(m_childTransforms), m_childTransforms);
            xs.WriteTransform(xe, nameof(m_transform), m_transform);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpShapeInfo);
        }

        public bool Equals(hkpShapeInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_shape is null && other.m_shape is null) || (m_shape is not null && other.m_shape is not null && m_shape.Equals((IHavokObject)other.m_shape))) &&
                   m_isHierarchicalCompound.Equals(other.m_isHierarchicalCompound) &&
                   m_hkdShapesCollected.Equals(other.m_hkdShapesCollected) &&
                   m_childShapeNames.SequenceEqual(other.m_childShapeNames) &&
                   m_childTransforms.SequenceEqual(other.m_childTransforms) &&
                   m_transform.Equals(other.m_transform) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_shape);
            hashcode.Add(m_isHierarchicalCompound);
            hashcode.Add(m_hkdShapesCollected);
            hashcode.Add(m_childShapeNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_childTransforms.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_transform);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

