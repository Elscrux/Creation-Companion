using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Models.GroupCollection;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.State;
using DynamicData;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Analyzers;
using Mutagen.Bethesda.Analyzers.Drivers;
using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace AnalyzerPlugin.ViewModels;

public sealed record AnalyzerMemento {
    public Severity MinimumSeverity { get; init; } = Severity.Suggestion;
    public List<string> InactiveTopicIds { get; init; } = [];
}

public sealed partial class TopicItem : ReactiveObject {
    public required TopicDefinition TopicDefinition { get; init; }
    public Type? RecordType { get; init; }
    [Reactive] public partial bool IsActive { get; set; } = true;
}

public sealed partial class AnalyzerVM : ViewModel {
    public static readonly IReadOnlyList<Severity> AllSeverities = Enum.GetValues<Severity>();
    public static readonly FuncValueConverter<Severity, IBrush> SeverityToBrushConverter =
        new(severity => severity switch {
            Severity.CTD => Brushes.IndianRed,
            Severity.Error => Brushes.Orange,
            Severity.Warning => Brushes.Yellow,
            Severity.Suggestion => Brushes.CornflowerBlue,
            _ => Brushes.ForestGreen,
        });

    private readonly IEditorEnvironment _editorEnvironment;

    public IObservableCollection<TopicItem> Topics { get; }
    public ReadOnlyObservableCollection<TopicItem> FilteredTopics { get; }
    public GroupCollection<TopicItem> GroupedTopics { get; }
    public Group<TopicItem> TopicsSeverityGroup { get; }
    public Group<TopicItem> TopicsRecordTypeGroup { get; }
    [Reactive] public partial bool IsTopicsGroupedBySeverity { get; set; } = true;
    [Reactive] public partial bool IsTopicsGroupedByRecordType { get; set; } = true;

    public ObservableCollectionExtended<AnalyzerResult> Results { get; } = [];
    public ReadOnlyObservableCollection<AnalyzerResult> FilteredResults { get; }
    public GroupCollection<AnalyzerResult> GroupedResults { get; }
    public Group<AnalyzerResult> ResultsRecordGroup { get; }
    public Group<AnalyzerResult> ResultsSeverityGroup { get; }
    public Group<AnalyzerResult> ResultsTopicGroup { get; }
    [Reactive] public partial bool IsResultsGroupedByRecord { get; set; } = true;
    [Reactive] public partial bool IsResultsGroupedBySeverity { get; set; } = false;
    [Reactive] public partial bool IsResultsGroupedByTopic { get; set; } = false;

    public HierarchicalTreeDataGridSource<object> TopicsTreeSource { get; }
    public HierarchicalTreeDataGridSource<object> ResultsTreeSource { get; }

    [Reactive] public partial Severity MinimumSeverity { get; set; }
    [Reactive] public partial string TopicSearchText { get; set; } = string.Empty;
    [Reactive] public partial string ResultsSearchText { get; set; } = string.Empty;
    public IObservable<int> ActiveTopicCount { get; }

    public ReactiveCommand<Unit, Unit> Analyze { get; }

    public MultiModPickerVM ModPickerVM { get; }

    public AnalyzerVM(
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem,
        IStateRepositoryFactory<AnalyzerMemento, AnalyzerMemento, Guid> stateRepositoryFactory,
        IReadOnlyList<IAnalyzer> analyzers,
        MultiModPickerVM multiModPickerVM) {
        _editorEnvironment = editorEnvironment;
        ModPickerVM = multiModPickerVM;

        var analyzersStateRepo = stateRepositoryFactory.Create("Analyzers");
        var (guid, analyzersState) = analyzersStateRepo.LoadAllWithIdentifier().FirstOrDefault();
        Guid? analyzersStateGuid = analyzersState is null ? null : guid;

        this.WhenAnyValue(x => x.MinimumSeverity)
            .Skip(1)
            .ThrottleLong()
            .Subscribe(severity => {
                analyzersStateRepo.Update(
                    _ => new AnalyzerMemento {
                        MinimumSeverity = severity,
                        InactiveTopicIds = analyzersState?.InactiveTopicIds ?? [],
                    },
                    analyzersStateGuid ??= Guid.NewGuid());
            })
            .DisposeWith(this);

        var topics = analyzers
            .DistinctBy(x => x.Id)
            .SelectMany(analyzer => {
                var genericArguments = analyzer.GetType().GetInterfaces().Select(i => i.GetGenericArguments()).FirstOrDefault(x => x.Length != 0);
                var type = genericArguments?.Length > 0 ? genericArguments[0] : null;

                return analyzer.Topics.Select(t => new TopicItem {
                    TopicDefinition = t,
                    RecordType = type,
                    IsActive = analyzersState is null || !analyzersState.InactiveTopicIds.Contains(t.Id.ToString()),
                });
            })
            .OrderBy(x => x.TopicDefinition.Id.ToString())
            .ToArray();

        Topics = new ObservableCollectionExtended<TopicItem>(topics);

        FilteredTopics = Topics.ToObservableChangeSet<IObservableCollection<TopicItem>, TopicItem>()
            .Filter(this.WhenAnyValue(x => x.TopicSearchText)
                .Select(searchText => new Func<TopicItem, bool>(topic =>
                    searchText.IsNullOrWhitespace()
                 || topic.TopicDefinition.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))))
            .ToObservableCollection(this);

