﻿using System;
using System.Text.RegularExpressions;
using Outliner.Controls.FiltersBase;
using Outliner.Scene;

namespace Outliner.Filters
{
   public class NameFilter : NodeFilter<IMaxNodeWrapper>
{
    public NameFilter() 
    {
        this.SearchString = String.Empty;
        this.CaseSensitive = false;
        this._useWildcard = false;
    }

    private const String SEARCH_BEGINS_WITH = "^";
    private const String SEARCH_WILDCARD = ".";
    private RegexOptions _regExpOptions;
    private String _searchString;
    private String _origSearchString;
    private Boolean _useWildcard;


    public String SearchString 
    {
        get 
        {
            return _origSearchString;
        }
        set
        {
            _origSearchString = value;
            if (value == String.Empty)
                _searchString = value;
            else
            {
                if (this.UseWildcard || value.Substring(0, 1) == "*")
                    _searchString = SEARCH_WILDCARD + Regex.Escape(value.Substring(1, value.Length - 1));
                else
                    _searchString = SEARCH_BEGINS_WITH + Regex.Escape(value);
            }

            this.OnFilterChanged();
        }
    }
    public Boolean CaseSensitive 
    {
        get { return _regExpOptions == RegexOptions.None; }
        set
        {
            if (value)
                _regExpOptions = RegexOptions.None;
            else
                _regExpOptions = RegexOptions.IgnoreCase;

            this.OnFilterChanged();
        }
    }
    public Boolean UseWildcard 
    {
        get { return _useWildcard; }
        set
        {
            _useWildcard = value;
            this.SearchString = _origSearchString;
        }
    }

    override public FilterResult ShowNode(IMaxNodeWrapper data) 
    {
        if (data == null)
            return FilterResult.Hide;

        if (_searchString == String.Empty)
            return FilterResult.Show;

        if (Regex.IsMatch(data.Name, _searchString, _regExpOptions))
            return FilterResult.Show;
        else
            return FilterResult.Hide;
    }
}
}
