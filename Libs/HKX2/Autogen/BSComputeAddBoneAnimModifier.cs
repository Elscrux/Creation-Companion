using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // BSComputeAddBoneAnimModifier Signatire: 0xa67f8c46 size: 160 flags: FLAGS_NONE

    // m_boneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_translationLSOut m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_rotationLSOut m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_scaleLSOut m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_pSkeletonMemory m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 144 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSComputeAddBoneAnimModifier : hkbModifier, IEquatable<BSComputeAddBoneAnimModifier?>
    {
        public short m_boneIndex { set; get; }
        public Vector4 m_translationLSOut { set; get; }
        public Quaternion m_rotationLSOut { set; get; }
        public Vector4 m_scaleLSOut { set; get; }
        private object? m_pSkeletonMemory { set; get; }

        public override uint Signature { set; get; } = 0xa67f8c46;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_boneIndex = br.ReadInt16();
            br.Position += 14;
            m_translationLSOut = br.ReadVector4();
            m_rotationLSOut = des.ReadQuaternion(br);
            m_scaleLSOut = br.ReadVector4();
            des.ReadEmptyPointer(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt16(m_boneIndex);
            bw.Position += 14;
            bw.WriteVector4(m_translationLSOut);
            s.WriteQuaternion(bw, m_rotationLSOut);
            bw.WriteVector4(m_scaleLSOut);
            s.WriteVoidPointer(bw);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_boneIndex = xd.ReadInt16(xe, nameof(m_boneIndex));
            m_translationLSOut = xd.ReadVector4(xe, nameof(m_translationLSOut));
            m_rotationLSOut = xd.ReadQuaternion(xe, nameof(m_rotationLSOut));
            m_scaleLSOut = xd.ReadVector4(xe, nameof(m_scaleLSOut));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_boneIndex), m_boneIndex);
            xs.WriteVector4(xe, nameof(m_translationLSOut), m_translationLSOut);
            xs.WriteQuaternion(xe, nameof(m_rotationLSOut), m_rotationLSOut);
            xs.WriteVector4(xe, nameof(m_scaleLSOut), m_scaleLSOut);
            xs.WriteSerializeIgnored(xe, nameof(m_pSkeletonMemory));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSComputeAddBoneAnimModifier);
        }

        public bool Equals(BSComputeAddBoneAnimModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_boneIndex.Equals(other.m_boneIndex) &&
                   m_translationLSOut.Equals(other.m_translationLSOut) &&
                   m_rotationLSOut.Equals(other.m_rotationLSOut) &&
                   m_scaleLSOut.Equals(other.m_scaleLSOut) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_boneIndex);
            hashcode.Add(m_translationLSOut);
            hashcode.Add(m_rotationLSOut);
            hashcode.Add(m_scaleLSOut);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

