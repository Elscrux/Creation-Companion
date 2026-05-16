using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbCharacterSteppedInfo Signatire: 0x2eda84f8 size: 112 flags: FLAGS_NONE

    // m_characterId m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_deltaTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_worldFromModel m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_poseModelSpace m_class:  Type.TYPE_ARRAY Type.TYPE_QSTRANSFORM arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_rigidAttachmentTransforms m_class:  Type.TYPE_ARRAY Type.TYPE_QSTRANSFORM arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    public partial class hkbCharacterSteppedInfo : hkReferencedObject, IEquatable<hkbCharacterSteppedInfo?>
    {
        public ulong m_characterId { set; get; }
        public float m_deltaTime { set; get; }
        public Matrix4x4 m_worldFromModel { set; get; }
        public IList<Matrix4x4> m_poseModelSpace { set; get; } = Array.Empty<Matrix4x4>();
        public IList<Matrix4x4> m_rigidAttachmentTransforms { set; get; } = Array.Empty<Matrix4x4>();

        public override uint Signature { set; get; } = 0x2eda84f8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_deltaTime = br.ReadSingle();
            br.Position += 4;
            m_worldFromModel = des.ReadQSTransform(br);
            m_poseModelSpace = des.ReadQSTransformArray(br);
            m_rigidAttachmentTransforms = des.ReadQSTransformArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_characterId);
            bw.WriteSingle(m_deltaTime);
            bw.Position += 4;
            s.WriteQSTransform(bw, m_worldFromModel);
            s.WriteQSTransformArray(bw, m_poseModelSpace);
            s.WriteQSTransformArray(bw, m_rigidAttachmentTransforms);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_characterId = xd.ReadUInt64(xe, nameof(m_characterId));
            m_deltaTime = xd.ReadSingle(xe, nameof(m_deltaTime));
            m_worldFromModel = xd.ReadQSTransform(xe, nameof(m_worldFromModel));
            m_poseModelSpace = xd.ReadQSTransformArray(xe, nameof(m_poseModelSpace));
            m_rigidAttachmentTransforms = xd.ReadQSTransformArray(xe, nameof(m_rigidAttachmentTransforms));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_characterId), m_characterId);
            xs.WriteFloat(xe, nameof(m_deltaTime), m_deltaTime);
            xs.WriteQSTransform(xe, nameof(m_worldFromModel), m_worldFromModel);
            xs.WriteQSTransformArray(xe, nameof(m_poseModelSpace), m_poseModelSpace);
            xs.WriteQSTransformArray(xe, nameof(m_rigidAttachmentTransforms), m_rigidAttachmentTransforms);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCharacterSteppedInfo);
        }

        public bool Equals(hkbCharacterSteppedInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_characterId.Equals(other.m_characterId) &&
                   m_deltaTime.Equals(other.m_deltaTime) &&
                   m_worldFromModel.Equals(other.m_worldFromModel) &&
                   m_poseModelSpace.SequenceEqual(other.m_poseModelSpace) &&
                   m_rigidAttachmentTransforms.SequenceEqual(other.m_rigidAttachmentTransforms) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_characterId);
            hashcode.Add(m_deltaTime);
            hashcode.Add(m_worldFromModel);
            hashcode.Add(m_poseModelSpace.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_rigidAttachmentTransforms.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

