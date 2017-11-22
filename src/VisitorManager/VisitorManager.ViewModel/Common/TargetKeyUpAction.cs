using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VisitorManager.ViewModel
{
    public class TargetKeyUpAction : TargetedTriggerAction<TreeView>
    {
        protected override void Invoke(object parameter)
        {
           KeyEventArgs e = parameter as KeyEventArgs;

            //if (e != null && e.Key == Key.Enter)
            {
                var condition = (this.AssociatedObject as TextBox).Text;

                var tree = this.Target;
                IEnumerable<TreeNode> tnc = (IEnumerable<TreeNode>)tree.Tag;

                //List<TreeNode> datas = tree.ItemsSource as List<TreeNode>;
                tree.Dispatcher.BeginInvoke(new Action(()=> {
                    List<TreeNode> dst = Method.Bindings(tnc, condition); ;

                    tree.ItemsSource = dst;

                }));
               
            }

            //this.Target.Text = (this.AssociatedObject as TextBox).Text;
            // this.Target.Background = (this.AssociatedObject as TextBlock).Background;
        }

       
    }
}