        TopicsSeverityGroup = new Group<TopicItem>(t => t.TopicDefinition.Severity, IsTopicsGroupedBySeverity);
        TopicsRecordTypeGroup = new Group<TopicItem>(t => t.RecordType, IsTopicsGroupedByRecordType);

        this.WhenAnyValue(x => x.IsTopicsGroupedBySeverity)
            .Subscribe(x => TopicsSeverityGroup.IsGrouped = TopicSearchText.IsNullOrEmpty() && x)
            .DisposeWith(this);

        this.WhenAnyValue(x => x.IsTopicsGroupedByRecordType)
            .Subscribe(x => TopicsRecordTypeGroup.IsGrouped = TopicSearchText.IsNullOrEmpty() && x)
            .DisposeWith(this);

        this.WhenAnyValue(x => x.TopicSearchText)
            .CombineLatest(FilteredTopics.WhenCollectionChanges(), (searchText, _) => searchText)
            .Subscribe(searchText => {
                // This is done to work around issues with using a filtered collection in grouped collections
                if (searchText.IsNullOrEmpty()) {
                    TopicsSeverityGroup.IsGrouped = IsTopicsGroupedBySeverity;
                    TopicsRecordTypeGroup.IsGrouped = IsTopicsGroupedByRecordType;
                } else {
                    TopicsSeverityGroup.IsGrouped = false;
                    TopicsRecordTypeGroup.IsGrouped = false;
                }
            });

        var activeTopicsChanged = Topics
            .ToObservableChangeSet<IObservableCollection<TopicItem>, TopicItem>()
            .AutoRefresh(x => x.IsActive)
            .ThrottleLong()
            .ToCollection();

        ActiveTopicCount = activeTopicsChanged
            .Select(x => x.Count(item => item.IsActive));

        activeTopicsChanged
            .Skip(1)
            .Subscribe(t => {
                analyzersStateRepo.Update(
                    _ => new AnalyzerMemento {
                        MinimumSeverity = MinimumSeverity,
                        InactiveTopicIds = t
                            .Where(x => !x.IsActive)
                            .Select(x => x.TopicDefinition.Id.ToString())
                            .Distinct()
                            .ToList(),
                    },
                    analyzersStateGuid ??= Guid.NewGuid());
            })
            .DisposeWith(this);

        GroupedTopics = new GroupCollection<TopicItem>(FilteredTopics, TopicsSeverityGroup, TopicsRecordTypeGroup);

        FilteredResults = Results.ToObservableChangeSet<IObservableCollection<AnalyzerResult>, AnalyzerResult>()
            .Filter(this.WhenAnyValue(x => x.ResultsSearchText)
                .Select(searchText => new Func<AnalyzerResult, bool>(result =>
                    searchText.IsNullOrWhitespace()
                 || result.Topic.FormattedTopic.TopicDefinition.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                 || result.Topic.FormattedTopic.FormattedMessage.Contains(searchText, StringComparison.OrdinalIgnoreCase))))
            .ToObservableCollection(this);

        ResultsRecordGroup = new Group<AnalyzerResult>(result => result.Record, IsResultsGroupedByRecord);
        ResultsSeverityGroup = new Group<AnalyzerResult>(result => result.Topic.Severity, IsResultsGroupedBySeverity);
        ResultsTopicGroup = new Group<AnalyzerResult>(result => result.Topic.TopicDefinition, IsResultsGroupedByTopic);
        this.WhenAnyValue(x => x.IsResultsGroupedByRecord)
            .Subscribe(x => ResultsRecordGroup.IsGrouped = ResultsSearchText.IsNullOrEmpty() && x)
            .DisposeWith(this);

        this.WhenAnyValue(x => x.IsResultsGroupedBySeverity)
            .Subscribe(x => ResultsSeverityGroup.IsGrouped = ResultsSearchText.IsNullOrEmpty() && x)
            .DisposeWith(this);

        this.WhenAnyValue(x => x.IsResultsGroupedByTopic)
            .Subscribe(x => ResultsTopicGroup.IsGrouped = ResultsSearchText.IsNullOrEmpty() && x)
            .DisposeWith(this);

