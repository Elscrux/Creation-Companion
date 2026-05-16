using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpRagdollMotorConstraintAtom Signatire: 0x71013826 size: 96 flags: FLAGS_NONE

    // m_isEnabled m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_initializedOffset m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_previousTargetAnglesOffset m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 6 flags: FLAGS_NONE enum: 
    // m_target_bRca m_class:  Type.TYPE_MATRIX3 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_motors m_class: hkpConstraintMotor Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 3 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpRagdollMotorConstraintAtom : hkpConstraintAtom, IEquatable<hkpRagdollMotorConstraintAtom?>
    {
        public bool m_isEnabled { set; get; }
        public short m_initializedOffset { set; get; }
        public short m_previousTargetAnglesOffset { set; get; }
        public Matrix4x4 m_target_bRca { set; get; }
        public hkpConstraintMotor?[] m_motors = new hkpConstraintMotor?[3];

        public override uint Signature { set; get; } = 0x71013826;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadBoolean();
            br.Position += 1;
            m_initializedOffset = br.ReadInt16();
            m_previousTargetAnglesOffset = br.ReadInt16();
            br.Position += 8;
            m_target_bRca = des.ReadMatrix3(br);
            m_motors = des.ReadClassPointerCStyleArray<hkpConstraintMotor>(br, 3);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_isEnabled);
            bw.Position += 1;
            bw.WriteInt16(m_initializedOffset);
            bw.WriteInt16(m_previousTargetAnglesOffset);
            bw.Position += 8;
            s.WriteMatrix3(bw, m_target_bRca);
            s.WriteClassPointerCStyleArray(bw, m_motors);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isEnabled = xd.ReadBoolean(xe, nameof(m_isEnabled));
            m_initializedOffset = xd.ReadInt16(xe, nameof(m_initializedOffset));
            m_previousTargetAnglesOffset = xd.ReadInt16(xe, nameof(m_previousTargetAnglesOffset));
            m_target_bRca = xd.ReadMatrix3(xe, nameof(m_target_bRca));
            m_motors = xd.ReadClassPointerCStyleArray<hkpConstraintMotor>(xe, nameof(m_motors), 3);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_isEnabled), m_isEnabled);
            xs.WriteNumber(xe, nameof(m_initializedOffset), m_initializedOffset);
            xs.WriteNumber(xe, nameof(m_previousTargetAnglesOffset), m_previousTargetAnglesOffset);
            xs.WriteMatrix3(xe, nameof(m_target_bRca), m_target_bRca);
            xs.WriteClassPointerArrayNullable(xe, nameof(m_motors), m_motors);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpRagdollMotorConstraintAtom);
        }

        public bool Equals(hkpRagdollMotorConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isEnabled.Equals(other.m_isEnabled) &&
                   m_initializedOffset.Equals(other.m_initializedOffset) &&
                   m_previousTargetAnglesOffset.Equals(other.m_previousTargetAnglesOffset) &&
                   m_target_bRca.Equals(other.m_target_bRca) &&
                   m_motors.SequenceEqual(other.m_motors) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_isEnabled);
            hashcode.Add(m_initializedOffset);
            hashcode.Add(m_previousTargetAnglesOffset);
            hashcode.Add(m_target_bRca);
            hashcode.Add(m_motors.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

