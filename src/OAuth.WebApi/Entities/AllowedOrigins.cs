//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OAuth.WebApi.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AllowedOrigins
    {
        public int Id { get; set; }
        public string Origin { get; set; }
        public int ClientId { get; set; }
    
        public virtual Client Client { get; set; }
    }
}
