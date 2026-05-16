using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbParticleSystemEventPayload Signatire: 0x9df46cd6 size: 80 flags: FLAGS_NONE

    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: SystemType
    // m_emitBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 18 flags: FLAGS_NONE enum: 
    // m_offset m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_direction m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_numParticles m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_speed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    public partial class hkbParticleSystemEventPayload : hkbEventPayload, IEquatable<hkbParticleSystemEventPayload?>
    {
        public byte m_type { set; get; }
        public short m_emitBoneIndex { set; get; }
        public Vector4 m_offset { set; get; }
        public Vector4 m_direction { set; get; }
        public int m_numParticles { set; get; }
        public float m_speed { set; get; }

        public override uint Signature { set; get; } = 0x9df46cd6;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_type = br.ReadByte();
            br.Position += 1;
            m_emitBoneIndex = br.ReadInt16();
            br.Position += 12;
            m_offset = br.ReadVector4();
            m_direction = br.ReadVector4();
            m_numParticles = br.ReadInt32();
            m_speed = br.ReadSingle();
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_type);
            bw.Position += 1;
            bw.WriteInt16(m_emitBoneIndex);
            bw.Position += 12;
            bw.WriteVector4(m_offset);
            bw.WriteVector4(m_direction);
            bw.WriteInt32(m_numParticles);
            bw.WriteSingle(m_speed);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_type = xd.ReadFlag<SystemType, byte>(xe, nameof(m_type));
            m_emitBoneIndex = xd.ReadInt16(xe, nameof(m_emitBoneIndex));
            m_offset = xd.ReadVector4(xe, nameof(m_offset));
            m_direction = xd.ReadVector4(xe, nameof(m_direction));
            m_numParticles = xd.ReadInt32(xe, nameof(m_numParticles));
            m_speed = xd.ReadSingle(xe, nameof(m_speed));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteEnum<SystemType, byte>(xe, nameof(m_type), m_type);
            xs.WriteNumber(xe, nameof(m_emitBoneIndex), m_emitBoneIndex);
            xs.WriteVector4(xe, nameof(m_offset), m_offset);
            xs.WriteVector4(xe, nameof(m_direction), m_direction);
            xs.WriteNumber(xe, nameof(m_numParticles), m_numParticles);
            xs.WriteFloat(xe, nameof(m_speed), m_speed);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbParticleSystemEventPayload);
        }

        public bool Equals(hkbParticleSystemEventPayload? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_type.Equals(other.m_type) &&
                   m_emitBoneIndex.Equals(other.m_emitBoneIndex) &&
                   m_offset.Equals(other.m_offset) &&
                   m_direction.Equals(other.m_direction) &&
                   m_numParticles.Equals(other.m_numParticles) &&
                   m_speed.Equals(other.m_speed) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_type);
            hashcode.Add(m_emitBoneIndex);
            hashcode.Add(m_offset);
            hashcode.Add(m_direction);
            hashcode.Add(m_numParticles);
            hashcode.Add(m_speed);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

