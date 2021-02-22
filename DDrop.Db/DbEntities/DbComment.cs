using System;
using System.ComponentModel.DataAnnotations;

namespace DDrop.Db.DbEntities
{
    public class DbComment
    {
        [Key] public Guid CommentId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
    }
}