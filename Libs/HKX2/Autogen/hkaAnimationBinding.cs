using System.Xml.Linq;
namespace HKX2
{
    // hkaAnimationBinding Signatire: 0x66eac971 size: 72 flags: FLAGS_NONE

    // m_originalSkeletonName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_animation m_class: hkaAnimation Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_transformTrackToBoneIndices m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_floatTrackToFloatSlotIndices m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_blendHint m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 64 flags: FLAGS_NONE enum: BlendHint
    public partial class hkaAnimationBinding : hkReferencedObject, IEquatable<hkaAnimationBinding?>
    {
        public string m_originalSkeletonName { set; get; } = "";
        public hkaAnimation? m_animation { set; get; }
        public IList<short> m_transformTrackToBoneIndices { set; get; } = Array.Empty<short>();
        public IList<short> m_floatTrackToFloatSlotIndices { set; get; } = Array.Empty<short>();
        public sbyte m_blendHint { set; get; }

        public override uint Signature { set; get; } = 0x66eac971;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_originalSkeletonName = des.ReadStringPointer(br);
            m_animation = des.ReadClassPointer<hkaAnimation>(br);
            m_transformTrackToBoneIndices = des.ReadInt16Array(br);
            m_floatTrackToFloatSlotIndices = des.ReadInt16Array(br);
            m_blendHint = br.ReadSByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_originalSkeletonName);
            s.WriteClassPointer(bw, m_animation);
            s.WriteInt16Array(bw, m_transformTrackToBoneIndices);
            s.WriteInt16Array(bw, m_floatTrackToFloatSlotIndices);
            bw.WriteSByte(m_blendHint);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_originalSkeletonName = xd.ReadString(xe, nameof(m_originalSkeletonName));
            m_animation = xd.ReadClassPointer<hkaAnimation>(xe, nameof(m_animation));
            m_transformTrackToBoneIndices = xd.ReadInt16Array(xe, nameof(m_transformTrackToBoneIndices));
            m_floatTrackToFloatSlotIndices = xd.ReadInt16Array(xe, nameof(m_floatTrackToFloatSlotIndices));
            m_blendHint = xd.ReadFlag<BlendHint, sbyte>(xe, nameof(m_blendHint));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_originalSkeletonName), m_originalSkeletonName);
            xs.WriteClassPointer(xe, nameof(m_animation), m_animation);
            xs.WriteNumberArray(xe, nameof(m_transformTrackToBoneIndices), m_transformTrackToBoneIndices);
            xs.WriteNumberArray(xe, nameof(m_floatTrackToFloatSlotIndices), m_floatTrackToFloatSlotIndices);
            xs.WriteEnum<BlendHint, sbyte>(xe, nameof(m_blendHint), m_blendHint);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaAnimationBinding);
        }

        public bool Equals(hkaAnimationBinding? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   (m_originalSkeletonName is null && other.m_originalSkeletonName is null || m_originalSkeletonName == other.m_originalSkeletonName || m_originalSkeletonName is null && other.m_originalSkeletonName == "" || m_originalSkeletonName == "" && other.m_originalSkeletonName is null) &&
                   ((m_animation is null && other.m_animation is null) || (m_animation is not null && other.m_animation is not null && m_animation.Equals((IHavokObject)other.m_animation))) &&
                   m_transformTrackToBoneIndices.SequenceEqual(other.m_transformTrackToBoneIndices) &&
                   m_floatTrackToFloatSlotIndices.SequenceEqual(other.m_floatTrackToFloatSlotIndices) &&
                   m_blendHint.Equals(other.m_blendHint) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_originalSkeletonName);
            hashcode.Add(m_animation);
            hashcode.Add(m_transformTrackToBoneIndices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_floatTrackToFloatSlotIndices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_blendHint);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

