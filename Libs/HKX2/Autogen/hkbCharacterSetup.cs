using System.Xml.Linq;
namespace HKX2
{
    // hkbCharacterSetup Signatire: 0xe5a2a413 size: 88 flags: FLAGS_NONE

    // m_retargetingSkeletonMappers m_class: hkaSkeletonMapper Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_animationSkeleton m_class: hkaSkeleton Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_ragdollToAnimationSkeletonMapper m_class: hkaSkeletonMapper Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_animationToRagdollSkeletonMapper m_class: hkaSkeletonMapper Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_animationBindingSet m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 56 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_data m_class: hkbCharacterData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_mirroredSkeleton m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 72 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_characterPropertyIdMap m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 80 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbCharacterSetup : hkReferencedObject, IEquatable<hkbCharacterSetup?>
    {
        public IList<hkaSkeletonMapper> m_retargetingSkeletonMappers { set; get; } = Array.Empty<hkaSkeletonMapper>();
        public hkaSkeleton? m_animationSkeleton { set; get; }
        public hkaSkeletonMapper? m_ragdollToAnimationSkeletonMapper { set; get; }
        public hkaSkeletonMapper? m_animationToRagdollSkeletonMapper { set; get; }
        private object? m_animationBindingSet { set; get; }
        public hkbCharacterData? m_data { set; get; }
        private object? m_mirroredSkeleton { set; get; }
        private object? m_characterPropertyIdMap { set; get; }

        public override uint Signature { set; get; } = 0xe5a2a413;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_retargetingSkeletonMappers = des.ReadClassPointerArray<hkaSkeletonMapper>(br);
            m_animationSkeleton = des.ReadClassPointer<hkaSkeleton>(br);
            m_ragdollToAnimationSkeletonMapper = des.ReadClassPointer<hkaSkeletonMapper>(br);
            m_animationToRagdollSkeletonMapper = des.ReadClassPointer<hkaSkeletonMapper>(br);
            des.ReadEmptyPointer(br);
            m_data = des.ReadClassPointer<hkbCharacterData>(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_retargetingSkeletonMappers);
            s.WriteClassPointer(bw, m_animationSkeleton);
            s.WriteClassPointer(bw, m_ragdollToAnimationSkeletonMapper);
            s.WriteClassPointer(bw, m_animationToRagdollSkeletonMapper);
            s.WriteVoidPointer(bw);
            s.WriteClassPointer(bw, m_data);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_retargetingSkeletonMappers = xd.ReadClassPointerArray<hkaSkeletonMapper>(xe, nameof(m_retargetingSkeletonMappers));
            m_animationSkeleton = xd.ReadClassPointer<hkaSkeleton>(xe, nameof(m_animationSkeleton));
            m_ragdollToAnimationSkeletonMapper = xd.ReadClassPointer<hkaSkeletonMapper>(xe, nameof(m_ragdollToAnimationSkeletonMapper));
            m_animationToRagdollSkeletonMapper = xd.ReadClassPointer<hkaSkeletonMapper>(xe, nameof(m_animationToRagdollSkeletonMapper));
            m_data = xd.ReadClassPointer<hkbCharacterData>(xe, nameof(m_data));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_retargetingSkeletonMappers), m_retargetingSkeletonMappers);
            xs.WriteClassPointer(xe, nameof(m_animationSkeleton), m_animationSkeleton);
            xs.WriteClassPointer(xe, nameof(m_ragdollToAnimationSkeletonMapper), m_ragdollToAnimationSkeletonMapper);
            xs.WriteClassPointer(xe, nameof(m_animationToRagdollSkeletonMapper), m_animationToRagdollSkeletonMapper);
            xs.WriteSerializeIgnored(xe, nameof(m_animationBindingSet));
            xs.WriteClassPointer(xe, nameof(m_data), m_data);
            xs.WriteSerializeIgnored(xe, nameof(m_mirroredSkeleton));
            xs.WriteSerializeIgnored(xe, nameof(m_characterPropertyIdMap));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCharacterSetup);
        }

        public bool Equals(hkbCharacterSetup? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_retargetingSkeletonMappers.SequenceEqual(other.m_retargetingSkeletonMappers) &&
                   ((m_animationSkeleton is null && other.m_animationSkeleton is null) || (m_animationSkeleton is not null && other.m_animationSkeleton is not null && m_animationSkeleton.Equals((IHavokObject)other.m_animationSkeleton))) &&
                   ((m_ragdollToAnimationSkeletonMapper is null && other.m_ragdollToAnimationSkeletonMapper is null) || (m_ragdollToAnimationSkeletonMapper is not null && other.m_ragdollToAnimationSkeletonMapper is not null && m_ragdollToAnimationSkeletonMapper.Equals((IHavokObject)other.m_ragdollToAnimationSkeletonMapper))) &&
                   ((m_animationToRagdollSkeletonMapper is null && other.m_animationToRagdollSkeletonMapper is null) || (m_animationToRagdollSkeletonMapper is not null && other.m_animationToRagdollSkeletonMapper is not null && m_animationToRagdollSkeletonMapper.Equals((IHavokObject)other.m_animationToRagdollSkeletonMapper))) &&
                   ((m_data is null && other.m_data is null) || (m_data is not null && other.m_data is not null && m_data.Equals((IHavokObject)other.m_data))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_retargetingSkeletonMappers.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_animationSkeleton);
            hashcode.Add(m_ragdollToAnimationSkeletonMapper);
            hashcode.Add(m_animationToRagdollSkeletonMapper);
            hashcode.Add(m_data);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

