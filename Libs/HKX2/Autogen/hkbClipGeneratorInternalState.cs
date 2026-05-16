using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbClipGeneratorInternalState Signatire: 0x26ce5bf3 size: 112 flags: FLAGS_NONE

    // m_extractedMotion m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_echos m_class: hkbClipGeneratorEcho Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_localTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_time m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_previousUserControlledTimeFraction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_bufferSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    // m_echoBufferSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_atEnd m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_ignoreStartTime m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 101 flags: FLAGS_NONE enum: 
    // m_pingPongBackward m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 102 flags: FLAGS_NONE enum: 
    public partial class hkbClipGeneratorInternalState : hkReferencedObject, IEquatable<hkbClipGeneratorInternalState?>
    {
        public Matrix4x4 m_extractedMotion { set; get; }
        public IList<hkbClipGeneratorEcho> m_echos { set; get; } = Array.Empty<hkbClipGeneratorEcho>();
        public float m_localTime { set; get; }
        public float m_time { set; get; }
        public float m_previousUserControlledTimeFraction { set; get; }
        public int m_bufferSize { set; get; }
        public int m_echoBufferSize { set; get; }
        public bool m_atEnd { set; get; }
        public bool m_ignoreStartTime { set; get; }
        public bool m_pingPongBackward { set; get; }

        public override uint Signature { set; get; } = 0x26ce5bf3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_extractedMotion = des.ReadQSTransform(br);
            m_echos = des.ReadClassArray<hkbClipGeneratorEcho>(br);
            m_localTime = br.ReadSingle();
            m_time = br.ReadSingle();
            m_previousUserControlledTimeFraction = br.ReadSingle();
            m_bufferSize = br.ReadInt32();
            m_echoBufferSize = br.ReadInt32();
            m_atEnd = br.ReadBoolean();
            m_ignoreStartTime = br.ReadBoolean();
            m_pingPongBackward = br.ReadBoolean();
            br.Position += 9;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteQSTransform(bw, m_extractedMotion);
            s.WriteClassArray(bw, m_echos);
            bw.WriteSingle(m_localTime);
            bw.WriteSingle(m_time);
            bw.WriteSingle(m_previousUserControlledTimeFraction);
            bw.WriteInt32(m_bufferSize);
            bw.WriteInt32(m_echoBufferSize);
            bw.WriteBoolean(m_atEnd);
            bw.WriteBoolean(m_ignoreStartTime);
            bw.WriteBoolean(m_pingPongBackward);
            bw.Position += 9;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_extractedMotion = xd.ReadQSTransform(xe, nameof(m_extractedMotion));
            m_echos = xd.ReadClassArray<hkbClipGeneratorEcho>(xe, nameof(m_echos));
            m_localTime = xd.ReadSingle(xe, nameof(m_localTime));
            m_time = xd.ReadSingle(xe, nameof(m_time));
            m_previousUserControlledTimeFraction = xd.ReadSingle(xe, nameof(m_previousUserControlledTimeFraction));
            m_bufferSize = xd.ReadInt32(xe, nameof(m_bufferSize));
            m_echoBufferSize = xd.ReadInt32(xe, nameof(m_echoBufferSize));
            m_atEnd = xd.ReadBoolean(xe, nameof(m_atEnd));
            m_ignoreStartTime = xd.ReadBoolean(xe, nameof(m_ignoreStartTime));
            m_pingPongBackward = xd.ReadBoolean(xe, nameof(m_pingPongBackward));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteQSTransform(xe, nameof(m_extractedMotion), m_extractedMotion);
            xs.WriteClassArray(xe, nameof(m_echos), m_echos);
            xs.WriteFloat(xe, nameof(m_localTime), m_localTime);
            xs.WriteFloat(xe, nameof(m_time), m_time);
            xs.WriteFloat(xe, nameof(m_previousUserControlledTimeFraction), m_previousUserControlledTimeFraction);
            xs.WriteNumber(xe, nameof(m_bufferSize), m_bufferSize);
            xs.WriteNumber(xe, nameof(m_echoBufferSize), m_echoBufferSize);
            xs.WriteBoolean(xe, nameof(m_atEnd), m_atEnd);
            xs.WriteBoolean(xe, nameof(m_ignoreStartTime), m_ignoreStartTime);
            xs.WriteBoolean(xe, nameof(m_pingPongBackward), m_pingPongBackward);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbClipGeneratorInternalState);
        }

        public bool Equals(hkbClipGeneratorInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_extractedMotion.Equals(other.m_extractedMotion) &&
                   m_echos.SequenceEqual(other.m_echos) &&
                   m_localTime.Equals(other.m_localTime) &&
                   m_time.Equals(other.m_time) &&
                   m_previousUserControlledTimeFraction.Equals(other.m_previousUserControlledTimeFraction) &&
                   m_bufferSize.Equals(other.m_bufferSize) &&
                   m_echoBufferSize.Equals(other.m_echoBufferSize) &&
                   m_atEnd.Equals(other.m_atEnd) &&
                   m_ignoreStartTime.Equals(other.m_ignoreStartTime) &&
                   m_pingPongBackward.Equals(other.m_pingPongBackward) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_extractedMotion);
            hashcode.Add(m_echos.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_localTime);
            hashcode.Add(m_time);
            hashcode.Add(m_previousUserControlledTimeFraction);
            hashcode.Add(m_bufferSize);
            hashcode.Add(m_echoBufferSize);
            hashcode.Add(m_atEnd);
            hashcode.Add(m_ignoreStartTime);
            hashcode.Add(m_pingPongBackward);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

