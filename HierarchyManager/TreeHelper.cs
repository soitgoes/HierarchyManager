using Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace HierarchyManager
{
    public static class TreeHelper
    {
        public static TreeNode[] Convert(this IEnumerable<NamedItem> input)
        {
            return input.Select(i => {
                var t = new TreeNode(i.Name);
                t.Name = i.Id.ToString();
                t.Nodes.Add(new TreeNode());//Allow for expand
                return t;
            }).ToArray();
        }
    }
}
