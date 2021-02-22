using System;
using DDrop.BE.Enums;

namespace DDrop.BE.Models
{
    public class Comment
    {
        public Guid CommentId { get; set; }

        public string Content { get; set; }

        public CommentableEntityType Type { get; set; }
    }
}