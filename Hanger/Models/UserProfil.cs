//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hanger.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserProfil
    {
        public int Id { get; set; }
        public int Color1Id { get; set; }
        public Nullable<int> Color2Id { get; set; }
        public int SizeId { get; set; }
        public int BrandId { get; set; }
        public int UserId { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual Color Color { get; set; }
        public virtual Color Color1 { get; set; }
        public virtual Size Size { get; set; }
        public virtual User User { get; set; }
    }
}
