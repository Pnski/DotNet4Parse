namespace DotNet4Parse
{
    public class TreeNode
    {
        public string Name { get; set; }
        public Dictionary<string, TreeNode> Children { get; set; } = new Dictionary<string, TreeNode>();
    }
}