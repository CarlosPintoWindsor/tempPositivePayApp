
namespace positive_pay_app.app.jobs
{
    internal interface IProcessorJob
    {
        public void ProcessFile(FileStream fileStream, string fileName);
        public string TransformFileName(string fileName);
    }
}
