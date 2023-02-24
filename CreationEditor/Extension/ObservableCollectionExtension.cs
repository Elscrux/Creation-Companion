﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using DynamicData;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor.Extension;

public static class ObservableCollectionExtension {
    public static ReadOnlyObservableCollection<TTarget> SelectObservableCollection<TSource, TTarget>(
        this ObservableCollection<TSource> source,
        Expression<Func<TSource, TTarget>> selector,
        IDisposableDropoff disposable)
        where TSource : INotifyPropertyChanged {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return source
            .ToObservableChangeSet()
            .AutoRefresh(selector)
            .ToObservableCollection(selector.Compile(), disposable);
    }

    public static void Apply<T>(this IList<T> source, Change<T> item, IEqualityComparer<T>? equalityComparer = null) {
        var comparer = equalityComparer ?? EqualityComparer<T>.Default;

        switch (item.Reason) {
            case ListChangeReason.Add: {
                var change = item.Item;
                var hasIndex = change.CurrentIndex >= 0;
                if (hasIndex) {
                    source.Insert(change.CurrentIndex, change.Current);
                } else {
                    source.Add(change.Current);
                }

                break;
            }
            case ListChangeReason.AddRange: {
                if (item.Range.Index.IsInRange(0, source.Count - 1)) {
                    source.AddOrInsertRange(item.Range, item.Range.Index);
                } else {
                    source.AddRange(item.Range, 0);
                }
                break;
            }
            case ListChangeReason.Clear: {
                source.Clear();
                break;
            }
            case ListChangeReason.Replace: {
                var change = item.Item;
                if (change.CurrentIndex >= 0 && change.CurrentIndex == change.PreviousIndex) {
                    source[change.CurrentIndex] = change.Current;
                } else {
                    if (change.PreviousIndex == -1) {
                        var index = source.IndexOf(change.Previous.Value, comparer);
                        if (index > -1) {
                            source.RemoveAt(index);
                        }
                    } else {
                        // is this best? or replace + move?
                        source.RemoveAt(change.PreviousIndex);
                    }

                    if (change.CurrentIndex == -1) {
                        source.Add(change.Current);
                    } else {
                        source.Insert(change.CurrentIndex, change.Current);
                    }
                }

                break;
            }
            case ListChangeReason.Refresh: {
                source.RemoveAt(item.Item.CurrentIndex);
                source.Insert(item.Item.CurrentIndex, item.Item.Current);

                break;
            }
            case ListChangeReason.Remove: {
                var change = item.Item;
                if (change.CurrentIndex >= 0) {
                    source.RemoveAt(change.CurrentIndex);
                } else {
                    var index = source.IndexOf(change.Current, comparer);
                    if (index > -1) {
                        source.RemoveAt(index);
                    }
                }

                break;
            }
            case ListChangeReason.RemoveRange: {
                // ignore this case because WhereReasonsAre removes the index [in which case call RemoveMany]
                //// if (item.Range.Index < 0)
                ////    throw new UnspecifiedIndexException("ListChangeReason.RemoveRange should not have an index specified index");
                if (item.Range.Index >= 0 && source is DynamicData.IExtendedList<T> or List<T>) {
                    source.RemoveRange(item.Range.Index, item.Range.Count);
                } else {
                    foreach (var remove in item.Range) {
                        var index = source.IndexOf(remove, comparer);
                        if (index > -1) {
                            source.RemoveAt(index);
                        }
                    }
                }

                break;
            }
            case ListChangeReason.Moved: {
                var change = item.Item;
                if (change.CurrentIndex < 0) {
                    throw new UnspecifiedIndexException("Cannot move as an index was not specified");
                }

                switch (source) {
                    case DynamicData.IExtendedList<T> extendedList:
                        extendedList.Move(change.PreviousIndex, change.CurrentIndex);
                        break;
                    case ObservableCollection<T> observableCollection:
                        observableCollection.Move(change.PreviousIndex, change.CurrentIndex);
                        break;
                    default:
                        // check this works whatever the index is
                        source.RemoveAt(change.PreviousIndex);
                        source.Insert(change.CurrentIndex, change.Current);
                        break;
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void RemoveRange<T>(this IList<T> source, int index, int count) {
        if (source is null) {
            throw new ArgumentNullException(nameof(source));
        }

        switch (source) {
            case List<T> list:
                list.RemoveRange(index, count);
                break;
            case Noggog.IExtendedList<T> list:
                list.RemoveRange(index, count);
                break;
            default:
                throw new NotSupportedException($"Cannot remove range from {source.GetType().FullName}");
        }
    }
}
