namespace WebApi.Models.Settings
{
    public class EmailSenderOptions
    {
        public string FromAddress { get; set; }

        public string FromName { get; set; }

        public string LocalDirectoryPath { get; set; }
    }
}