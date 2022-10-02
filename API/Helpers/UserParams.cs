namespace API.Helpers
{
    public class UserParams
    {
        /// <summary>
        /// This is the maximum number of items
        /// we can display per page. In our application
        /// we want to display at maximum 50
        /// </summary>
        private const int MAX_PAGE_SIZE = 50;

        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize 
        {
            get => _pageSize;
            set => _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }

        public string CurrentUsername { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 150;

        public string OrderBy { get; set; } = "lastActive";
    }
}
