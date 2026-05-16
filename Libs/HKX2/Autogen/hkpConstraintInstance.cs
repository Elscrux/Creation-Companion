using System.Xml.Linq;
namespace HKX2
{
    // hkpConstraintInstance Signatire: 0x34eba5f size: 112 flags: FLAGS_NONE

    // m_owner m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_data m_class: hkpConstraintData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_constraintModifiers m_class: hkpModifierConstraintAtom Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_entities m_class: hkpEntity Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 2 offset: 40 flags: FLAGS_NONE enum: 
    // m_priority m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 56 flags: FLAGS_NONE enum: ConstraintPriority
    // m_wantRuntime m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 57 flags: FLAGS_NONE enum: 
    // m_destructionRemapInfo m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 58 flags: FLAGS_NONE enum: OnDestructionRemapInfo
    // m_listeners m_class: hkpConstraintInstanceSmallArraySerializeOverrideType Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 64 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_userData m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_internal m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_uid m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 104 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpConstraintInstance : hkReferencedObject, IEquatable<hkpConstraintInstance?>
    {
        private object? m_owner { set; get; }
        public hkpConstraintData? m_data { set; get; }
        public hkpModifierConstraintAtom? m_constraintModifiers { set; get; }
        public hkpEntity?[] m_entities = new hkpEntity?[2];
        public byte m_priority { set; get; }
        public bool m_wantRuntime { set; get; }
        public byte m_destructionRemapInfo { set; get; }
        public hkpConstraintInstanceSmallArraySerializeOverrideType m_listeners { set; get; } = new();
        public string m_name { set; get; } = "";
        public ulong m_userData { set; get; }
        private object? m_internal { set; get; }
        private uint m_uid { set; get; }

        public override uint Signature { set; get; } = 0x34eba5f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            des.ReadEmptyPointer(br);
            m_data = des.ReadClassPointer<hkpConstraintData>(br);
            m_constraintModifiers = des.ReadClassPointer<hkpModifierConstraintAtom>(br);
            m_entities = des.ReadClassPointerCStyleArray<hkpEntity>(br, 2);
            m_priority = br.ReadByte();
            m_wantRuntime = br.ReadBoolean();
            m_destructionRemapInfo = br.ReadByte();
            br.Position += 5;
            m_listeners.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_userData = br.ReadUInt64();
            des.ReadEmptyPointer(br);
            m_uid = br.ReadUInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteVoidPointer(bw);
            s.WriteClassPointer(bw, m_data);
            s.WriteClassPointer(bw, m_constraintModifiers);
            s.WriteClassPointerCStyleArray(bw, m_entities);
            bw.WriteByte(m_priority);
            bw.WriteBoolean(m_wantRuntime);
            bw.WriteByte(m_destructionRemapInfo);
            bw.Position += 5;
            m_listeners.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            bw.WriteUInt64(m_userData);
            s.WriteVoidPointer(bw);
            bw.WriteUInt32(m_uid);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_data = xd.ReadClassPointer<hkpConstraintData>(xe, nameof(m_data));
            m_constraintModifiers = xd.ReadClassPointer<hkpModifierConstraintAtom>(xe, nameof(m_constraintModifiers));
            m_entities = xd.ReadClassPointerCStyleArray<hkpEntity>(xe, nameof(m_entities), 2);
            m_priority = xd.ReadFlag<ConstraintPriority, byte>(xe, nameof(m_priority));
            m_wantRuntime = xd.ReadBoolean(xe, nameof(m_wantRuntime));
            m_destructionRemapInfo = xd.ReadFlag<OnDestructionRemapInfo, byte>(xe, nameof(m_destructionRemapInfo));
            m_name = xd.ReadString(xe, nameof(m_name));
            m_userData = xd.ReadUInt64(xe, nameof(m_userData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_owner));
            xs.WriteClassPointer(xe, nameof(m_data), m_data);
            xs.WriteClassPointer(xe, nameof(m_constraintModifiers), m_constraintModifiers);
            xs.WriteClassPointerArrayNullable(xe, nameof(m_entities), m_entities);
            xs.WriteEnum<ConstraintPriority, byte>(xe, nameof(m_priority), m_priority);
            xs.WriteBoolean(xe, nameof(m_wantRuntime), m_wantRuntime);
            xs.WriteEnum<OnDestructionRemapInfo, byte>(xe, nameof(m_destructionRemapInfo), m_destructionRemapInfo);
            xs.WriteSerializeIgnored(xe, nameof(m_listeners));
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteNumber(xe, nameof(m_userData), m_userData);
            xs.WriteSerializeIgnored(xe, nameof(m_internal));
            xs.WriteSerializeIgnored(xe, nameof(m_uid));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConstraintInstance);
        }

        public bool Equals(hkpConstraintInstance? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_data is null && other.m_data is null) || (m_data is not null && other.m_data is not null && m_data.Equals((IHavokObject)other.m_data))) &&
                   ((m_constraintModifiers is null && other.m_constraintModifiers is null) || (m_constraintModifiers is not null && other.m_constraintModifiers is not null && m_constraintModifiers.Equals((IHavokObject)other.m_constraintModifiers))) &&
                   m_entities.SequenceEqual(other.m_entities) &&
                   m_priority.Equals(other.m_priority) &&
                   m_wantRuntime.Equals(other.m_wantRuntime) &&
                   m_destructionRemapInfo.Equals(other.m_destructionRemapInfo) &&
                   m_name == other.m_name &&
                   m_userData.Equals(other.m_userData) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_data);
            hashcode.Add(m_constraintModifiers);
            hashcode.Add(m_entities.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_priority);
            hashcode.Add(m_wantRuntime);
            hashcode.Add(m_destructionRemapInfo);
            hashcode.Add(m_name);
            hashcode.Add(m_userData);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

