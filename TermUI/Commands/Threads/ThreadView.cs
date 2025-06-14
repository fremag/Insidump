using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Core.ObjectTable;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.Threads;

public class ThreadView : ViewBase
{
    private DumpModel DumpModel { get; }
    private readonly ObjectTableView<ClrThreadInfo> tvThreads;
    private readonly ObjectTableSource<ClrThreadInfo> otsThreads;
    private readonly ObjectTableView<ClrStackFrameInfo> tvCallStackFrames;
    private readonly ObjectTableSource<ClrStackFrameInfo> otsCallStackFrames;
    private readonly ObjectTableView<ClrObjectInfo> tvStackObjects;
    private readonly ObjectTableSource<ClrObjectInfo> otsStackObjects;
    
    public ThreadView(DumpModel dumpModel)
    {
        DumpModel = dumpModel;
        
        var clrThreadInfos = DumpModel
            .GetThreadInfos()
            .OrderBy(info => info.Id)
            .ToArray();
        otsThreads = new ObjectTableSource<ClrThreadInfo>(clrThreadInfos);
        tvThreads = new ObjectTableView<ClrThreadInfo>(otsThreads)
        {
            Y = 1  
        };
        
        tvThreads.SelectedCellChanged += OnSelectedRowChanged;
        var filter = new ObjectTableFilter<ClrThreadInfo>(tvThreads, nameof(ClrThreadInfo.Name))
        {
            Width = Dim.Fill()
        };

        otsCallStackFrames = new ObjectTableSource<ClrStackFrameInfo>([]);
        tvCallStackFrames = new ObjectTableView<ClrStackFrameInfo>(otsCallStackFrames)
        {
            Y = 0
        };

        var callStackView = new FrameView
        {
            Title = "Call Stack",
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        callStackView.Add(tvCallStackFrames);
        
        var rightView = new TileView
        {
            Orientation = Orientation.Horizontal,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        var stackView = new FrameView
        {
            Title = "Stack",
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        otsStackObjects = new ObjectTableSource<ClrObjectInfo>([]);
        tvStackObjects = new ObjectTableView<ClrObjectInfo>(otsStackObjects);
        stackView.Add(tvStackObjects);

        rightView.Tiles.First().ContentView?.Add(callStackView);
        rightView.Tiles.Last().ContentView?.Add(stackView);
        
        var mainView = new TileView(2)
        {
            Orientation = Orientation.Vertical,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        mainView.Tiles.First().ContentView?.Add(filter, tvThreads);
        mainView.Tiles.Last().ContentView?.Add(rightView);
        Add([mainView]);
        tvThreads.SetFocus();
        OnSelectedRowChanged(tvThreads, new SelectedCellChangedEventArgs(otsThreads, 0, 0, 0, 0));
    }

    private void OnSelectedRowChanged(object? sender, SelectedCellChangedEventArgs e)
    {
        if (e.NewRow < 0 || e.NewRow >= otsThreads.DisplayValues.Length)
        {
            return;
        }
        
        ClrThreadInfo clrThreadInfo = otsThreads.DisplayValues[e.NewRow];
        var frames = clrThreadInfo
            .ClrStackFrames()
            .Select(frame => new ClrStackFrameInfo(frame))
            .ToArray();
        otsCallStackFrames.Init(frames);
        tvCallStackFrames.NeedsDraw = true;

        var clrObjectInfos = DumpModel.GetStackObjects(clrThreadInfo).Select(clrValue => new ClrObjectInfo(clrValue)).ToArray();
        otsStackObjects.Init(clrObjectInfos);
        tvStackObjects.NeedsDraw = true;
    }
}