﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("ReferencePhotos")]
    public class DbReferencePhoto
    {
        [Key] public Guid PhotoId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public int PixelsInMillimeter { get; set; }
        public virtual DbSeries Series { get; set; }
        public string ReferenceLine { get; set; }
        public string AddedDate { get; set; }
        public string CreationDateTime { get; set; }
        public string PhotoType { get; set; }
    }
}