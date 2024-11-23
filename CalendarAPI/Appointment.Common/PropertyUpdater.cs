using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Common;

public class PropertyUpdater<TProperty>
{
    public bool NeedsUpdate { get; }
    public TProperty NewValue { get; }

    public static implicit operator TProperty(PropertyUpdater<TProperty> updater)
    {
        return updater.NewValue;
    }
    public static implicit operator PropertyUpdater<TProperty>(TProperty property)
    {
        return new PropertyUpdater<TProperty>(property);
    }
    public static PropertyUpdater<TProperty> Keep
    {
        get { return new PropertyUpdater<TProperty>(); }
    }
    public PropertyUpdater<TNew> CastValue<TNew>(Func<TProperty, TNew> transform)
    {
        return NeedsUpdate ? new PropertyUpdater<TNew>(true, transform(this.NewValue)) : PropertyUpdater<TNew>.Keep;
    }
    public PropertyUpdater()
            : this(false, default)
    {
    }

    public PropertyUpdater(TProperty newValue)
        : this(true, newValue)
    {
    }

    private PropertyUpdater(bool needsUpdate, TProperty newValue)
    {
        NeedsUpdate = needsUpdate;
        NewValue = newValue;
    }
}
