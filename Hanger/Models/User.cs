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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Ad = new HashSet<Ad>();
            this.ForumPosts = new HashSet<ForumPosts>();
            this.ForumTopic = new HashSet<ForumTopic>();
            this.Message = new HashSet<Message>();
            this.Message1 = new HashSet<Message>();
            this.Favourite = new HashSet<Favourite>();
            this.UserProfil = new HashSet<UserProfil>();
        }
        public int Id { get; set; }
        [DisplayName("Nazwa profilu")]
        [Required(ErrorMessage = "Prosz� wprowad� nazw� profilu", AllowEmptyStrings = false)]
        [Remote("IsProfilNameAvailable", "Register", HttpMethod = "POST", ErrorMessage = "Profil o podanej nazwie istnieje. Prosz� wprowad� inn� nazw�")]
        public string Profil_name { get; set; }
        [DisplayName("E-mail")]
        [Required(ErrorMessage = "Prosz� wprowad� e-mail", AllowEmptyStrings = false)]
        [Remote("IsMailAvailable", "Register", HttpMethod = "POST", ErrorMessage = "Podany e-mail zosta� wykorzystany")]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Prosz� wprowad� poprawny e-mail")]
        public string Mail { get; set; }
        [Required(ErrorMessage = "Prosz� wprowad� has�o", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Has�o musi zaiera� co najmniej 6 znak�w")]
        [DisplayName("Has�o")]
        public string Password { get; set; }
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Has�a nie zgadzj� si�")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [DisplayName("Powt�rz has�o")]
        public string ConfirmPassword { get; set; }
        public System.DateTime Date_access { get; set; }
        public string Description { get; set; }
        public Nullable<int> UserPhotoId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ad> Ad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForumPosts> ForumPosts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForumTopic> ForumTopic { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Message { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Message1 { get; set; }
        public virtual UserPhoto UserPhoto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Favourite> Favourite { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserProfil> UserProfil { get; set; }
    }
}
