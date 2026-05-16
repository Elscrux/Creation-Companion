using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkSimpleLocalFrame Signatire: 0xe758f63c size: 128 flags: FLAGS_NONE

    // m_transform m_class:  Type.TYPE_TRANSFORM Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_children m_class: hkLocalFrame Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_parentFrame m_class: hkLocalFrame Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 96 flags: NOT_OWNED|FLAGS_NONE enum: 
    // m_group m_class: hkLocalFrameGroup Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkSimpleLocalFrame : hkLocalFrame, IEquatable<hkSimpleLocalFrame?>
    {
        public Matrix4x4 m_transform { set; get; }
        public IList<hkLocalFrame> m_children { set; get; } = Array.Empty<hkLocalFrame>();
        public hkLocalFrame? m_parentFrame { set; get; }
        public hkLocalFrameGroup? m_group { set; get; }
        public string m_name { set; get; } = "";

        public override uint Signature { set; get; } = 0xe758f63c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transform = des.ReadTransform(br);
            m_children = des.ReadClassPointerArray<hkLocalFrame>(br);
            m_parentFrame = des.ReadClassPointer<hkLocalFrame>(br);
            m_group = des.ReadClassPointer<hkLocalFrameGroup>(br);
            m_name = des.ReadStringPointer(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteTransform(bw, m_transform);
            s.WriteClassPointerArray(bw, m_children);
            s.WriteClassPointer(bw, m_parentFrame);
            s.WriteClassPointer(bw, m_group);
            s.WriteStringPointer(bw, m_name);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_transform = xd.ReadTransform(xe, nameof(m_transform));
            m_children = xd.ReadClassPointerArray<hkLocalFrame>(xe, nameof(m_children));
            m_parentFrame = xd.ReadClassPointer<hkLocalFrame>(xe, nameof(m_parentFrame));
            m_group = xd.ReadClassPointer<hkLocalFrameGroup>(xe, nameof(m_group));
            m_name = xd.ReadString(xe, nameof(m_name));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteTransform(xe, nameof(m_transform), m_transform);
            xs.WriteClassPointerArray(xe, nameof(m_children), m_children);
            xs.WriteClassPointer(xe, nameof(m_parentFrame), m_parentFrame);
            xs.WriteClassPointer(xe, nameof(m_group), m_group);
            xs.WriteString(xe, nameof(m_name), m_name);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkSimpleLocalFrame);
        }

        public bool Equals(hkSimpleLocalFrame? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_transform.Equals(other.m_transform) &&
                   m_children.SequenceEqual(other.m_children) &&
                   ((m_parentFrame is null && other.m_parentFrame is null) || (m_parentFrame is not null && other.m_parentFrame is not null && m_parentFrame.Equals((IHavokObject)other.m_parentFrame))) &&
                   ((m_group is null && other.m_group is null) || (m_group is not null && other.m_group is not null && m_group.Equals((IHavokObject)other.m_group))) &&
                   m_name == other.m_name &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_transform);
            hashcode.Add(m_children.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_parentFrame);
            hashcode.Add(m_group);
            hashcode.Add(m_name);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

