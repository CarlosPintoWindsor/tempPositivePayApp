using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using positive_pay_app.app.Sftp;

namespace positive_pay_app.app.jobs.impl
{
    class WindsorServiceJob(
        AppConfig appConfig,
        [FromKeyedServices("WindsorSFTP")] SftpService sftpService,
        ILogger logger,
         EmailService emailService
        ) : IProcessorJob
    {

        public void OnFileProcessError(string fileName, string fullPath, string server, Exception? exception = null)
        {
            string serverName = "Windsor Sftp";
            emailService.SendEmail(new EmailService.EmailPayload
            {
                To = appConfig.EmailerConfig.To,
                Subject = $"Failed to Upload file {fileName}",
                HtmlContent = $"Error Upload File <b>{fileName}</b> To {serverName} Sftp Server: <b>{server}</b> <br/>from ChaseGp Shared Directory [ {appConfig.ChaseGPSharedFolder.Path} ] <br/> {$": - {exception?.Message}"}"
            }).Wait();

        }

        public void ProcessFile(FileStream fileStream, string fileName)
        {
            try
            {
                string destinationFolder = appConfig.Jobs["Windsor"].DestinationFolder;
                string finalFileName = this.TransformFileName(fileName);
                sftpService.Connect();

                logger.LogInformation($"Dropping file [{fileName} into middleware sftp");
                sftpService.UploadFile(fileStream, $"{destinationFolder}/{finalFileName}");
            }
            catch (Exception e)
            {
                logger.LogError($"Could not process file {fileName} - {e.Message}");
                OnFileProcessError(fileName, fileStream.Name, sftpService.Configs().Host, e);
            }
            finally
            {
                sftpService.Disconnect();
            }
        }

        public string TransformFileName(string fileName)
        {
            return $"{fileName}_{DateTime.Now.ToString("MM-dd-yyyyHHmm")}";
        }
    }
}
