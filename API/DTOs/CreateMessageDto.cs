namespace API.DTOs
{
    /// <summary>
    /// This Dto is generated on the client side
    /// and contains the message info
    /// </summary>
    public class CreateMessageDto
    {
        public string RecipientUsername { get; set; }

        public string Content { get; set; }
    }
}
