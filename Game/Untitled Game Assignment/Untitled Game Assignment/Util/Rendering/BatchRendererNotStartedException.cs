namespace Util.Rendering
{
    public class BatchRendererNotStartedException : BatchRendererException
    {
        public BatchRendererNotStartedException():base("The Begin Function was never called, before end was called")
        {}
    }
}
