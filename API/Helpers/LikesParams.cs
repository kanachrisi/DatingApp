namespace API.Helpers
{
    public class LikesParams : PaginationParams
    {
        public int MemberId { get; set; }

        public string Predicate { get; set; }
    }
}
