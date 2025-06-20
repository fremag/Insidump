﻿using Insidump.Core.View;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Insidump.Core.ObjectTable;

public class ObjectTableFilter<T> : ViewBase
{
    public ObjectTableFilter(ObjectTableView<T> objectTableView, string columnName)
    {
        ObjectTableView = objectTableView;
        var lblFilter = new Label { Y = 0, X = 0, Text = $"{columnName}: " };
        var tbFilter = new TextView { Y = 0, X = Pos.Right(lblFilter), Width = Dim.Fill(), Height = 1, Multiline = false, Enabled = true, CanFocus = true };
        tbFilter.ContentsChanged += (_, _) => { ObjectTableView.SetFilter(tbFilter.Text, columnName); };

        Add(lblFilter, tbFilter);
    }

    private ObjectTableView<T> ObjectTableView { get; }
}