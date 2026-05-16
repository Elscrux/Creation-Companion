using System.Xml.Linq;
namespace HKX2
{
    // hkbAttachmentSetup Signatire: 0x774632b size: 48 flags: FLAGS_NONE

    // m_blendInTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_moveAttacherFraction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_gain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_extrapolationTimeStep m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 28 flags: FLAGS_NONE enum: 
    // m_fixUpGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_maxLinearDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_maxAngularDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_attachmentType m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 44 flags: FLAGS_NONE enum: AttachmentType
    public partial class hkbAttachmentSetup : hkReferencedObject, IEquatable<hkbAttachmentSetup?>
    {
        public float m_blendInTime { set; get; }
        public float m_moveAttacherFraction { set; get; }
        public float m_gain { set; get; }
        public float m_extrapolationTimeStep { set; get; }
        public float m_fixUpGain { set; get; }
        public float m_maxLinearDistance { set; get; }
        public float m_maxAngularDistance { set; get; }
        public sbyte m_attachmentType { set; get; }

        public override uint Signature { set; get; } = 0x774632b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_blendInTime = br.ReadSingle();
            m_moveAttacherFraction = br.ReadSingle();
            m_gain = br.ReadSingle();
            m_extrapolationTimeStep = br.ReadSingle();
            m_fixUpGain = br.ReadSingle();
            m_maxLinearDistance = br.ReadSingle();
            m_maxAngularDistance = br.ReadSingle();
            m_attachmentType = br.ReadSByte();
            br.Position += 3;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_blendInTime);
            bw.WriteSingle(m_moveAttacherFraction);
            bw.WriteSingle(m_gain);
            bw.WriteSingle(m_extrapolationTimeStep);
            bw.WriteSingle(m_fixUpGain);
            bw.WriteSingle(m_maxLinearDistance);
            bw.WriteSingle(m_maxAngularDistance);
            bw.WriteSByte(m_attachmentType);
            bw.Position += 3;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_blendInTime = xd.ReadSingle(xe, nameof(m_blendInTime));
            m_moveAttacherFraction = xd.ReadSingle(xe, nameof(m_moveAttacherFraction));
            m_gain = xd.ReadSingle(xe, nameof(m_gain));
            m_extrapolationTimeStep = xd.ReadSingle(xe, nameof(m_extrapolationTimeStep));
            m_fixUpGain = xd.ReadSingle(xe, nameof(m_fixUpGain));
            m_maxLinearDistance = xd.ReadSingle(xe, nameof(m_maxLinearDistance));
            m_maxAngularDistance = xd.ReadSingle(xe, nameof(m_maxAngularDistance));
            m_attachmentType = xd.ReadFlag<AttachmentType, sbyte>(xe, nameof(m_attachmentType));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_blendInTime), m_blendInTime);
            xs.WriteFloat(xe, nameof(m_moveAttacherFraction), m_moveAttacherFraction);
            xs.WriteFloat(xe, nameof(m_gain), m_gain);
            xs.WriteFloat(xe, nameof(m_extrapolationTimeStep), m_extrapolationTimeStep);
            xs.WriteFloat(xe, nameof(m_fixUpGain), m_fixUpGain);
            xs.WriteFloat(xe, nameof(m_maxLinearDistance), m_maxLinearDistance);
            xs.WriteFloat(xe, nameof(m_maxAngularDistance), m_maxAngularDistance);
            xs.WriteEnum<AttachmentType, sbyte>(xe, nameof(m_attachmentType), m_attachmentType);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbAttachmentSetup);
        }

        public bool Equals(hkbAttachmentSetup? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_blendInTime.Equals(other.m_blendInTime) &&
                   m_moveAttacherFraction.Equals(other.m_moveAttacherFraction) &&
                   m_gain.Equals(other.m_gain) &&
                   m_extrapolationTimeStep.Equals(other.m_extrapolationTimeStep) &&
                   m_fixUpGain.Equals(other.m_fixUpGain) &&
                   m_maxLinearDistance.Equals(other.m_maxLinearDistance) &&
                   m_maxAngularDistance.Equals(other.m_maxAngularDistance) &&
                   m_attachmentType.Equals(other.m_attachmentType) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_blendInTime);
            hashcode.Add(m_moveAttacherFraction);
            hashcode.Add(m_gain);
            hashcode.Add(m_extrapolationTimeStep);
            hashcode.Add(m_fixUpGain);
            hashcode.Add(m_maxLinearDistance);
            hashcode.Add(m_maxAngularDistance);
            hashcode.Add(m_attachmentType);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

