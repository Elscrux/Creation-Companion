using System.Xml.Linq;
namespace HKX2
{
    // hkbContext Signatire: 0xe0c4d4a7 size: 80 flags: FLAGS_NONE

    // m_character m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_behavior m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 8 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_nodeToIndexMap m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_eventQueue m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 24 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_sharedEventQueue m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 32 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_generatorOutputListener m_class: hkbGeneratorOutputListener Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_eventTriggeredTransition m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 48 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_world m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 56 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_attachmentManager m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 64 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_animationCache m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 72 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbContext : IHavokObject, IEquatable<hkbContext?>
    {
        private object? m_character { set; get; }
        private object? m_behavior { set; get; }
        private object? m_nodeToIndexMap { set; get; }
        private object? m_eventQueue { set; get; }
        private object? m_sharedEventQueue { set; get; }
        public hkbGeneratorOutputListener? m_generatorOutputListener { set; get; }
        private bool m_eventTriggeredTransition { set; get; }
        private object? m_world { set; get; }
        private object? m_attachmentManager { set; get; }
        private object? m_animationCache { set; get; }

        public virtual uint Signature { set; get; } = 0xe0c4d4a7;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            m_generatorOutputListener = des.ReadClassPointer<hkbGeneratorOutputListener>(br);
            m_eventTriggeredTransition = br.ReadBoolean();
            br.Position += 7;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteClassPointer(bw, m_generatorOutputListener);
            bw.WriteBoolean(m_eventTriggeredTransition);
            bw.Position += 7;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_generatorOutputListener = xd.ReadClassPointer<hkbGeneratorOutputListener>(xe, nameof(m_generatorOutputListener));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_character));
            xs.WriteSerializeIgnored(xe, nameof(m_behavior));
            xs.WriteSerializeIgnored(xe, nameof(m_nodeToIndexMap));
            xs.WriteSerializeIgnored(xe, nameof(m_eventQueue));
            xs.WriteSerializeIgnored(xe, nameof(m_sharedEventQueue));
            xs.WriteClassPointer(xe, nameof(m_generatorOutputListener), m_generatorOutputListener);
            xs.WriteSerializeIgnored(xe, nameof(m_eventTriggeredTransition));
            xs.WriteSerializeIgnored(xe, nameof(m_world));
            xs.WriteSerializeIgnored(xe, nameof(m_attachmentManager));
            xs.WriteSerializeIgnored(xe, nameof(m_animationCache));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbContext);
        }

        public bool Equals(hkbContext? other)
        {
            return other is not null &&
                   ((m_generatorOutputListener is null && other.m_generatorOutputListener is null) || (m_generatorOutputListener is not null && other.m_generatorOutputListener is not null && m_generatorOutputListener.Equals((IHavokObject)other.m_generatorOutputListener))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_generatorOutputListener);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

