namespace positive_pay_app
{

    public class SftpDetails
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Directory { get; set; }

    }

    public class ChaseGPSharedFolderDetails
    {
        public string Path { get; set; }
    }

    public class Jobs
    {
        public string DestinationFolder { get; set; }
    }

    public class EmailConfig
    {
        public string From { get; set; }
        public string SendGridApiKey { get; set; }
        public string SubjectPrefix { get; set; }
        public string[] To { get; set; }
    }

    public class AppConfig
    {
        public string Environment { get; set; }
        public SftpDetails ChaseSFTPDetails { get; set; }

        public SftpDetails WindsorSFTPDetails { get; set; }

        public ChaseGPSharedFolderDetails ChaseGPSharedFolder { get; set; }

        public Dictionary<string, Jobs> Jobs { get; set; }

        public EmailConfig EmailerConfig { get; set; }


    }
}