        this.WhenAnyValue(x => x.ResultsSearchText)
            .CombineLatest(FilteredResults.WhenCollectionChanges(), (searchText, _) => searchText)
            .Subscribe(searchText => {
                // This is done to work around issues with using a filtered collection in grouped collections
                if (searchText.IsNullOrEmpty()) {
                    ResultsRecordGroup.IsGrouped = IsResultsGroupedByRecord;
                    ResultsSeverityGroup.IsGrouped = IsResultsGroupedBySeverity;
                    ResultsTopicGroup.IsGrouped = IsResultsGroupedByTopic;
                } else {
                    ResultsRecordGroup.IsGrouped = false;
                    ResultsSeverityGroup.IsGrouped = false;
                    ResultsTopicGroup.IsGrouped = false;
                }
            });

        GroupedResults = new GroupCollection<AnalyzerResult>(FilteredResults, ResultsRecordGroup, ResultsSeverityGroup, ResultsTopicGroup);

        var setTopics = ReactiveCommand.Create<GroupInstance>(SetTopics);
        TopicsTreeSource = new HierarchicalTreeDataGridSource<object>(GroupedTopics.Items) {
            Columns = {
                new HierarchicalExpanderColumn<object>(
                    new TemplateColumn<object>(
                        string.Empty,
                        new FuncDataTemplate<object>((item, _) => {
                            return item switch {
                                GroupInstance groupInstance => new StackPanel {
                                    Orientation = Orientation.Horizontal,
                                    Children = {
                                        new CheckBox {
                                            [!ToggleButton.IsCheckedProperty] = groupInstance.Items
                                                .WhenCollectionChanges()
                                                .Select(_ => GetAllActive(groupInstance))
                                                .ToBinding(),
                                            Command = setTopics,
                                            CommandParameter = groupInstance,
                                        },
                                        groupInstance.Class switch {
                                            Type recordType => GetRecordTypeControl(recordType),
                                            Severity severity => GetSeverityControl(severity),
                                            null => new TextBlock {
                                                Text = "All",
                                                VerticalAlignment = VerticalAlignment.Center,
                                            },
                                            _ => new TextBlock {
                                                Text = groupInstance.Class?.ToString(),
                                                VerticalAlignment = VerticalAlignment.Center,
                                            }
                                        }
                                    },
                                },
                                TopicItem => new CheckBox {
                                    [!ToggleButton.IsCheckedProperty] = new Binding(nameof(TopicItem.IsActive))
                                },
                                _ => null
                            };
                        })),
                    x => x switch {
                        GroupInstance groupInstance => groupInstance.Items,
                        _ => null,
                    }
                ),
                new TemplateColumn<object>(
                    "Topic",
                    new FuncDataTemplate<object>((item, _) => {
                        return item switch {
                            TopicItem result => GetTopicControl(result.TopicDefinition),
                            _ => null,
                        };
                    })),
                new TemplateColumn<object>(
                    "Type",
                    new FuncDataTemplate<object>((item, _) => {
                        return item switch {
                            TopicItem result => GetRecordTypeControl(result.RecordType),
                            _ => null,
                        };
                    })),
            }
        };

        ResultsTreeSource = new HierarchicalTreeDataGridSource<object>(GroupedResults.Items) {
            Columns = {
                new HierarchicalExpanderColumn<object>(
                    new TemplateColumn<object>(
                        "Record",
                        new FuncDataTemplate<object>((item, _) => item switch {
                            GroupInstance groupInstance => groupInstance.Class switch {
                                TopicDefinition topicDefinition => GetTopicControl(topicDefinition),
                                IFormLinkIdentifier formLinkIdentifier => GetRecordControl(formLinkIdentifier),
                                Severity severity => GetSeverityControl(severity),
                                _ => new TextBox {
                                    Text = groupInstance.Class?.ToString(),
                                    VerticalAlignment = VerticalAlignment.Center,
                                }
                            },
                            AnalyzerResult result => GetRecordControl(result.Record),
                            _ => null
                        })),
                    x => x switch {
                        GroupInstance groupInstance => groupInstance.Items,
                        _ => null,
                    }
                ),
                new TemplateColumn<object>(
                    "Topic",
                    new FuncDataTemplate<object>((item, _) => {
                        return item switch {
                            AnalyzerResult result => GetTopicControl(result.Topic.TopicDefinition),
                            _ => null,
                        };
                    })),
                new TemplateColumn<object>(
                    "Severity",
                    new FuncDataTemplate<object>((item, _) => {
                        return item switch {
                            AnalyzerResult result => GetSeverityControl(result.Topic.Severity),
                            _ => null,
                        };
                    })),
                new TemplateColumn<object>(
                    "Message",
                    new FuncDataTemplate<object>((item, _) => {
                        return item switch {
                            AnalyzerResult result => GetFormattedTopicControl(result.Topic.FormattedTopic),
                            _ => null,
                        };
                    })),
            }
        };

