﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppNameTestify.DBModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TestifyEntities : DbContext
    {
        public TestifyEntities()
            : base("name=TestifyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<LandlordHouseDetail> LandlordHouseDetails { get; set; }
        public DbSet<LandlordRegistration> LandlordRegistrations { get; set; }
        public DbSet<TenantRegistration> TenantRegistrations { get; set; }
        public DbSet<TenantTicket> TenantTickets { get; set; }
    }
}
