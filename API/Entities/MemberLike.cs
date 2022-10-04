namespace API.Entities
{
    public class MemberLike
    {
        public int SourceMemberId { get; set; }

        /// <summary>
        /// This MEMBER liking another member
        /// </summary>
        public Member SourceMember { get; set; }

        public int LikedMemberId { get; set; }

        /// <summary>
        /// this member is liked by another MEMBER
        /// </summary>
        public Member LikedMember { get; set; }
    }
}
