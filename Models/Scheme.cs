//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GymProcess.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Scheme
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Scheme()
        {
            this.Tbl_Member = new HashSet<Tbl_Member>();
        }
    
        public int SchemeId { get; set; }
        public string SchemeName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_Member> Tbl_Member { get; set; }
        public virtual Scheme Scheme1 { get; set; }
        public virtual Scheme Scheme2 { get; set; }
    }
}