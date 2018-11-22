namespace bwoah_shared.DataClasses
{
    public class ChannelData : AData
    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string[] UserNicknames { get; set; }
    }
}
