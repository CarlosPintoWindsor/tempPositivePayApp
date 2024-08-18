using System;
using Microsoft.Extensions.Logging;

namespace positive_pay_app.app.jobs
{
    class ChaseGPPickUpService(
        ILogger logger,
        AppConfig appConfig,
        IEnumerable<IProcessorJob> serviceJobs,
        EmailService emailService
        )
    {
        public void RunFilesMigration()
        {
            logger.LogDebug(@$"Connecting to ChaseGP Shared Folder -> {appConfig.ChaseGPSharedFolder.Path}");
            var exists = Directory.Exists($"{appConfig.ChaseGPSharedFolder.Path}");

            if (!exists)
            {
                logger.LogError("Could not connect to chase GP Shared Folder");
                NotifyAbourErrors("ChaseGP File Processor failed to Read files", $"Could not connect to chase GP Shared Folder - Folder Path {appConfig.ChaseGPSharedFolder.Path}");
                return;
            }

            try
            {
                DirectoryInfo dir = new DirectoryInfo(@$"{appConfig.ChaseGPSharedFolder.Path}");
                FileInfo[] sharedFiles = dir.GetFiles();

                logger.LogInformation("Found {0} files to process ..", sharedFiles.Length);

                foreach (FileInfo file in sharedFiles)
                {
                    logger.LogInformation("Found File: {0} - FullPath {1} ... ", file.Name, file.FullName);
                    try
                    {
                        FileStream fileStream = file.OpenRead();

                        //Running jobs
                        List<Task> tasks = serviceJobs.Select(job => Task.Run(() => job.ProcessFile(fileStream, file.Name))).ToList();
                        Task.WhenAll(tasks).ContinueWith(done =>
                        {
                            fileStream.Dispose();

                        }).Wait();
                    }
                    catch (Exception e)
                    {
                        OnFileOpenError(file.Name, file.FullName, e);
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error Loading  files from ChaseGp Shared Directory: {e.Message}");
                NotifyAbourErrors("ChaseGP File Processor failed to Read files", $"Error Loading  files from ChaseGp Shared Directory[{appConfig.ChaseGPSharedFolder.Path}]: {e.Message}");
            }
        }
        public void OnFileOpenError(string fileName, string fullPath, Exception? exception = null)
        {
            NotifyAbourErrors($"Filed to Open file {fileName} from  ChaseGp Shared Directory",
                $"Error Opening File {fullPath} from ChaseGp Shared Directory[{appConfig.ChaseGPSharedFolder.Path}] {$": - {exception?.Message}"}");
        }

        private void NotifyAbourErrors(string subject, string text)
        {
            emailService.SendEmail(new EmailService.EmailPayload
            {
                To = appConfig.EmailerConfig.To,
                Subject = subject,
                PlainTextContent = text
            }).Wait();

        }
    }
}
