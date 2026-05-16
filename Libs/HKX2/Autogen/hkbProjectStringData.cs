using System.Xml.Linq;
namespace HKX2
{
    // hkbProjectStringData Signatire: 0x76ad60a size: 120 flags: FLAGS_NONE

    // m_animationFilenames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_behaviorFilenames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_characterFilenames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_eventNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_animationPath m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_behaviorPath m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_characterPath m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_fullPathToSource m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_rootPath m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbProjectStringData : hkReferencedObject, IEquatable<hkbProjectStringData?>
    {
        public IList<string> m_animationFilenames { set; get; } = Array.Empty<string>();
        public IList<string> m_behaviorFilenames { set; get; } = Array.Empty<string>();
        public IList<string> m_characterFilenames { set; get; } = Array.Empty<string>();
        public IList<string> m_eventNames { set; get; } = Array.Empty<string>();
        public string m_animationPath { set; get; } = "";
        public string m_behaviorPath { set; get; } = "";
        public string m_characterPath { set; get; } = "";
        public string m_fullPathToSource { set; get; } = "";
        public string m_rootPath { set; get; } = "";

        public override uint Signature { set; get; } = 0x76ad60a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_animationFilenames = des.ReadStringPointerArray(br);
            m_behaviorFilenames = des.ReadStringPointerArray(br);
            m_characterFilenames = des.ReadStringPointerArray(br);
            m_eventNames = des.ReadStringPointerArray(br);
            m_animationPath = des.ReadStringPointer(br);
            m_behaviorPath = des.ReadStringPointer(br);
            m_characterPath = des.ReadStringPointer(br);
            m_fullPathToSource = des.ReadStringPointer(br);
            m_rootPath = des.ReadStringPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointerArray(bw, m_animationFilenames);
            s.WriteStringPointerArray(bw, m_behaviorFilenames);
            s.WriteStringPointerArray(bw, m_characterFilenames);
            s.WriteStringPointerArray(bw, m_eventNames);
            s.WriteStringPointer(bw, m_animationPath);
            s.WriteStringPointer(bw, m_behaviorPath);
            s.WriteStringPointer(bw, m_characterPath);
            s.WriteStringPointer(bw, m_fullPathToSource);
            s.WriteStringPointer(bw, m_rootPath);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_animationFilenames = xd.ReadStringArray(xe, nameof(m_animationFilenames));
            m_behaviorFilenames = xd.ReadStringArray(xe, nameof(m_behaviorFilenames));
            m_characterFilenames = xd.ReadStringArray(xe, nameof(m_characterFilenames));
            m_eventNames = xd.ReadStringArray(xe, nameof(m_eventNames));
            m_animationPath = xd.ReadString(xe, nameof(m_animationPath));
            m_behaviorPath = xd.ReadString(xe, nameof(m_behaviorPath));
            m_characterPath = xd.ReadString(xe, nameof(m_characterPath));
            m_fullPathToSource = xd.ReadString(xe, nameof(m_fullPathToSource));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteStringArray(xe, nameof(m_animationFilenames), m_animationFilenames);
            xs.WriteStringArray(xe, nameof(m_behaviorFilenames), m_behaviorFilenames);
            xs.WriteStringArray(xe, nameof(m_characterFilenames), m_characterFilenames);
            xs.WriteStringArray(xe, nameof(m_eventNames), m_eventNames);
            xs.WriteString(xe, nameof(m_animationPath), m_animationPath);
            xs.WriteString(xe, nameof(m_behaviorPath), m_behaviorPath);
            xs.WriteString(xe, nameof(m_characterPath), m_characterPath);
            xs.WriteString(xe, nameof(m_fullPathToSource), m_fullPathToSource);
            xs.WriteSerializeIgnored(xe, nameof(m_rootPath));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbProjectStringData);
        }

        public bool Equals(hkbProjectStringData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_animationFilenames.SequenceEqual(other.m_animationFilenames) &&
                   m_behaviorFilenames.SequenceEqual(other.m_behaviorFilenames) &&
                   m_characterFilenames.SequenceEqual(other.m_characterFilenames) &&
                   m_eventNames.SequenceEqual(other.m_eventNames) &&
                   (m_animationPath is null && other.m_animationPath is null || m_animationPath == other.m_animationPath || m_animationPath is null && other.m_animationPath == "" || m_animationPath == "" && other.m_animationPath is null) &&
                   (m_behaviorPath is null && other.m_behaviorPath is null || m_behaviorPath == other.m_behaviorPath || m_behaviorPath is null && other.m_behaviorPath == "" || m_behaviorPath == "" && other.m_behaviorPath is null) &&
                   (m_characterPath is null && other.m_characterPath is null || m_characterPath == other.m_characterPath || m_characterPath is null && other.m_characterPath == "" || m_characterPath == "" && other.m_characterPath is null) &&
                   (m_fullPathToSource is null && other.m_fullPathToSource is null || m_fullPathToSource == other.m_fullPathToSource || m_fullPathToSource is null && other.m_fullPathToSource == "" || m_fullPathToSource == "" && other.m_fullPathToSource is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_animationFilenames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_behaviorFilenames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_characterFilenames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_eventNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_animationPath);
            hashcode.Add(m_behaviorPath);
            hashcode.Add(m_characterPath);
            hashcode.Add(m_fullPathToSource);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

