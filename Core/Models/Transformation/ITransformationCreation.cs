namespace Unity.Cloud.Assets
{
    public interface ITransformationCreation
    {
        WorkflowType WorkflowType { get; }
        string[] InputFilePaths { get; }
    }
}
