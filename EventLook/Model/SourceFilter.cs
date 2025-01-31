﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EventLook.Model;

public class SourceFilter : FilterBase
{
    public SourceFilter()
    {
        sourceFilters = new ObservableCollection<SourceFilterItem>();
        SourceFilters = new ReadOnlyObservableCollection<SourceFilterItem>(sourceFilters);
    }

    /// <summary>
    /// Collection of the checkboxes to filter by event source
    /// </summary>
    private readonly ObservableCollection<SourceFilterItem> sourceFilters;
    public ReadOnlyObservableCollection<SourceFilterItem> SourceFilters 
    { 
        get; 
        private set; 
    }

    public override void Refresh(IEnumerable<EventItem> events)
    {
        // Make a copy before clearing
        var prevFilters = sourceFilters.Select(f => new SourceFilterItem { Name = f.Name, Selected = f.Selected }).ToList();
        sourceFilters.Clear();

        var distinctSources = events.Select(e => e.Record.ProviderName).Distinct().OrderBy(s => s);
        foreach (var s in distinctSources)
        {
            sourceFilters.Add(new SourceFilterItem
            {
                Name = s,
                Selected = prevFilters.FirstOrDefault(f => f.Name == s)?.Selected ?? true
            });
        }

        Apply();
    }
    public override void Reset()
    {
        RemoveFilter();
        foreach (var sf in SourceFilters)
        {
            sf.Selected = true;
        }
    }

    protected override bool IsFilterMatched(EventItem evt)
    {
        return SourceFilters.Where(sf => sf.Selected).Any(sf => String.Compare(sf.Name, evt.Record.ProviderName) == 0);
    }

    /// <summary>
    /// Selects (Checks) only filter with the given name, and unchecks the other filters.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Returns true if success.</returns>
    public bool SetSingleFilter(string name)
    {
        if (SourceFilters.Any(x => x.Name == name) == false)
            return false;

        foreach (var filterItem in SourceFilters)
        {
            filterItem.Selected = (filterItem.Name == name);
        }
        return true;
    }
    /// <summary>
    /// Unchecks filter with the given name. i.e., Filters out the source.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Returns true if success.</returns>
    public bool UncheckFilter(string name)
    {
        if (SourceFilters.Any(x => x.Name == name) == false)
            return false;

        foreach (var filterItem in SourceFilters)
        {
            if (filterItem.Name == name)
                filterItem.Selected = false;
        }
        return true;
    }
}

public class SourceFilterItem : Monitorable
{
    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value == _name)
                return;

            _name = value;
            NotifyPropertyChanged();
        }
    }

    private bool _selected;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            if (value == _selected)
                return;

            _selected = value;
            NotifyPropertyChanged();
        }
    }
}
