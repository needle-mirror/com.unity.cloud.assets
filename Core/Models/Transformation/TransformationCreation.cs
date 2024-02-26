namespace Unity.Cloud.Assets
{
    public class TransformationCreation : ITransformationCreation
    {
        public WorkflowType WorkflowType { get; set; }
        public string[] InputFilePaths { get; set; }
    }
}
