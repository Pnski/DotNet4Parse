namespace DotNet4Parse
{
    // TreeNode class should be in the same namespace
    public class TreeNode
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, TreeNode> Children { get; set; } = new Dictionary<string, TreeNode>();
    }
}