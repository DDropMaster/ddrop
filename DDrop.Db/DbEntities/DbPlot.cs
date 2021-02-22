using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("Plots")]
    public class DbPlot
    {
        [Key] public Guid PlotId { get; set; }
        public string Name { get; set; }
        public string Points { get; set; }
        public Guid CurrentUserId { get; set; }
        public DbUser CurrentUser { get; set; }
        public string PlotType { get; set; }
    }
}