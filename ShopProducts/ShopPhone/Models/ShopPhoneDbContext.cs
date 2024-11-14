using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ShopPhone.Models
{
    public class ShopPhoneDbContext : DbContext
    {
        public ShopPhoneDbContext() : base("name=ChuoiKN")
        { }
        public virtual DbSet<Mcategory> Categorys { get; set; }
        public virtual DbSet<Mcontact> Contacts { get; set; }
        public virtual DbSet<link> Link { get; set; }
        public virtual DbSet<Mmenu> Menus { get; set; }
        public virtual DbSet<Morder> Orders { get; set; }
        public virtual DbSet<Mordersdetail> Orderdetails { get; set; }
        public virtual DbSet<Mpost> posts { get; set; }
        public virtual DbSet<Mproduct> Products { get; set; }
        public virtual DbSet<Mslider> Sliders { get; set; }
        public virtual DbSet<Mtopic> topics { get; set; }
        public virtual DbSet<Muser> users { get; set; }
        public virtual DbSet<role> roles { get; set; }
        internal int ExecuteScalar()
        {
            throw new NotImplementedException();
        }
    }
}
