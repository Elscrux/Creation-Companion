using System.Xml.Linq;
namespace HKX2
{
    // hkbHandle Signatire: 0xd8b6401c size: 48 flags: FLAGS_NONE

    // m_frame m_class: hkLocalFrame Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_rigidBody m_class: hkpRigidBody Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_character m_class: hkbCharacter Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_animationBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    public partial class hkbHandle : hkReferencedObject, IEquatable<hkbHandle?>
    {
        public hkLocalFrame? m_frame { set; get; }
        public hkpRigidBody? m_rigidBody { set; get; }
        public hkbCharacter? m_character { set; get; }
        public short m_animationBoneIndex { set; get; }

        public override uint Signature { set; get; } = 0xd8b6401c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_frame = des.ReadClassPointer<hkLocalFrame>(br);
            m_rigidBody = des.ReadClassPointer<hkpRigidBody>(br);
            m_character = des.ReadClassPointer<hkbCharacter>(br);
            m_animationBoneIndex = br.ReadInt16();
            br.Position += 6;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_frame);
            s.WriteClassPointer(bw, m_rigidBody);
            s.WriteClassPointer(bw, m_character);
            bw.WriteInt16(m_animationBoneIndex);
            bw.Position += 6;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_frame = xd.ReadClassPointer<hkLocalFrame>(xe, nameof(m_frame));
            m_rigidBody = xd.ReadClassPointer<hkpRigidBody>(xe, nameof(m_rigidBody));
            m_character = xd.ReadClassPointer<hkbCharacter>(xe, nameof(m_character));
            m_animationBoneIndex = xd.ReadInt16(xe, nameof(m_animationBoneIndex));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_frame), m_frame);
            xs.WriteClassPointer(xe, nameof(m_rigidBody), m_rigidBody);
            xs.WriteClassPointer(xe, nameof(m_character), m_character);
            xs.WriteNumber(xe, nameof(m_animationBoneIndex), m_animationBoneIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbHandle);
        }

        public bool Equals(hkbHandle? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_frame is null && other.m_frame is null) || (m_frame is not null && other.m_frame is not null && m_frame.Equals((IHavokObject)other.m_frame))) &&
                   ((m_rigidBody is null && other.m_rigidBody is null) || (m_rigidBody is not null && other.m_rigidBody is not null && m_rigidBody.Equals((IHavokObject)other.m_rigidBody))) &&
                   ((m_character is null && other.m_character is null) || (m_character is not null && other.m_character is not null && m_character.Equals((IHavokObject)other.m_character))) &&
                   m_animationBoneIndex.Equals(other.m_animationBoneIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_frame);
            hashcode.Add(m_rigidBody);
            hashcode.Add(m_character);
            hashcode.Add(m_animationBoneIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

