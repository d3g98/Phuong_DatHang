//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThaoPhuong.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DQUAY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DQUAY()
        {
            this.TDONHANGs = new HashSet<TDONHANG>();
        }
    
        public string ID { get; set; }
        public string NAME { get; set; }
        public Nullable<int> POSITION { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDONHANG> TDONHANGs { get; set; }
    }
}
