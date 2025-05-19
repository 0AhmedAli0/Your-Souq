using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class AppUser : IdentityUser
    {    //microsoft.extensions.identity.stores من هنا يمكننا الحصول علي الكلاس الخاص بالمستخدم الذي نحتاج الي الاشتقاق منه 
        //مش هحتاج للباكدج دي infrastructre وانا لو حطيط الكلاس دا في مشروع 
    
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Address? Address { get; set; }
    }
}
