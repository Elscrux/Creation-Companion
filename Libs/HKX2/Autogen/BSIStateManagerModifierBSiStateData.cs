using System.Xml.Linq;
namespace HKX2
{
    // BSIStateManagerModifierBSiStateData Signatire: 0x6b8a15fc size: 16 flags: FLAGS_NONE

    // m_pStateMachine m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_StateID m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_iStateToSetAs m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    public partial class BSIStateManagerModifierBSiStateData : IHavokObject, IEquatable<BSIStateManagerModifierBSiStateData?>
    {
        public hkbGenerator? m_pStateMachine { set; get; }
        public int m_StateID { set; get; }
        public int m_iStateToSetAs { set; get; }

        public virtual uint Signature { set; get; } = 0x6b8a15fc;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pStateMachine = des.ReadClassPointer<hkbGenerator>(br);
            m_StateID = br.ReadInt32();
            m_iStateToSetAs = br.ReadInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer(bw, m_pStateMachine);
            bw.WriteInt32(m_StateID);
            bw.WriteInt32(m_iStateToSetAs);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_pStateMachine = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_pStateMachine));
            m_StateID = xd.ReadInt32(xe, nameof(m_StateID));
            m_iStateToSetAs = xd.ReadInt32(xe, nameof(m_iStateToSetAs));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassPointer(xe, nameof(m_pStateMachine), m_pStateMachine);
            xs.WriteNumber(xe, nameof(m_StateID), m_StateID);
            xs.WriteNumber(xe, nameof(m_iStateToSetAs), m_iStateToSetAs);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSIStateManagerModifierBSiStateData);
        }

        public bool Equals(BSIStateManagerModifierBSiStateData? other)
        {
            return other is not null &&
                   ((m_pStateMachine is null && other.m_pStateMachine is null) || (m_pStateMachine is not null && other.m_pStateMachine is not null && m_pStateMachine.Equals((IHavokObject)other.m_pStateMachine))) &&
                   m_StateID.Equals(other.m_StateID) &&
                   m_iStateToSetAs.Equals(other.m_iStateToSetAs) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_pStateMachine);
            hashcode.Add(m_StateID);
            hashcode.Add(m_iStateToSetAs);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

