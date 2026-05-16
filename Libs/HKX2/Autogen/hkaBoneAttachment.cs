using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkaBoneAttachment Signatire: 0xa8ccd5cf size: 128 flags: FLAGS_NONE

    // m_originalSkeletonName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_boneFromAttachment m_class:  Type.TYPE_MATRIX4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_attachment m_class: hkReferencedObject Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_boneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkaBoneAttachment : hkReferencedObject, IEquatable<hkaBoneAttachment?>
    {
        public string m_originalSkeletonName { set; get; } = "";
        public Matrix4x4 m_boneFromAttachment { set; get; }
        public hkReferencedObject? m_attachment { set; get; }
        public string m_name { set; get; } = "";
        public short m_boneIndex { set; get; }

        public override uint Signature { set; get; } = 0xa8ccd5cf;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_originalSkeletonName = des.ReadStringPointer(br);
            br.Position += 8;
            m_boneFromAttachment = des.ReadMatrix4(br);
            m_attachment = des.ReadClassPointer<hkReferencedObject>(br);
            m_name = des.ReadStringPointer(br);
            m_boneIndex = br.ReadInt16();
            br.Position += 14;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_originalSkeletonName);
            bw.Position += 8;
            s.WriteMatrix4(bw, m_boneFromAttachment);
            s.WriteClassPointer(bw, m_attachment);
            s.WriteStringPointer(bw, m_name);
            bw.WriteInt16(m_boneIndex);
            bw.Position += 14;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_originalSkeletonName = xd.ReadString(xe, nameof(m_originalSkeletonName));
            m_boneFromAttachment = xd.ReadMatrix4(xe, nameof(m_boneFromAttachment));
            m_attachment = xd.ReadClassPointer<hkReferencedObject>(xe, nameof(m_attachment));
            m_name = xd.ReadString(xe, nameof(m_name));
            m_boneIndex = xd.ReadInt16(xe, nameof(m_boneIndex));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_originalSkeletonName), m_originalSkeletonName);
            xs.WriteMatrix4(xe, nameof(m_boneFromAttachment), m_boneFromAttachment);
            xs.WriteClassPointer(xe, nameof(m_attachment), m_attachment);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteNumber(xe, nameof(m_boneIndex), m_boneIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaBoneAttachment);
        }

        public bool Equals(hkaBoneAttachment? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   (m_originalSkeletonName is null && other.m_originalSkeletonName is null || m_originalSkeletonName == other.m_originalSkeletonName || m_originalSkeletonName is null && other.m_originalSkeletonName == "" || m_originalSkeletonName == "" && other.m_originalSkeletonName is null) &&
                   m_boneFromAttachment.Equals(other.m_boneFromAttachment) &&
                   ((m_attachment is null && other.m_attachment is null) || (m_attachment is not null && other.m_attachment is not null && m_attachment.Equals((IHavokObject)other.m_attachment))) &&
                   m_name == other.m_name &&
                   m_boneIndex.Equals(other.m_boneIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_originalSkeletonName);
            hashcode.Add(m_boneFromAttachment);
            hashcode.Add(m_attachment);
            hashcode.Add(m_name);
            hashcode.Add(m_boneIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

