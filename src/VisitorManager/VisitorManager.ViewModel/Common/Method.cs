using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NLog;
using ThriftCommon;

namespace VisitorManager.ViewModel
{
    public static class Method
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static string UploadImg(string bimgAddress, byte[] imgBytes)
        {
            using (WebClient web = new WebClient())
            {
                byte[] bytetmp = web.UploadData(new Uri(string.Format("{0}/UpLoadFile?dest=LOC&filetype=jpg", bimgAddress)), imgBytes);
                UploadResult res = JsonConvert.DeserializeObject<UploadResult>(System.Text.Encoding.Default.GetString(bytetmp));
                if (res.errorcode == "0")
                {
                    return string.Format("{0}/DownLoadFile?filename={1}", bimgAddress, res.filename);
                }
                else
                {
                    return "";
                }
            }
        }

        struct UploadResult
        {
            public string msg { get; set; }
            public string errorcode { get; set; }
            public string filename { get; set; }
        }



        /// <summary>
        /// 绑定树
        /// </summary>
       static List<TreeNode>  Bind(List<TreeNode> nodes)
        {
            List<TreeNode> outputList = new List<TreeNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ParentID == "-1")
                {
                    outputList.Add(nodes[i]);
                }
                else
                {
                    var nd = FindDownward(nodes, nodes[i].ParentID);
                    if (nd != null) nd.Nodes.Add(nodes[i]);
                }
            }
            return outputList;
        }
        /// <summary>
        /// 递归向下查找
        /// </summary>
       static TreeNode FindDownward(List<TreeNode> nodes, string id)
        {
            if (nodes == null) return null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID == id)
                {
                    return nodes[i];
                }
                TreeNode node = FindDownward(nodes[i].Nodes, id);
                if (node != null)
                {
                    return node;
                }
            }
            return null;
        }

        public static void AddRange<T>(this ObservableCollection<T> dst, IEnumerable<T> source)
        {
            for (int i = 0; i < source.Count(); i++)
            {
                dst.Add(source.ElementAt(i));
            }
        }

        public static List<TreeNode> Bindings(IEnumerable<TreeNode> items)
        {
            Dictionary<string, TreeNode> _treeNodes = new Dictionary<string, TreeNode>();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.ID))
                {
                    try
                    {
                        if (!_treeNodes.ContainsKey(item.ID))
                        {
                            _treeNodes.Add(item.ID, item);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                }
            }

            List<TreeNode> dataList = new List<TreeNode>();
            foreach (var id in _treeNodes.Keys)
            {
                try
                {
                    var node = _treeNodes[id];
                    if (node != null)
                    {
                        var parentId = node.ParentID;
                        if (string.IsNullOrEmpty(parentId))
                        {
                            //logger.Info("ParentId is null");
                        }
                        if (!string.IsNullOrEmpty(parentId) && _treeNodes.Keys.Contains(parentId))
                        {
                            var parentNode = _treeNodes[parentId];
                            parentNode.Nodes.Add(node);
                        }
                        else
                        {
                            dataList.Add(node);
                        }
                    }
                    else
                    {
                        logger.Info("node is null");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }

            }
            return dataList;
        }

        public static List<TreeNode> Bindings(IEnumerable<TreeNode> items, string condition)
        {
            Dictionary<string, TreeNode> _treeNodes = new Dictionary<string, TreeNode>();
            foreach (var item in items)
            {
                try
                {
                    if (!item.Name.Contains(condition) && !string.IsNullOrEmpty(condition))
                        continue;

                    if (!string.IsNullOrEmpty(item.ID))
                    {
                        if (!_treeNodes.ContainsKey(item.ID))
                        {
                            _treeNodes.Add(item.ID, item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            List<TreeNode> dataList = new List<TreeNode>();
            foreach (var id in _treeNodes.Keys)
            {
                try
                {
                    var node = _treeNodes[id];

                    var parentId = node.ParentID;

                    if (!string.IsNullOrEmpty(parentId) && _treeNodes.Keys.Contains(parentId))
                    {
                        var parentNode = _treeNodes[parentId];

                        parentNode.Nodes.Add(node);
                    }
                    else
                    {
                        dataList.Add(node);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
              
            }
            return dataList;
        }
    }

}
