using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace InsanityBot.Utility.Modlogs.Reference
{
    public struct VerbalModlogEntry
    {
        [XmlAttribute]
        public DateTime Time { get; set; }

        [XmlAttribute]
        public String Reason { get; set; }

        public override Boolean Equals(Object obj)
        {
            if (((ModlogEntry)obj).Time == this.Time)
                return true;
            return false;
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Boolean operator ==(VerbalModlogEntry left, VerbalModlogEntry right)
        {
            return left.Equals(right);
        }

        public static Boolean operator !=(VerbalModlogEntry left, VerbalModlogEntry right)
        {
            return !(left == right);
        }
    }
}
