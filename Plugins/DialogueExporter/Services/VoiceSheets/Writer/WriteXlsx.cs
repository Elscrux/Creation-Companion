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
                        new FontSize { Val = 10 })
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
                        Style = new EnumValue<BorderStyleValues>(BorderStyleValues.MediumDashed),
                    }),
                    new Border(new TopBorder {
                        Style = new EnumValue<BorderStyleValues>(BorderStyleValues.Double),
                    }),
                    new Border(new TopBorder {
                        Style = new EnumValue<BorderStyleValues>(BorderStyleValues.Thin),
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
                            TextRotation = 90,
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
                            TextRotation = 90
                        },
                    },
                    // Non-Bold - Blue
                    new CellFormat {
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 0,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Quest Start - Blue
                    new CellFormat {
                        BorderId = 0,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 0,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Branch Start - Blue
                    new CellFormat {
                        BorderId = 1,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 0,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Topic Start - Blue
                    new CellFormat {
                        BorderId = 2,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 0,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Responses Start - Blue
                    new CellFormat {
                        BorderId = 3,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 0,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Response Start - Blue
                    new CellFormat {
                        BorderId = 4,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 0,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Line Start - Blue
                    new CellFormat {
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 0,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Non-Bold - Green
                    new CellFormat {
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 1,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Quest Start - Green
                    new CellFormat {
                        BorderId = 0,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 1,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Branch Start - Green
                    new CellFormat {
                        BorderId = 1,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 1,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Topic Start - Green
                    new CellFormat {
                        BorderId = 2,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 1,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Responses Start - Green
                    new CellFormat {
                        BorderId = 3,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 1,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Response Start - Green
                    new CellFormat {
                        BorderId = 4,
                        ApplyBorder = true,
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 1,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    },
                    // Line Start - Green
                    new CellFormat {
                        FontId = 0,
                        ApplyFont = true,
                        FillId = 1,
                        ApplyFill = true,
                        Alignment = new Alignment {
                            Vertical = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center),
                            WrapText = true,
                        },
                    }
                ),
            };
            workbookStylesPart.Stylesheet.Save();

            const uint nonBoldCellFormatId = 0;
            const uint boldCellFormatId = 1;
            const uint questNoBorderCellFormatId = 2;
            const uint questBorderCellFormatId = 3;
            const uint blueNonBoldCellFormatId = 4;
            const uint questStartCellFormatId = 5;
            const uint branchStartCellFormatId = 6;
            const uint topicStartCellFormatId = 7;
            const uint responsesStartCellFormatId = 8;
            const uint responseStartCellFormatId = 9;
            const uint lineStartCellFormatId = 10;

            // Add Sheets to the Workbook
            var sheets = workbookPart.Workbook.AppendChild(new Sheets());

            var speakerGrouping = voiceTypeGrouping.GroupBy(x => x.Speaker).ToArray();
            for (uint speakerId = 0; speakerId < speakerGrouping.Length; speakerId++) {
                var speaker = speakerGrouping[speakerId];

                // Add columns
                var columns = new Columns(
                    // Quest
                    new Column {
                        Min = 1,
                        Max = 1,
                        Width = 10,
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
                        Width = 100,
                        CustomWidth = true
                    },
                    // Acting Note
                    new Column {
                        Min = 4,
                        Max = 4,
                        Width = 35,
                        CustomWidth = true
                    },
                    // Facial Emotion
                    new Column {
                        Min = 5,
                        Max = 5,
                        Width = 15,
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
                    Name = speaker.Key,
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

                // Create a Pane element to freeze the top row
                var pane = new Pane {
                    VerticalSplit = 2,
                    TopLeftCell = "A3",
                    ActivePane = PaneValues.BottomRight,
                    State = PaneStateValues.Frozen
                };

                // Add the Pane to the SheetView
                sheetView.AppendChild(pane);
                sheetViews.AppendChild(sheetView);
                worksheetPart.Worksheet.AppendChild(sheetViews);

                // Add a row to the SheetData
                var row = new Row { CustomHeight = true, Height = new DoubleValue(18.0) };
                var npc = speaker.FirstOrDefault()?.Npc;
                var npcInfo = $"You are voicing {speaker.Key}";
                if (npc is not null) {
                    var race = npc.Race.TryResolve(linkCache);
                    var sex = npc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female) ? "female" : "male";
                    npcInfo += $" who is a {sex} {race?.Name?.String}";
                }

                row.Append(
                    CreateCell(string.Empty, nonBoldCellFormatId),
                    CreateCell(npcInfo, nonBoldCellFormatId)
                );
                sheetData.AppendChild(row);

                // Add a row to the SheetData
                row = new Row { CustomHeight = true, Height = new DoubleValue(18.0) };
                row.Append(
                    CreateCell("Quest", boldCellFormatId),
                    CreateCell("Context", boldCellFormatId),
                    CreateCell("Line to speak", boldCellFormatId),
                    CreateCell("Acting Note", boldCellFormatId),
                    CreateCell("Facial Emotion", boldCellFormatId),
                    CreateCell("Filename", boldCellFormatId)
                );
                sheetData.AppendChild(row);

                // Add merged cells
                var mergeCells = new MergeCells();
                worksheetPart.Worksheet.InsertAfter(mergeCells, sheetData);

                var isBlue = true;

                // Start from row 3 due to first two rows being headers
                var currentRow = 3;
                var speakerArray = speaker.OrderBy(x => x.Quest.EditorID ?? x.Quest.Name?.String ?? x.Quest.FormKey.ToString()).ToList();

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
                            isBlue = !isBlue;
                            for (var responsesId = 0; responsesId < responsesGrouping.Length; responsesId++) {
                                var responses = responsesGrouping[responsesId];
                                var responsesArray = responses.ToArray();
                                var responseGrouping = responsesArray.GroupBy(x => x.Responses.FormKey.ToString()).ToArray();
                                for (var responseId = 0; responseId < responseGrouping.Length; responseId++) {
                                    var response = responseGrouping[responseId];
                                    var responseArray = response.OrderBy(x => x.Path).ToArray();
                                    for (var lineId = 0; lineId < responseArray.Length; lineId++) {
                                        var line = responseArray[lineId];
                                        uint styleId;
                                        var offset = isBlue ? (uint) 7 : 0;
                                        if (lineId > 0) {
                                            styleId = lineStartCellFormatId + offset;
                                        } else if (responseId > 0) {
                                            styleId = responseStartCellFormatId + offset;
                                        } else if (responsesId > 0) {
                                            if (line.Branch is null) {
                                                // For greetings, goodbyes, etc. use branch separator
                                                styleId = branchStartCellFormatId + offset;
                                            } else {
                                                styleId = responsesStartCellFormatId + offset;
                                            }
                                        } else if (topicId > 0) {
                                            styleId = topicStartCellFormatId + offset;
                                        } else if (branchId > 0) {
                                            styleId = branchStartCellFormatId + offset;
                                        } else if (questId > 0) {
                                            styleId = questStartCellFormatId + offset;
                                        } else {
                                            styleId = blueNonBoldCellFormatId;
                                        }

                                        row = new Row { CustomHeight = true, Height = new DoubleValue(36.0) };
                                        row.Append(
                                            CreateCell(line.Quest.Name?.String ?? line.Quest.EditorID ?? line.Quest.FormKey.ToString(),
                                                questId == 0 ? questNoBorderCellFormatId : questBorderCellFormatId),
                                            CreateCell(lineId > 0 ? string.Empty : line.Context, styleId),
                                            CreateCell(line.Line, styleId),
                                            CreateCell(line.VaNote, styleId),
                                            CreateCell(line.Emotion.Contains("Neutral") ? string.Empty : line.Emotion, styleId),
                                            CreateCell(Path.GetFileNameWithoutExtension(line.Path), styleId)
                                        );

                                        sheetData.AppendChild(row);
                                    }
                                }
                            }
                        }
                    }
                }

                worksheetPart.Worksheet.Save();
            }

            // Sort sheets by name
            var sortedSheets = sheets.Elements<Sheet>().OrderBy(s => s.Name?.Value ?? string.Empty).ToList();
            sheets.RemoveAllChildren();
            foreach (var sortedSheet in sortedSheets) {
                sheets.AppendChild(sortedSheet);
            }

            // Save the changes
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
