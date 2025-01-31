namespace TypicalTechTools.Models.Repositories
{
    public interface ICommentRepository
    {
        List<Comment> GetAllComments(int id);

        void DeleteSingleComment(int id);

        Comment GetSingleComment(int id);

        void UpdateComment(Comment comment);

        void CreateComment(Comment comment);
    }
}
