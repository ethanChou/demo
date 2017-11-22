using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManager.ViewModel
{
    public class TreeNode:ViewModelBase
    {
        public TreeNode()
        {
            this.Nodes = new List<TreeNode>();
            this.ParentID = "-1";
        }

        public bool _isShowShort;

        /// <summary>
        /// 0 是单位，1是 员工
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 如果是单位级节点，代表Dep_id
        /// 如果是员工级节点，代表Emp_id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 节点显示名称
        /// </summary>
        public string Name { get; set; }

        public string Telephone { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 代表 Dep_id
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeNode> Nodes { get; set; }

        public bool IsShowShort
        {
            get { return _isShowShort; }
            set
            {
                _isShowShort = value;
                NotifyChange("IsShowShort");
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class TreeNodeCollection : ICollection<TreeNode>, IList<TreeNode>, INotifyPropertyChanged
    {
        public TreeNodeCollection()
        {

        }

        private int _index = -1;
        private int _indexForChilds = -1;

        public List<TreeNode> Childrens
        {
            get
            {
                if (_nodes.Count == 0 || _index < 0) return new List<TreeNode>();
                return _nodes[_index].Nodes;
            }
        }

        public TreeNodeCollection(List<TreeNode> nodes)
        {
            _nodes = nodes;
        }
        public int Count
        {
            get
            {
                return _nodes.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                bool f = value != _index;
                _index = value;
                NotifyChange("Index");
                NotifyChange("Childrens");
                if (f && Childrens.Count < 0) IndexForChilds = -1;
                else
                    IndexForChilds = 0;
            }
        }

        public int IndexForChilds
        {
            get
            {
                return _indexForChilds;
            }

            set
            {
                _indexForChilds = value;
                NotifyChange("IndexForChilds");
            }
        }

        public TreeNode this[int index]
        {
            get
            {
                return _nodes[index];
            }

            set
            {
                _nodes[index] = value;
            }
        }

        public void Add(TreeNode item)
        {
            _nodes.Add(item);
        }

        public void AddRange(List<TreeNode> items)
        {
            if (items != null)
                _nodes.AddRange(items);
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public bool Contains(TreeNode item)
        {
            return _nodes.Contains(item);
        }

        public void CopyTo(TreeNode[] array, int arrayIndex)
        {
            _nodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(TreeNode item)
        {
            return _nodes.Remove(item);
        }

        public IEnumerator<TreeNode> GetEnumerator()
        {
            foreach (var f in _nodes) yield return f;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var f in _nodes) yield return f;
        }

        private List<TreeNode> _nodes = new List<TreeNode>();

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 封装PropertyChanged触发方法，触发指定属性
        /// </summary>
        /// <param name="propertyName">通知的属性名</param>
        public virtual void NotifyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public int IndexOf(TreeNode item)
        {
            return _nodes.IndexOf(item);
        }

        public void Insert(int index, TreeNode item)
        {
            _nodes.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _nodes.RemoveAt(index);
        }
    }

}
