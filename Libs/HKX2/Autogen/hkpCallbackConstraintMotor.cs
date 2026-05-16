using System.Xml.Linq;
namespace HKX2
{
    // hkpCallbackConstraintMotor Signatire: 0xafcd79ad size: 72 flags: FLAGS_NONE

    // m_callbackFunc m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 32 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_callbackType m_class:  Type.TYPE_ENUM Type.TYPE_UINT32 arrSize: 0 offset: 40 flags: FLAGS_NONE enum: CallbackType
    // m_userData0 m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_userData1 m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_userData2 m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpCallbackConstraintMotor : hkpLimitedForceConstraintMotor, IEquatable<hkpCallbackConstraintMotor?>
    {
        private object? m_callbackFunc { set; get; }
        public uint m_callbackType { set; get; }
        public ulong m_userData0 { set; get; }
        public ulong m_userData1 { set; get; }
        public ulong m_userData2 { set; get; }

        public override uint Signature { set; get; } = 0xafcd79ad;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            des.ReadEmptyPointer(br);
            m_callbackType = br.ReadUInt32();
            br.Position += 4;
            m_userData0 = br.ReadUInt64();
            m_userData1 = br.ReadUInt64();
            m_userData2 = br.ReadUInt64();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteVoidPointer(bw);
            bw.WriteUInt32(m_callbackType);
            bw.Position += 4;
            bw.WriteUInt64(m_userData0);
            bw.WriteUInt64(m_userData1);
            bw.WriteUInt64(m_userData2);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_callbackType = xd.ReadFlag<CallbackType, uint>(xe, nameof(m_callbackType));
            m_userData0 = xd.ReadUInt64(xe, nameof(m_userData0));
            m_userData1 = xd.ReadUInt64(xe, nameof(m_userData1));
            m_userData2 = xd.ReadUInt64(xe, nameof(m_userData2));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_callbackFunc));
            xs.WriteEnum<CallbackType, uint>(xe, nameof(m_callbackType), m_callbackType);
            xs.WriteNumber(xe, nameof(m_userData0), m_userData0);
            xs.WriteNumber(xe, nameof(m_userData1), m_userData1);
            xs.WriteNumber(xe, nameof(m_userData2), m_userData2);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCallbackConstraintMotor);
        }

        public bool Equals(hkpCallbackConstraintMotor? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_callbackType.Equals(other.m_callbackType) &&
                   m_userData0.Equals(other.m_userData0) &&
                   m_userData1.Equals(other.m_userData1) &&
                   m_userData2.Equals(other.m_userData2) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_callbackType);
            hashcode.Add(m_userData0);
            hashcode.Add(m_userData1);
            hashcode.Add(m_userData2);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

