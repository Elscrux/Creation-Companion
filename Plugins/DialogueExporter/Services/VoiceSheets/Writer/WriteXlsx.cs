using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Mutagen.Bethesda.Skyrim;
using Serilog;
using Alignment = DocumentFormat.OpenXml.Spreadsheet.Alignment;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;
using Fill = DocumentFormat.OpenXml.Spreadsheet.Fill;
using Fonts = DocumentFormat.OpenXml.Spreadsheet.Fonts;
using ForegroundColor = DocumentFormat.OpenXml.Spreadsheet.ForegroundColor;
using Path = System.IO.Path;
using PatternFill = DocumentFormat.OpenXml.Spreadsheet.PatternFill;
using TopBorder = DocumentFormat.OpenXml.Spreadsheet.TopBorder;
namespace DialogueExporter.Services.VoiceSheets.Writer;

public sealed class WriteXlsx(
    ILogger logger,
    IEditorEnvironment editorEnvironment,
    IFileSystem fileSystem) {
    private enum BorderStyle {
        None,
        MediumDashed,
        Double,
        Thin,
        Dotted,
    }

    private enum BackgroundColor {
        None,
        Green,
        Blue,
    }

    private enum VerticalAlignment {
        Top,
        Center,
        Bottom,
    }

    private enum TextSize {
        Small,
        Medium,
        Big,
    }

    public void Write(IEnumerable<ExportLine> lines, string outputDirectory) {
        logger.Here().Verbose("Start writing voice sheets to {OutputDirectory}", outputDirectory);
        var linkCache = editorEnvironment.LinkCache;

        foreach (var voiceTypeGrouping in lines.GroupBy(l => l.VoiceType)) {
            var voiceType = voiceTypeGrouping.Key;
            // Create a spreadsheet document
            var filePath = Path.Combine(outputDirectory, $"{voiceType}.xlsx");
            if (!fileSystem.Directory.Exists(outputDirectory)) {
                fileSystem.Directory.CreateDirectory(outputDirectory);
            }

            using var spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart.Stylesheet = new Stylesheet {
                Fonts = new Fonts(
                    new Font(
                        new FontName { Val = new StringValue("Arial") },
                        new FontSize { Val = 10 }),
                    new Font(
                        new Bold(),
                        new FontName { Val = new StringValue("Arial") },
                        new FontSize { Val = 10 }),
                    new Font(
                        new FontName { Val = new StringValue("Arial") },
                        new FontSize { Val = 12 }),
                    new Font(
                        new Bold(),
                        new FontName { Val = new StringValue("Arial") },
                        new FontSize { Val = 12 }),
                    new Font(
                        new FontName { Val = new StringValue("Arial") },
                        new FontSize { Val = 14 }),
                    new Font(
                        new Bold(),
                        new FontName { Val = new StringValue("Arial") },
                        new FontSize { Val = 14 })
                ),
                Fills = new Fills(
                    new Fill(
                        new PatternFill {
                            PatternType = new EnumValue<PatternValues>(PatternValues.Solid),
                            ForegroundColor = new ForegroundColor {
                                Rgb = new HexBinaryValue { Value = "DDE8CB" }
                            }
                        }
                    ),
                    new Fill(
                        new PatternFill {
                            PatternType = new EnumValue<PatternValues>(PatternValues.Solid),
                            ForegroundColor = new ForegroundColor {
                                Rgb = new HexBinaryValue { Value = "DEE6EF" }
                            }
                        }
                    )
                ),
                Borders = new Borders(
                    new Border(new TopBorder {
                        Style = new EnumValue<BorderStyleValues>(BorderStyleValues.None),
                    }),
                    new Border(new TopBorder {
                        Style = new EnumValue<BorderStyleValues>(BorderStyleValues.MediumDashed),
                    }),
                    new Border(new TopBorder {
                        Style = new EnumValue<BorderStyleValues>(BorderStyleValues.Double),
                    }),
                    new Border(new TopBorder {
                        Style = new EnumValue<BorderStyleValues>(BorderStyleValues.Thin),
                    }),
                    new Border(new TopBorder {
                        Style = new EnumValue<BorderStyleValues>(BorderStyleValues.Dotted),
                    })
                ),
                CellFormats = new CellFormats(
                    // Non-Bold
                    new CellFormat {
                        FontId = 0,
                        ApplyFont = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Bold
                    new CellFormat {
                        FontId = 1,
                        ApplyFont = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Quest No Border
                    new CellFormat {
                        FontId = 1,
                        ApplyFont = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Top),
                            WrapText = true,
                        },
                    },
                    // Quest Border
                    new CellFormat {
                        FontId = 1,
                        ApplyFont = true,
                        BorderId = 0,
                        ApplyBorder = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Top),
                            WrapText = true,
                        },
                    }
                ),
            };
            workbookStylesPart.Stylesheet.Save();

            uint GetCellFormat(
                bool bold,
                TextSize textSize,
                BackgroundColor background,
                BorderStyle borderStyle,
                VerticalAlignment verticalAlignment = VerticalAlignment.Center) {
                if (workbookStylesPart.Stylesheet?.CellFormats is null) throw new InvalidOperationException("CellFormats is null");

                uint? fillId = background switch {
                    BackgroundColor.None => null,
                    BackgroundColor.Green => 0U,
                    BackgroundColor.Blue => 1U,
                    _ => throw new ArgumentOutOfRangeException(nameof(background), background, null)
                };
                var fontId = (bold ? 1U : 0U) + (uint) textSize * 2;
                var borderId = (uint) borderStyle;
                var verticalAlignmentValues = verticalAlignment switch {
                    VerticalAlignment.Top => VerticalAlignmentValues.Top,
                    VerticalAlignment.Center => VerticalAlignmentValues.Center,
                    VerticalAlignment.Bottom => VerticalAlignmentValues.Bottom,
                    _ => throw new ArgumentOutOfRangeException(nameof(verticalAlignment), verticalAlignment, null)
                };

                var cellFormats = workbookStylesPart.Stylesheet.CellFormats.OfType<CellFormat>().ToArray();
                for (uint i = 0; i < cellFormats.Length; i++) {
                    var cellFormat = cellFormats[i];
                    if (cellFormat.BorderId is null || cellFormat.FontId is null || cellFormat.FillId is null || cellFormat.Alignment?.Vertical is null) continue;

                    if (cellFormat.BorderId == borderId
                     && cellFormat.FontId == fontId
                     && cellFormat.FillId == fillId
                     && cellFormat.Alignment.Vertical == verticalAlignmentValues) {
                        return i;
                    }
                }

                workbookStylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat {
                    BorderId = borderId,
                    ApplyBorder = true,
                    FontId = fontId,
                    ApplyFont = true,
                    FillId = fillId,
                    ApplyFill = true,
                    Alignment = new Alignment {
                        Vertical = new EnumValue<VerticalAlignmentValues>(verticalAlignmentValues),
                        WrapText = true,
                    },
                });

                return (uint) ((workbookStylesPart.Stylesheet.CellFormats?.ToArray().Length) ?? throw new InvalidOperationException("CellFormats Count is null")) - 1;
            }

            // Add Sheets to the Workbook
            var sheets = workbookPart.Workbook.AppendChild(new Sheets());

            var isCombatLineGrouping = voiceTypeGrouping.GroupBy(x => x.IsCombatLine).ToArray();
            for (uint speakerId = 0; speakerId < isCombatLineGrouping.Length; speakerId++) {
                var combatLinesGrouping = isCombatLineGrouping[speakerId];

                // Add columns
                var columns = new Columns(
                    // Quest
                    new Column {
                        Min = 1,
                        Max = 1,
                        Width = 20,
                        CustomWidth = true
                    },
                    // Context
                    new Column {
                        Min = 2,
                        Max = 2,
                        Width = 70,
                        CustomWidth = true
                    },
                    // Line to speak
                    new Column {
                        Min = 3,
                        Max = 3,
                        Width = 110,
                        CustomWidth = true
                    },
                    // Acting Note
                    new Column {
                        Min = 4,
                        Max = 4,
                        Width = 35,
                        CustomWidth = true
                    },
                    // Filecutting Note
                    new Column {
                        Min = 5,
                        Max = 5,
                        Width = 60,
                        CustomWidth = true
                    },
                    // Filename
                    new Column {
                        Min = 6,
                        Max = 6,
                        Width = 50,
                        CustomWidth = true
                    });

                // Add a WorksheetPart to the Workbook
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(columns, new SheetData());

                // Create a new sheet
                var sheet = new Sheet {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = new UInt32Value(speakerId),
                    Name = combatLinesGrouping.Key ? "Combat" : "Main",
                };
                sheets.AppendChild(sheet);

                // Get the SheetData from the Worksheet
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                if (sheetData is null) {
                    sheetData = new SheetData();
                    worksheetPart.Worksheet.AppendChild(sheetData);
                }

                // Create a SheetViews element
                var sheetViews = new SheetViews();

                // Create a SheetView element
                var sheetView = new SheetView { TabSelected = true, WorkbookViewId = 0 };

                // Add merged cells
                var mergeCells = new MergeCells();
                worksheetPart.Worksheet.InsertAfter(mergeCells, sheetData);

                // Add the Pane to the SheetView
                sheetViews.AppendChild(sheetView);
                worksheetPart.Worksheet.AppendChild(sheetViews);

                var currentRow = 1;
                var speakersGrouping = combatLinesGrouping.GroupBy(x => x.Speaker).ToArray();
                foreach (var speakerGrouping in speakersGrouping.OrderBy(x => x.FirstOrDefault()?.SpeakerType)) {
                    // Add a row to the SheetData
                    var row = new Row { CustomHeight = true, Height = new DoubleValue(40.0) };
                    var npc = speakerGrouping.FirstOrDefault()?.Npc;
                    var npcInfo = $"You are voicing {speakerGrouping.Key}";
                    if (npc is not null && !npc.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Traits)) {
                        var race = npc.Race.TryResolve(linkCache);
                        var sex = npc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female) ? "female" : "male";
                        npcInfo += $" who is a {sex} {race?.Name?.String}";
                    }

                    var npcDescriptionStyleId = GetCellFormat(false, TextSize.Big, BackgroundColor.None, BorderStyle.None);
                    row.Append(
                        CreateCell(string.Empty, npcDescriptionStyleId),
                        CreateCell(npcInfo, npcDescriptionStyleId)
                    );
                    sheetData.AppendChild(row);
                    currentRow++;

                    // Add a row to the SheetData
                    var headerStyleId = GetCellFormat(true, TextSize.Medium, BackgroundColor.None, BorderStyle.None);
                    row = new Row { CustomHeight = true, Height = new DoubleValue(20.0) };
                    row.Append(
                        CreateCell("Quest", headerStyleId),
                        CreateCell("Context", headerStyleId),
                        CreateCell("Line to speak", headerStyleId),
                        CreateCell("Acting Note", headerStyleId),
                        CreateCell("Filecutting Note", headerStyleId),
                        CreateCell("Filename", headerStyleId)
                    );
                    sheetData.AppendChild(row);
                    currentRow++;

                    var backgroundColor = BackgroundColor.Blue;

                    // Start from row 3 due to first two rows being headers
                    var speakerArray = speakerGrouping.OrderBy(x => x.Quest.EditorID ?? x.Quest.Name?.String ?? x.Quest.FormKey.ToString()).ToList();

                    // Move dialogue quests to the start of the list
                    var dialogueQuests = speakerArray
                        .Where(x => x.Quest.EditorID is not null
                         && x.Quest.EditorID.Contains("Dialogue", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    if (dialogueQuests.Length > 0 && dialogueQuests.Length < speakerArray.Count) {
                        var dialogueQuestFormKeys = dialogueQuests.Select(x => x.Quest.FormKey).ToArray();
                        speakerArray.RemoveAll(x => dialogueQuestFormKeys.Contains(x.Quest.FormKey));
                        speakerArray.InsertRange(0, dialogueQuests);
                    }

                    var questGrouping = speakerArray.GroupBy(x => x.Quest.FormKey.ToString()).ToArray();
                    for (var questId = 0; questId < questGrouping.Length; questId++) {
                        var quest = questGrouping[questId];
                        var questArray = quest
                            .OrderBy(x => x.Branch?.EditorID ?? x.Branch?.FormKey.ToString())
                            .ToList();

                        mergeCells.AppendChild(new MergeCell { Reference = $"A{currentRow}:A{currentRow + questArray.Count - 1}" });

                        currentRow += questArray.Count;

                        var branchGrouping = questArray
                            .GroupBy(x => x.Branch?.FormKey.ToString())
                            .OrderBy(x => {
                                var startingTopic = x.FirstOrDefault(l => l.Branch is {} b && b.StartingTopic.FormKey == l.Topic.FormKey);
                                if (startingTopic is null) return uint.MinValue;

                                var c = startingTopic.Responses.Conditions
                                    .OfType<IConditionFloatGetter>()
                                    .FirstOrDefault(c => c.Data is IGetStageConditionDataGetter);
                                if (c?.ComparisonValue is null) return uint.MinValue;

                                return c.ComparisonValue;
                            }).ToArray();
                        for (var branchId = 0; branchId < branchGrouping.Length; branchId++) {
                            var branch = branchGrouping[branchId];
                            var branchArray = branch
                                .OrderBy(x => {
                                    var subtype = x.Topic.SubtypeName.ToDialogTopicSubtype();
                                    if (subtype is not null && subtype.Value.IsCombatLine()) return -1;

                                    return 1;
                                })
                                .ThenBy(x => x.Action?.Index ?? uint.MaxValue)
                                .ThenBy(x => x.Topic.EditorID ?? x.Topic.FormKey.ToString())
                                .ToList();
                            // Move topics in branch to the start of the list that are starting topics
                            var startingLines = branchArray
                                .Where(x => x.Branch is {} b && b.StartingTopic.FormKey == x.Topic.FormKey)
                                .ToArray();
                            if (startingLines.Length > 0 && startingLines.Length < branchArray.Count) {
                                var startingTopicFormKey = startingLines[0].Topic.FormKey;
                                branchArray.RemoveAll(x => x.Topic.FormKey == startingTopicFormKey);
                                branchArray.InsertRange(0, startingLines);
                            }

                            var topicGrouping = branchArray.GroupBy(x => x.Topic.FormKey.ToString()).ToArray();
                            for (var topicId = 0; topicId < topicGrouping.Length; topicId++) {
                                var topic = topicGrouping[topicId];
                                var topicArray = topic
                                    .OrderBy(x => x.Responses.EditorID ?? x.Responses.FormKey.ToString())
                                    .ToList();
                                var responsesGrouping = topicArray.GroupBy(x => x.Topic.FormKey.ToString()).ToArray();
                                backgroundColor = backgroundColor == BackgroundColor.Blue ? BackgroundColor.Green : BackgroundColor.Blue;
                                for (var responsesId = 0; responsesId < responsesGrouping.Length; responsesId++) {
                                    var responses = responsesGrouping[responsesId];
                                    var responsesArray = responses.ToArray();
                                    var responseGrouping = responsesArray.GroupBy(x => x.Responses.FormKey.ToString()).ToArray();
                                    for (var responseId = 0; responseId < responseGrouping.Length; responseId++) {
                                        var response = responseGrouping[responseId];
                                        var responseArray = response.OrderBy(x => x.Path).ToArray();
                                        for (var lineId = 0; lineId < responseArray.Length; lineId++) {
                                            var line = responseArray[lineId];
                                            BorderStyle borderStyle;
                                            if (lineId > 0) {
                                                borderStyle = BorderStyle.None;
                                            } else if (responseId > 0) {
                                                borderStyle = BorderStyle.Dotted;
                                            } else if (responsesId > 0) {
                                                if (line.Branch is null) {
                                                    // For greetings, goodbyes, etc. use branch separator
                                                    borderStyle = BorderStyle.Double;
                                                } else {
                                                    borderStyle = BorderStyle.Thin;
                                                }
                                            } else if (topicId > 0) {
                                                borderStyle = BorderStyle.Thin;
                                            } else if (branchId > 0) {
                                                borderStyle = BorderStyle.Double;
                                            } else if (questId > 0) {
                                                borderStyle = BorderStyle.MediumDashed;
                                            } else {
                                                borderStyle = BorderStyle.None;
                                            }

                                            var defaultStyleId = GetCellFormat(false, TextSize.Small, backgroundColor, borderStyle);
                                            var boldStyleId = GetCellFormat(true, TextSize.Medium, backgroundColor, borderStyle);
                                            var questBorder = questId == 0 ? BorderStyle.None : BorderStyle.MediumDashed;
                                            var questStyleId = GetCellFormat(true, TextSize.Small, BackgroundColor.None, questBorder, VerticalAlignment.Top);
                                            row = new Row { CustomHeight = true, Height = new DoubleValue(40.0) };
                                            row.Append(
                                                CreateCell(line.Quest.Name?.String ?? line.Quest.EditorID ?? line.Quest.FormKey.ToString(), questStyleId),
                                                CreateCell(lineId > 0 ? string.Empty : line.Context, defaultStyleId),
                                                CreateCell(line.Line, boldStyleId),
                                                CreateCell(line.VaNote, defaultStyleId),
                                                CreateCell(string.Empty, defaultStyleId),
                                                CreateCell(Path.GetFileNameWithoutExtension(line.Path), defaultStyleId)
                                            );

                                            sheetData.AppendChild(row);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    sheetData.AppendChild(new Row { CustomHeight = true, Height = new DoubleValue(50.0) });
                    currentRow++;
                }

                worksheetPart.Worksheet.Save();
            }

            // Save the changes
            workbookStylesPart.Stylesheet.Save();
            workbookPart.Workbook.Save();
        }

        logger.Here().Verbose("Finished writing voice sheets to {OutputDirectory}", outputDirectory);
    }

    private static Cell CreateCell(string text, uint? styleId = null) {
        var cell = new Cell {
            StyleIndex = styleId is null ? null : new UInt32Value { Value = styleId.Value },
            DataType = new EnumValue<CellValues>(CellValues.String),
            CellValue = new CellValue(text)
        };
        return cell;
    }
}
