using System;
using System.Threading.Tasks;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.DAL;
using DDrop.Db.DbEntities;

namespace DDrop.BL.Comments
{
    public class CommentsBL : ICommentsBL
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly IMapper _mapper;

        public CommentsBL(IDDropRepository dDropRepository, IMapper mapper)
        {
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task UpdateComment(Comment comment, Guid entityId)
        {
            await _dDropRepository.UpdateComment(_mapper.Map<Comment, DbComment>(comment), entityId);
        }
    }
}