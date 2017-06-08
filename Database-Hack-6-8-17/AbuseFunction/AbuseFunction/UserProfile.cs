using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbuseFunction
{
    [Table(Name = "UserProfile")]
    public class UserProfile
    {
        [Column(Name = "Id", IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public Guid Id { get; set; }
        [Column(Name = "PreferredName", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string PreferredName { get; set; }
        [Column(Name = "FirstName", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string FirstName { get; set; }
        [Column(Name = "LastName", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string LastName { get; set; }
        [Column(Name = "Birthdate", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public DateTime Birthdate { get; set; }
        [Column(Name = "Gender", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Gender { get; set; }
        [Column(Name = "PhoneNumber", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string PhoneNumber { get; set; }
        [Column(Name = "PhoneType", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string PhoneType { get; set; }
        [Column(Name = "TeacherId", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public Guid TeacherId { get; set; }
        [Column(Name = "CompanyId", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public Guid CompanyId { get; set; }
        [Column(Name = "EmailVerified", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public bool? EmailVerified { get; set; }
        [Column(Name = "Email", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Email { get; set; }
    }
}
