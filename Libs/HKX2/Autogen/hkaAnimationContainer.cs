using System.Xml.Linq;
namespace HKX2
{
    // hkaAnimationContainer Signatire: 0x8dc20333 size: 96 flags: FLAGS_NONE

    // m_skeletons m_class: hkaSkeleton Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_animations m_class: hkaAnimation Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_bindings m_class: hkaAnimationBinding Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_attachments m_class: hkaBoneAttachment Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_skins m_class: hkaMeshBinding Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkaAnimationContainer : hkReferencedObject, IEquatable<hkaAnimationContainer?>
    {
        public IList<hkaSkeleton> m_skeletons { set; get; } = Array.Empty<hkaSkeleton>();
        public IList<hkaAnimation> m_animations { set; get; } = Array.Empty<hkaAnimation>();
        public IList<hkaAnimationBinding> m_bindings { set; get; } = Array.Empty<hkaAnimationBinding>();
        public IList<hkaBoneAttachment> m_attachments { set; get; } = Array.Empty<hkaBoneAttachment>();
        public IList<hkaMeshBinding> m_skins { set; get; } = Array.Empty<hkaMeshBinding>();

        public override uint Signature { set; get; } = 0x8dc20333;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_skeletons = des.ReadClassPointerArray<hkaSkeleton>(br);
            m_animations = des.ReadClassPointerArray<hkaAnimation>(br);
            m_bindings = des.ReadClassPointerArray<hkaAnimationBinding>(br);
            m_attachments = des.ReadClassPointerArray<hkaBoneAttachment>(br);
            m_skins = des.ReadClassPointerArray<hkaMeshBinding>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_skeletons);
            s.WriteClassPointerArray(bw, m_animations);
            s.WriteClassPointerArray(bw, m_bindings);
            s.WriteClassPointerArray(bw, m_attachments);
            s.WriteClassPointerArray(bw, m_skins);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_skeletons = xd.ReadClassPointerArray<hkaSkeleton>(xe, nameof(m_skeletons));
            m_animations = xd.ReadClassPointerArray<hkaAnimation>(xe, nameof(m_animations));
            m_bindings = xd.ReadClassPointerArray<hkaAnimationBinding>(xe, nameof(m_bindings));
            m_attachments = xd.ReadClassPointerArray<hkaBoneAttachment>(xe, nameof(m_attachments));
            m_skins = xd.ReadClassPointerArray<hkaMeshBinding>(xe, nameof(m_skins));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_skeletons), m_skeletons);
            xs.WriteClassPointerArray(xe, nameof(m_animations), m_animations);
            xs.WriteClassPointerArray(xe, nameof(m_bindings), m_bindings);
            xs.WriteClassPointerArray(xe, nameof(m_attachments), m_attachments);
            xs.WriteClassPointerArray(xe, nameof(m_skins), m_skins);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaAnimationContainer);
        }

        public bool Equals(hkaAnimationContainer? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_skeletons.SequenceEqual(other.m_skeletons) &&
                   m_animations.SequenceEqual(other.m_animations) &&
                   m_bindings.SequenceEqual(other.m_bindings) &&
                   m_attachments.SequenceEqual(other.m_attachments) &&
                   m_skins.SequenceEqual(other.m_skins) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_skeletons.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_animations.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_bindings.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_attachments.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_skins.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

