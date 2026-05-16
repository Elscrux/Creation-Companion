using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // BSTweenerModifier Signatire: 0xd2d9a04 size: 208 flags: FLAGS_NONE

    // m_tweenPosition m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_tweenRotation m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 81 flags: FLAGS_NONE enum: 
    // m_useTweenDuration m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 82 flags: FLAGS_NONE enum: 
    // m_tweenDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_targetPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_targetRotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_duration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 128 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_startTransform m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 144 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_time m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 192 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSTweenerModifier : hkbModifier, IEquatable<BSTweenerModifier?>
    {
        public bool m_tweenPosition { set; get; }
        public bool m_tweenRotation { set; get; }
        public bool m_useTweenDuration { set; get; }
        public float m_tweenDuration { set; get; }
        public Vector4 m_targetPosition { set; get; }
        public Quaternion m_targetRotation { set; get; }
        private float m_duration { set; get; }
        private Matrix4x4 m_startTransform { set; get; }
        private float m_time { set; get; }

        public override uint Signature { set; get; } = 0xd2d9a04;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_tweenPosition = br.ReadBoolean();
            m_tweenRotation = br.ReadBoolean();
            m_useTweenDuration = br.ReadBoolean();
            br.Position += 1;
            m_tweenDuration = br.ReadSingle();
            br.Position += 8;
            m_targetPosition = br.ReadVector4();
            m_targetRotation = des.ReadQuaternion(br);
            m_duration = br.ReadSingle();
            br.Position += 12;
            m_startTransform = des.ReadQSTransform(br);
            m_time = br.ReadSingle();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_tweenPosition);
            bw.WriteBoolean(m_tweenRotation);
            bw.WriteBoolean(m_useTweenDuration);
            bw.Position += 1;
            bw.WriteSingle(m_tweenDuration);
            bw.Position += 8;
            bw.WriteVector4(m_targetPosition);
            s.WriteQuaternion(bw, m_targetRotation);
            bw.WriteSingle(m_duration);
            bw.Position += 12;
            s.WriteQSTransform(bw, m_startTransform);
            bw.WriteSingle(m_time);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_tweenPosition = xd.ReadBoolean(xe, nameof(m_tweenPosition));
            m_tweenRotation = xd.ReadBoolean(xe, nameof(m_tweenRotation));
            m_useTweenDuration = xd.ReadBoolean(xe, nameof(m_useTweenDuration));
            m_tweenDuration = xd.ReadSingle(xe, nameof(m_tweenDuration));
            m_targetPosition = xd.ReadVector4(xe, nameof(m_targetPosition));
            m_targetRotation = xd.ReadQuaternion(xe, nameof(m_targetRotation));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_tweenPosition), m_tweenPosition);
            xs.WriteBoolean(xe, nameof(m_tweenRotation), m_tweenRotation);
            xs.WriteBoolean(xe, nameof(m_useTweenDuration), m_useTweenDuration);
            xs.WriteFloat(xe, nameof(m_tweenDuration), m_tweenDuration);
            xs.WriteVector4(xe, nameof(m_targetPosition), m_targetPosition);
            xs.WriteQuaternion(xe, nameof(m_targetRotation), m_targetRotation);
            xs.WriteSerializeIgnored(xe, nameof(m_duration));
            xs.WriteSerializeIgnored(xe, nameof(m_startTransform));
            xs.WriteSerializeIgnored(xe, nameof(m_time));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSTweenerModifier);
        }

        public bool Equals(BSTweenerModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_tweenPosition.Equals(other.m_tweenPosition) &&
                   m_tweenRotation.Equals(other.m_tweenRotation) &&
                   m_useTweenDuration.Equals(other.m_useTweenDuration) &&
                   m_tweenDuration.Equals(other.m_tweenDuration) &&
                   m_targetPosition.Equals(other.m_targetPosition) &&
                   m_targetRotation.Equals(other.m_targetRotation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_tweenPosition);
            hashcode.Add(m_tweenRotation);
            hashcode.Add(m_useTweenDuration);
            hashcode.Add(m_tweenDuration);
            hashcode.Add(m_targetPosition);
            hashcode.Add(m_targetRotation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

