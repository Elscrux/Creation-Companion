using System.Xml.Linq;
namespace HKX2
{
    // hkbExpressionData Signatire: 0x6740042a size: 24 flags: FLAGS_NONE

    // m_expression m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_assignmentVariableIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_assignmentEventIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_eventMode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: ExpressionEventMode
    // m_raisedEvent m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 17 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_wasTrueInPreviousFrame m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 18 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbExpressionData : IHavokObject, IEquatable<hkbExpressionData?>
    {
        public string m_expression { set; get; } = "";
        public int m_assignmentVariableIndex { set; get; }
        public int m_assignmentEventIndex { set; get; }
        public sbyte m_eventMode { set; get; }
        private bool m_raisedEvent { set; get; }
        private bool m_wasTrueInPreviousFrame { set; get; }

        public virtual uint Signature { set; get; } = 0x6740042a;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_expression = des.ReadStringPointer(br);
            m_assignmentVariableIndex = br.ReadInt32();
            m_assignmentEventIndex = br.ReadInt32();
            m_eventMode = br.ReadSByte();
            m_raisedEvent = br.ReadBoolean();
            m_wasTrueInPreviousFrame = br.ReadBoolean();
            br.Position += 5;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStringPointer(bw, m_expression);
            bw.WriteInt32(m_assignmentVariableIndex);
            bw.WriteInt32(m_assignmentEventIndex);
            bw.WriteSByte(m_eventMode);
            bw.WriteBoolean(m_raisedEvent);
            bw.WriteBoolean(m_wasTrueInPreviousFrame);
            bw.Position += 5;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_expression = xd.ReadString(xe, nameof(m_expression));
            m_assignmentVariableIndex = xd.ReadInt32(xe, nameof(m_assignmentVariableIndex));
            m_assignmentEventIndex = xd.ReadInt32(xe, nameof(m_assignmentEventIndex));
            m_eventMode = xd.ReadFlag<ExpressionEventMode, sbyte>(xe, nameof(m_eventMode));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_expression), m_expression);
            xs.WriteNumber(xe, nameof(m_assignmentVariableIndex), m_assignmentVariableIndex);
            xs.WriteNumber(xe, nameof(m_assignmentEventIndex), m_assignmentEventIndex);
            xs.WriteEnum<ExpressionEventMode, sbyte>(xe, nameof(m_eventMode), m_eventMode);
            xs.WriteSerializeIgnored(xe, nameof(m_raisedEvent));
            xs.WriteSerializeIgnored(xe, nameof(m_wasTrueInPreviousFrame));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbExpressionData);
        }

        public bool Equals(hkbExpressionData? other)
        {
            return other is not null &&
                   (m_expression is null && other.m_expression is null || m_expression == other.m_expression || m_expression is null && other.m_expression == "" || m_expression == "" && other.m_expression is null) &&
                   m_assignmentVariableIndex.Equals(other.m_assignmentVariableIndex) &&
                   m_assignmentEventIndex.Equals(other.m_assignmentEventIndex) &&
                   m_eventMode.Equals(other.m_eventMode) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_expression);
            hashcode.Add(m_assignmentVariableIndex);
            hashcode.Add(m_assignmentEventIndex);
            hashcode.Add(m_eventMode);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