        Analyze = ReactiveCommand.CreateFromTask(() => Task.Run(async () => {
                var selectedMods = ModPickerVM.GetSelectedMods();
                var notSelectedMods = _editorEnvironment.LinkCache.ListedOrder
                    .Select(mod => mod.ModKey)
                    .Except(selectedMods.Select(mod => mod.ModKey));

                var selectedTopicIds = Topics
                    .Where(t => t.IsActive)
                    .Select(t => t.TopicDefinition.Id)
                    .ToHashSet();

                var selectedAnalyzers = analyzers
                    .Where(a => a.Topics.Any(t => selectedTopicIds.Contains(t.Id)))
                    .ToList();

                var analyzer = AnalyzerRunnerBuilder.Create(GameRelease.SkyrimSE)
                    .WithLinkCache(_editorEnvironment.LinkCache)
                    .WithAnalyzers(selectedAnalyzers)
                    .WithBlacklistedMods(notSelectedMods)
                    .WithMinimumSeverity(MinimumSeverity)
                    .WithFileSystem(fileSystem)
                    .Build();

                var resultsObservable = analyzer.Analyze()
                    .ToObservable()
                    .Publish()
                    .RefCount();

                var disposableBucket = new DisposableBucket();

                Dispatcher.UIThread.Post(void () => {
                    Results.Clear();

                    // Add results in batches to avoid UI freezing
                    resultsObservable
                        .Where(result => selectedTopicIds.Contains(result.Topic.TopicDefinition.Id))
                        .Subscribe(result => Results.Add(result))
                        // ReSharper disable once AccessToDisposedClosure - dispose called after await
                        .DisposeWith(disposableBucket);
                });

                await resultsObservable;
                disposableBucket.Dispose();
            }),
            ModPickerVM.SelectedMods.Any());
    }

    private bool? GetAllActive(GroupInstance group) {
        var activeCount = group.Items
            .Count(x => x switch {
                GroupInstance groupInstance => GetAllActive(groupInstance) is true,
                TopicItem topicItem => topicItem.IsActive,
            });

        if (activeCount == 0) return false;
        if (activeCount == group.Items.Count) return true;

        return null;
    }

    private void SetTopics(GroupInstance group) {
        var allActive = GetAllActive(group);

        if (allActive is false) {
            SetTopics(group, true);
        } else {
            SetTopics(group, false);
        }
    }

    private void SetTopics(GroupInstance group, bool newState) {
        foreach (var item in group.Items) {
            switch (item) {
                case GroupInstance g:
                    SetTopics(g, newState);
                    break;
                case TopicItem topicItem:
                    topicItem.IsActive = newState;
                    break;
            }
        }
    }

    private Control GetTopicControl(TopicDefinition topicDefinition) {
        return new StackPanel {
            Spacing = 5,
            Orientation = Orientation.Horizontal,
            Children = {
                new Button {
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = null,
                    BorderBrush = null,
                    Padding = new Thickness(0, 0),
                    Content = new SymbolIcon {
                        Symbol = Symbol.Open,
                        Foreground = StandardBrushes.HighlightBrush,
                    },
                    Command = ReactiveCommand.Create(() => {
                        Process.Start(new ProcessStartInfo {
                            FileName = topicDefinition.InformationUri?.AbsoluteUri,
                            UseShellExecute = true,
                            Verb = "open",
                        });
                    })
                },
                new TextBlock {
                    Text = topicDefinition.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                },
            }
        };
    }

    private Control GetRecordControl(IFormLinkIdentifier? formLinkIdentifier) {
        // TODO add button to open record
        return new TextBlock {
            Text = formLinkIdentifier is not null && _editorEnvironment.LinkCache.TryResolve(formLinkIdentifier, out var record)
                ? record.GetHumanReadableName()
                : formLinkIdentifier?.FormKey.ToString() ?? "Unknown Record",
            VerticalAlignment = VerticalAlignment.Center,
        };
    }

    private Control GetRecordTypeControl(Type? type) {
        return new TextBlock {
            Text = type?.Name[1..^6] ?? "All",
            VerticalAlignment = VerticalAlignment.Center,
        };
    }

    private Control GetSeverityControl(Severity severity) {
        return new TextBlock {
            Text = severity.ToString(),
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = SeverityToBrushConverter.Convert(severity),
        };
    }

    private Control GetFormattedTopicControl(IFormattedTopicDefinition formattedTopic) {
        // TODO add clickable buttons for items (records, files etc) in message (use spans to separate text and links)
        return new TextBlock {
            Text = formattedTopic.FormattedMessage,
            VerticalAlignment = VerticalAlignment.Center,
        };
    }
}
