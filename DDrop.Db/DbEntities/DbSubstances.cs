using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("Substances")]
    public class DbSubstances
    {
        [Key] public Guid SubstanceId { get; set; }

        public string CommonName { get; set; }

        public int Id { get; set; }
        public virtual DbSeries Series { get; set; }
    }
}