using System.ComponentModel;

namespace LinkGreenODBCUtility.RunnableActivities
{
    public class RunnableResult
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public string Error { get; set; }
        public string InfoMessage { get; set; }
    }

    public interface IRunnableActivity
    {
        RunnableResult Run(BackgroundWorker bw);
    }
}
