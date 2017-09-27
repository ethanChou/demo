using System;
using System.ComponentModel;
using System.Globalization;
namespace System.Windows.Interactivity
{
	public abstract class TargetedTriggerAction : TriggerAction
	{
		private Type targetTypeConstraint;
		private bool isTargetChangedRegistered;
		private NameResolver targetResolver;
		public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(object), typeof(TargetedTriggerAction), new FrameworkPropertyMetadata(new PropertyChangedCallback(TargetedTriggerAction.OnTargetObjectChanged)));
		public static readonly DependencyProperty TargetNameProperty = DependencyProperty.Register("TargetName", typeof(string), typeof(TargetedTriggerAction), new FrameworkPropertyMetadata(new PropertyChangedCallback(TargetedTriggerAction.OnTargetNameChanged)));
		public object TargetObject
		{
			get
			{
				return base.GetValue(TargetedTriggerAction.TargetObjectProperty);
			}
			set
			{
				base.SetValue(TargetedTriggerAction.TargetObjectProperty, value);
			}
		}
		public string TargetName
		{
			get
			{
				return (string)base.GetValue(TargetedTriggerAction.TargetNameProperty);
			}
			set
			{
				base.SetValue(TargetedTriggerAction.TargetNameProperty, value);
			}
		}
		protected object Target
		{
			get
			{
				object obj = base.AssociatedObject;
				if (this.TargetObject != null)
				{
					obj = this.TargetObject;
				}
				else
				{
					if (this.IsTargetNameSet)
					{
						obj = this.TargetResolver.Object;
					}
				}
				if (obj != null && !this.TargetTypeConstraint.IsAssignableFrom(obj.GetType()))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.RetargetedTypeConstraintViolatedExceptionMessage, new object[]
					{
						base.GetType().Name,
						obj.GetType(),
						this.TargetTypeConstraint,
						"Target"
					}));
				}
				return obj;
			}
		}
		protected sealed override Type AssociatedObjectTypeConstraint
		{
			get
			{
				AttributeCollection attributes = TypeDescriptor.GetAttributes(base.GetType());
				TypeConstraintAttribute typeConstraintAttribute = attributes[typeof(TypeConstraintAttribute)] as TypeConstraintAttribute;
				if (typeConstraintAttribute != null)
				{
					return typeConstraintAttribute.Constraint;
				}
				return typeof(DependencyObject);
			}
		}
		protected Type TargetTypeConstraint
		{
			get
			{
				base.ReadPreamble();
				return this.targetTypeConstraint;
			}
		}
		private bool IsTargetNameSet
		{
			get
			{
				return !string.IsNullOrEmpty(this.TargetName) || base.ReadLocalValue(TargetedTriggerAction.TargetNameProperty) != DependencyProperty.UnsetValue;
			}
		}
		private NameResolver TargetResolver
		{
			get
			{
				return this.targetResolver;
			}
		}
		private bool IsTargetChangedRegistered
		{
			get
			{
				return this.isTargetChangedRegistered;
			}
			set
			{
				this.isTargetChangedRegistered = value;
			}
		}
		internal TargetedTriggerAction(Type targetTypeConstraint) : base(typeof(DependencyObject))
		{
			this.targetTypeConstraint = targetTypeConstraint;
			this.targetResolver = new NameResolver();
			this.RegisterTargetChanged();
		}
		internal virtual void OnTargetChangedImpl(object oldTarget, object newTarget)
		{
		}
		protected override void OnAttached()
		{
			base.OnAttached();
			DependencyObject associatedObject = base.AssociatedObject;
			Behavior behavior = associatedObject as Behavior;
			this.RegisterTargetChanged();
			if (behavior != null)
			{
				associatedObject = ((IAttachedObject)behavior).AssociatedObject;
				behavior.AssociatedObjectChanged += new EventHandler(this.OnBehaviorHostChanged);
			}
			this.TargetResolver.NameScopeReferenceElement = (associatedObject as FrameworkElement);
		}
		protected override void OnDetaching()
		{
			Behavior behavior = base.AssociatedObject as Behavior;
			base.OnDetaching();
			this.OnTargetChangedImpl(this.TargetResolver.Object, null);
			this.UnregisterTargetChanged();
			if (behavior != null)
			{
				behavior.AssociatedObjectChanged -= new EventHandler(this.OnBehaviorHostChanged);
			}
			this.TargetResolver.NameScopeReferenceElement = null;
		}
		private void OnBehaviorHostChanged(object sender, EventArgs e)
		{
			this.TargetResolver.NameScopeReferenceElement = (((IAttachedObject)sender).AssociatedObject as FrameworkElement);
		}
		private void RegisterTargetChanged()
		{
			if (!this.IsTargetChangedRegistered)
			{
				this.TargetResolver.ResolvedElementChanged += new EventHandler<NameResolvedEventArgs>(this.OnTargetChanged);
				this.IsTargetChangedRegistered = true;
			}
		}
		private void UnregisterTargetChanged()
		{
			if (this.IsTargetChangedRegistered)
			{
				this.TargetResolver.ResolvedElementChanged -= new EventHandler<NameResolvedEventArgs>(this.OnTargetChanged);
				this.IsTargetChangedRegistered = false;
			}
		}
		private static void OnTargetObjectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			TargetedTriggerAction targetedTriggerAction = (TargetedTriggerAction)obj;
			targetedTriggerAction.OnTargetChanged(obj, new NameResolvedEventArgs(args.OldValue, args.NewValue));
		}
		private static void OnTargetNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			TargetedTriggerAction targetedTriggerAction = (TargetedTriggerAction)obj;
			targetedTriggerAction.TargetResolver.Name = (string)args.NewValue;
		}
		private void OnTargetChanged(object sender, NameResolvedEventArgs e)
		{
			if (base.AssociatedObject != null)
			{
				this.OnTargetChangedImpl(e.OldObject, e.NewObject);
			}
		}
	}
	public abstract class TargetedTriggerAction<T> : TargetedTriggerAction where T : class
	{
		protected new T Target
		{
			get
			{
				return (T)((object)base.Target);
			}
		}
		protected TargetedTriggerAction() : base(typeof(T))
		{
		}
		internal sealed override void OnTargetChangedImpl(object oldTarget, object newTarget)
		{
			base.OnTargetChangedImpl(oldTarget, newTarget);
			this.OnTargetChanged(oldTarget as T, newTarget as T);
		}
		protected virtual void OnTargetChanged(T oldTarget, T newTarget)
		{
		}
	}
}
