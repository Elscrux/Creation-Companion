using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkaSkeleton Signatire: 0x366e8220 size: 120 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_parentIndices m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_bones m_class: hkaBone Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_referencePose m_class:  Type.TYPE_ARRAY Type.TYPE_QSTRANSFORM arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_referenceFloats m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_floatSlots m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_localFrames m_class: hkaSkeletonLocalFrameOnBone Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    public partial class hkaSkeleton : hkReferencedObject, IEquatable<hkaSkeleton?>
    {
        public string m_name { set; get; } = "";
        public IList<short> m_parentIndices { set; get; } = Array.Empty<short>();
        public IList<hkaBone> m_bones { set; get; } = Array.Empty<hkaBone>();
        public IList<Matrix4x4> m_referencePose { set; get; } = Array.Empty<Matrix4x4>();
        public IList<float> m_referenceFloats { set; get; } = Array.Empty<float>();
        public IList<string> m_floatSlots { set; get; } = Array.Empty<string>();
        public IList<hkaSkeletonLocalFrameOnBone> m_localFrames { set; get; } = Array.Empty<hkaSkeletonLocalFrameOnBone>();

        public override uint Signature { set; get; } = 0x366e8220;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_parentIndices = des.ReadInt16Array(br);
            m_bones = des.ReadClassArray<hkaBone>(br);
            m_referencePose = des.ReadQSTransformArray(br);
            m_referenceFloats = des.ReadSingleArray(br);
            m_floatSlots = des.ReadStringPointerArray(br);
            m_localFrames = des.ReadClassArray<hkaSkeletonLocalFrameOnBone>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            s.WriteInt16Array(bw, m_parentIndices);
            s.WriteClassArray(bw, m_bones);
            s.WriteQSTransformArray(bw, m_referencePose);
            s.WriteSingleArray(bw, m_referenceFloats);
            s.WriteStringPointerArray(bw, m_floatSlots);
            s.WriteClassArray(bw, m_localFrames);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_name = xd.ReadString(xe, nameof(m_name));
            m_parentIndices = xd.ReadInt16Array(xe, nameof(m_parentIndices));
            m_bones = xd.ReadClassArray<hkaBone>(xe, nameof(m_bones));
            m_referencePose = xd.ReadQSTransformArray(xe, nameof(m_referencePose));
            m_referenceFloats = xd.ReadSingleArray(xe, nameof(m_referenceFloats));
            m_floatSlots = xd.ReadStringArray(xe, nameof(m_floatSlots));
            m_localFrames = xd.ReadClassArray<hkaSkeletonLocalFrameOnBone>(xe, nameof(m_localFrames));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteNumberArray(xe, nameof(m_parentIndices), m_parentIndices);
            xs.WriteClassArray(xe, nameof(m_bones), m_bones);
            xs.WriteQSTransformArray(xe, nameof(m_referencePose), m_referencePose);
            xs.WriteFloatArray(xe, nameof(m_referenceFloats), m_referenceFloats);
            xs.WriteStringArray(xe, nameof(m_floatSlots), m_floatSlots);
            xs.WriteClassArray(xe, nameof(m_localFrames), m_localFrames);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaSkeleton);
        }

        public bool Equals(hkaSkeleton? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_name == other.m_name &&
                   m_parentIndices.SequenceEqual(other.m_parentIndices) &&
                   m_bones.SequenceEqual(other.m_bones) &&
                   m_referencePose.SequenceEqual(other.m_referencePose) &&
                   m_referenceFloats.SequenceEqual(other.m_referenceFloats) &&
                   m_floatSlots.SequenceEqual(other.m_floatSlots) &&
                   m_localFrames.SequenceEqual(other.m_localFrames) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_name);
            hashcode.Add(m_parentIndices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_bones.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_referencePose.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_referenceFloats.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_floatSlots.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_localFrames.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

