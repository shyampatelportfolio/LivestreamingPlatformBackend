namespace LiveStreamingBackend.Data
{
    public class StreamData
    {
        public int TotalViews { get; set; } = 0;
        public int TotalViewers { get; set; } = 0;
        public string? StreamDescription { get; set; } = "Sorry, the user is not currently streaming. Please check back later or explore other content in the meantime.";
        public string? StreamName { get; set; } = "Stream is Offline";
        public string? StreamAuthor { get; set; } = "undefined";
        public bool StreamLive { get; set; } = false;
       
    }
}
