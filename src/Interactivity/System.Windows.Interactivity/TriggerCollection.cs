using System;
namespace System.Windows.Interactivity
{
	public sealed class TriggerCollection : AttachableCollection<TriggerBase>
	{
		internal TriggerCollection()
		{
		}
		protected override void OnAttached()
		{
			foreach (TriggerBase current in this)
			{
				current.Attach(base.AssociatedObject);
			}
		}
		protected override void OnDetaching()
		{
			foreach (TriggerBase current in this)
			{
				current.Detach();
			}
		}
		internal override void ItemAdded(TriggerBase item)
		{
			if (base.AssociatedObject != null)
			{
				item.Attach(base.AssociatedObject);
			}
		}
		internal override void ItemRemoved(TriggerBase item)
		{
			if (((IAttachedObject)item).AssociatedObject != null)
			{
				item.Detach();
			}
		}
		protected override Freezable CreateInstanceCore()
		{
			return new TriggerCollection();
		}
	}
}
