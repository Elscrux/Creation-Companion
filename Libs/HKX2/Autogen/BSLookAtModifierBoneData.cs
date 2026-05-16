using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // BSLookAtModifierBoneData Signatire: 0x29efee59 size: 64 flags: FLAGS_NONE

    // m_index m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_fwdAxisLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_limitAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_onGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_offGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_enabled m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 44 flags: FLAGS_NONE enum: 
    // m_currentFwdAxisLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSLookAtModifierBoneData : IHavokObject, IEquatable<BSLookAtModifierBoneData?>
    {
        public short m_index { set; get; }
        public Vector4 m_fwdAxisLS { set; get; }
        public float m_limitAngleDegrees { set; get; }
        public float m_onGain { set; get; }
        public float m_offGain { set; get; }
        public bool m_enabled { set; get; }
        private Vector4 m_currentFwdAxisLS { set; get; }

        public virtual uint Signature { set; get; } = 0x29efee59;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_index = br.ReadInt16();
            br.Position += 14;
            m_fwdAxisLS = br.ReadVector4();
            m_limitAngleDegrees = br.ReadSingle();
            m_onGain = br.ReadSingle();
            m_offGain = br.ReadSingle();
            m_enabled = br.ReadBoolean();
            br.Position += 3;
            m_currentFwdAxisLS = br.ReadVector4();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt16(m_index);
            bw.Position += 14;
            bw.WriteVector4(m_fwdAxisLS);
            bw.WriteSingle(m_limitAngleDegrees);
            bw.WriteSingle(m_onGain);
            bw.WriteSingle(m_offGain);
            bw.WriteBoolean(m_enabled);
            bw.Position += 3;
            bw.WriteVector4(m_currentFwdAxisLS);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_index = xd.ReadInt16(xe, nameof(m_index));
            m_fwdAxisLS = xd.ReadVector4(xe, nameof(m_fwdAxisLS));
            m_limitAngleDegrees = xd.ReadSingle(xe, nameof(m_limitAngleDegrees));
            m_onGain = xd.ReadSingle(xe, nameof(m_onGain));
            m_offGain = xd.ReadSingle(xe, nameof(m_offGain));
            m_enabled = xd.ReadBoolean(xe, nameof(m_enabled));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_index), m_index);
            xs.WriteVector4(xe, nameof(m_fwdAxisLS), m_fwdAxisLS);
            xs.WriteFloat(xe, nameof(m_limitAngleDegrees), m_limitAngleDegrees);
            xs.WriteFloat(xe, nameof(m_onGain), m_onGain);
            xs.WriteFloat(xe, nameof(m_offGain), m_offGain);
            xs.WriteBoolean(xe, nameof(m_enabled), m_enabled);
            xs.WriteSerializeIgnored(xe, nameof(m_currentFwdAxisLS));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSLookAtModifierBoneData);
        }

        public bool Equals(BSLookAtModifierBoneData? other)
        {
            return other is not null &&
                   m_index.Equals(other.m_index) &&
                   m_fwdAxisLS.Equals(other.m_fwdAxisLS) &&
                   m_limitAngleDegrees.Equals(other.m_limitAngleDegrees) &&
                   m_onGain.Equals(other.m_onGain) &&
                   m_offGain.Equals(other.m_offGain) &&
                   m_enabled.Equals(other.m_enabled) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_index);
            hashcode.Add(m_fwdAxisLS);
            hashcode.Add(m_limitAngleDegrees);
            hashcode.Add(m_onGain);
            hashcode.Add(m_offGain);
            hashcode.Add(m_enabled);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

