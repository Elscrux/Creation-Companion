using System.Xml.Linq;
namespace HKX2
{
    // hkbEvaluateExpressionModifierInternalExpressionData Signatire: 0xb8686f6b size: 2 flags: FLAGS_NONE

    // m_raisedEvent m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_wasTrueInPreviousFrame m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 1 flags: FLAGS_NONE enum: 
    public partial class hkbEvaluateExpressionModifierInternalExpressionData : IHavokObject, IEquatable<hkbEvaluateExpressionModifierInternalExpressionData?>
    {
        public bool m_raisedEvent { set; get; }
        public bool m_wasTrueInPreviousFrame { set; get; }

        public virtual uint Signature { set; get; } = 0xb8686f6b;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_raisedEvent = br.ReadBoolean();
            m_wasTrueInPreviousFrame = br.ReadBoolean();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteBoolean(m_raisedEvent);
            bw.WriteBoolean(m_wasTrueInPreviousFrame);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_raisedEvent = xd.ReadBoolean(xe, nameof(m_raisedEvent));
            m_wasTrueInPreviousFrame = xd.ReadBoolean(xe, nameof(m_wasTrueInPreviousFrame));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteBoolean(xe, nameof(m_raisedEvent), m_raisedEvent);
            xs.WriteBoolean(xe, nameof(m_wasTrueInPreviousFrame), m_wasTrueInPreviousFrame);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEvaluateExpressionModifierInternalExpressionData);
        }

        public bool Equals(hkbEvaluateExpressionModifierInternalExpressionData? other)
        {
            return other is not null &&
                   m_raisedEvent.Equals(other.m_raisedEvent) &&
                   m_wasTrueInPreviousFrame.Equals(other.m_wasTrueInPreviousFrame) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_raisedEvent);
            hashcode.Add(m_wasTrueInPreviousFrame);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

