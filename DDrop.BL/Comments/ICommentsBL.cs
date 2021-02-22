using System;
using System.Threading.Tasks;
using DDrop.BE.Models;

namespace DDrop.BL.Comments
{
    public interface ICommentsBL
    {
        Task UpdateComment(Comment comment, Guid entityId);
    }
}