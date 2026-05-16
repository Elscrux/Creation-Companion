using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkaSkeletonMapperData Signatire: 0x95687ea0 size: 128 flags: FLAGS_NONE

    // m_skeletonA m_class: hkaSkeleton Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_skeletonB m_class: hkaSkeleton Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_simpleMappings m_class: hkaSkeletonMapperDataSimpleMapping Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_chainMappings m_class: hkaSkeletonMapperDataChainMapping Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_unmappedBones m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_extractedMotionMapping m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_keepUnmappedLocal m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_mappingType m_class:  Type.TYPE_ENUM Type.TYPE_INT32 arrSize: 0 offset: 116 flags: FLAGS_NONE enum: MappingType
    public partial class hkaSkeletonMapperData : IHavokObject, IEquatable<hkaSkeletonMapperData?>
    {
        public hkaSkeleton? m_skeletonA { set; get; }
        public hkaSkeleton? m_skeletonB { set; get; }
        public IList<hkaSkeletonMapperDataSimpleMapping> m_simpleMappings { set; get; } = Array.Empty<hkaSkeletonMapperDataSimpleMapping>();
        public IList<hkaSkeletonMapperDataChainMapping> m_chainMappings { set; get; } = Array.Empty<hkaSkeletonMapperDataChainMapping>();
        public IList<short> m_unmappedBones { set; get; } = Array.Empty<short>();
        public Matrix4x4 m_extractedMotionMapping { set; get; }
        public bool m_keepUnmappedLocal { set; get; }
        public int m_mappingType { set; get; }

        public virtual uint Signature { set; get; } = 0x95687ea0;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_skeletonA = des.ReadClassPointer<hkaSkeleton>(br);
            m_skeletonB = des.ReadClassPointer<hkaSkeleton>(br);
            m_simpleMappings = des.ReadClassArray<hkaSkeletonMapperDataSimpleMapping>(br);
            m_chainMappings = des.ReadClassArray<hkaSkeletonMapperDataChainMapping>(br);
            m_unmappedBones = des.ReadInt16Array(br);
            m_extractedMotionMapping = des.ReadQSTransform(br);
            m_keepUnmappedLocal = br.ReadBoolean();
            br.Position += 3;
            m_mappingType = br.ReadInt32();
            br.Position += 8;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer(bw, m_skeletonA);
            s.WriteClassPointer(bw, m_skeletonB);
            s.WriteClassArray(bw, m_simpleMappings);
            s.WriteClassArray(bw, m_chainMappings);
            s.WriteInt16Array(bw, m_unmappedBones);
            s.WriteQSTransform(bw, m_extractedMotionMapping);
            bw.WriteBoolean(m_keepUnmappedLocal);
            bw.Position += 3;
            bw.WriteInt32(m_mappingType);
            bw.Position += 8;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_skeletonA = xd.ReadClassPointer<hkaSkeleton>(xe, nameof(m_skeletonA));
            m_skeletonB = xd.ReadClassPointer<hkaSkeleton>(xe, nameof(m_skeletonB));
            m_simpleMappings = xd.ReadClassArray<hkaSkeletonMapperDataSimpleMapping>(xe, nameof(m_simpleMappings));
            m_chainMappings = xd.ReadClassArray<hkaSkeletonMapperDataChainMapping>(xe, nameof(m_chainMappings));
            m_unmappedBones = xd.ReadInt16Array(xe, nameof(m_unmappedBones));
            m_extractedMotionMapping = xd.ReadQSTransform(xe, nameof(m_extractedMotionMapping));
            m_keepUnmappedLocal = xd.ReadBoolean(xe, nameof(m_keepUnmappedLocal));
            m_mappingType = xd.ReadFlag<MappingType, int>(xe, nameof(m_mappingType));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassPointer(xe, nameof(m_skeletonA), m_skeletonA);
            xs.WriteClassPointer(xe, nameof(m_skeletonB), m_skeletonB);
            xs.WriteClassArray(xe, nameof(m_simpleMappings), m_simpleMappings);
            xs.WriteClassArray(xe, nameof(m_chainMappings), m_chainMappings);
            xs.WriteNumberArray(xe, nameof(m_unmappedBones), m_unmappedBones);
            xs.WriteQSTransform(xe, nameof(m_extractedMotionMapping), m_extractedMotionMapping);
            xs.WriteBoolean(xe, nameof(m_keepUnmappedLocal), m_keepUnmappedLocal);
            xs.WriteEnum<MappingType, int>(xe, nameof(m_mappingType), m_mappingType);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaSkeletonMapperData);
        }

        public bool Equals(hkaSkeletonMapperData? other)
        {
            return other is not null &&
                   ((m_skeletonA is null && other.m_skeletonA is null) || (m_skeletonA is not null && other.m_skeletonA is not null && m_skeletonA.Equals((IHavokObject)other.m_skeletonA))) &&
                   ((m_skeletonB is null && other.m_skeletonB is null) || (m_skeletonB is not null && other.m_skeletonB is not null && m_skeletonB.Equals((IHavokObject)other.m_skeletonB))) &&
                   m_simpleMappings.SequenceEqual(other.m_simpleMappings) &&
                   m_chainMappings.SequenceEqual(other.m_chainMappings) &&
                   m_unmappedBones.SequenceEqual(other.m_unmappedBones) &&
                   m_extractedMotionMapping.Equals(other.m_extractedMotionMapping) &&
                   m_keepUnmappedLocal.Equals(other.m_keepUnmappedLocal) &&
                   m_mappingType.Equals(other.m_mappingType) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_skeletonA);
            hashcode.Add(m_skeletonB);
            hashcode.Add(m_simpleMappings.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_chainMappings.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_unmappedBones.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_extractedMotionMapping);
            hashcode.Add(m_keepUnmappedLocal);
            hashcode.Add(m_mappingType);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

