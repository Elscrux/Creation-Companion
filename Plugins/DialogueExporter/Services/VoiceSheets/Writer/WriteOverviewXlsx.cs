using System.Globalization;
using System.IO.Abstractions;
using CreationEditor;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Serilog;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;
using Path = System.IO.Path;

namespace DialogueExporter.Services.VoiceSheets.Writer;

public sealed class WriteOverviewXlsx(
    ILogger logger,
    IFileSystem fileSystem) {

    public void WriteOverview(IEnumerable<ExportLine> lines, string outputFilePath) {
        logger.Here().Verbose("Start writing overview voice sheet to {OutputFilePath}", outputFilePath);

        var directory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(directory) && !fileSystem.Directory.Exists(directory)) {
            fileSystem.Directory.CreateDirectory(directory);
        }

        using var spreadsheetDocument = SpreadsheetDocument.Create(outputFilePath, SpreadsheetDocumentType.Workbook);

        var workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
        stylesPart.Stylesheet = CreateStylesheet();
        stylesPart.Stylesheet.Save();

        var sheets = workbookPart.Workbook.AppendChild(new Sheets());

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

        var columns = new Columns(
            new Column { Min = 1, Max = 1, Width = 40, CustomWidth = true }, // Speaker
            new Column { Min = 2, Max = 4, Width = 15, CustomWidth = true } // Counts
        );

        var sheetData = new SheetData();
        worksheetPart.Worksheet = new Worksheet(columns, sheetData);

        var sheet = new Sheet {
            Id = workbookPart.GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = "Overview",
        };
        sheets.AppendChild(sheet);

        const uint headerStyle = 1U; // bold
        const uint normalStyle = 0U; // normal

        var currentRowIndex = 1U;

        // Header row
        var (speakerCellRef, uniqueCellRef, totalCellRef, questCellRef, nonQuestCellRef) = GetCellRefs(currentRowIndex);
        var headerRow = new Row { RowIndex = currentRowIndex };
        headerRow.Append(
            CreateTextCell(speakerCellRef, "Speaker", headerStyle),
            CreateTextCell(uniqueCellRef, "Unique Lines", headerStyle),
            CreateTextCell(totalCellRef, "Total Lines", headerStyle),
            CreateTextCell(questCellRef, "Quest Lines", headerStyle),
            CreateTextCell(nonQuestCellRef, "Non Quest Lines", headerStyle)
        );
        sheetData.AppendChild(headerRow);
        currentRowIndex++;

        // Comments setup
        var commentsPart = worksheetPart.AddNewPart<WorksheetCommentsPart>();
        commentsPart.Comments = new Comments {
            Authors = new Authors(new Author("Dialogue Exporter")),
            CommentList = new CommentList(),
        };

        // Classic Excel comments require a legacy drawing relationship; we only need the comments
        // data to exist for tools like Excel to show them, so we omit the VML drawing setup here.

        var bySpeakerType = lines.GroupBy(x => x.SpeakerType)
            .OrderBy(g => g.Key.ToString());

        foreach (var speakerTypeGroup in bySpeakerType) {
            // Speaker type header row
            var allLines = speakerTypeGroup.ToArray();
            var (uniqueLines, totalLines, questLines, nonQuestLines) = GetLinesStats(allLines);

            (speakerCellRef, uniqueCellRef, totalCellRef, questCellRef, nonQuestCellRef) = GetCellRefs(currentRowIndex);

            var typeRow = new Row { RowIndex = currentRowIndex };
            typeRow.Append(
                CreateTextCell(speakerCellRef, speakerTypeGroup.Key.ToString(), headerStyle),
                CreateNumberCell(uniqueCellRef, uniqueLines, normalStyle),
                CreateNumberCell(totalCellRef, totalLines, normalStyle),
                CreateNumberCell(questCellRef, questLines, normalStyle),
                CreateNumberCell(nonQuestCellRef, nonQuestLines, normalStyle)
            );
            sheetData.AppendChild(typeRow);
            currentRowIndex++;

            var bySpeaker = speakerTypeGroup
                .GroupBy(x => x.Speaker)
                .OrderBy(g => g.Key)
                .ToArray();

            (speakerCellRef, uniqueCellRef, totalCellRef, questCellRef, nonQuestCellRef) = GetCellRefs(currentRowIndex);
            var averageUniqueLines = bySpeaker.Average(x => x.DistinctBy(UniqueLine).Count());
            var averageTotalLines = bySpeaker.Average(x => x.Count());
            var averageQuestLines = bySpeaker.Average(x => x.Count(IsQuestLine));
            var averageNonQuestLines = averageTotalLines - averageQuestLines;
            var typeAveragesRow = new Row { RowIndex = currentRowIndex };
            typeAveragesRow.Append(
                CreateTextCell(speakerCellRef, speakerTypeGroup.Key + " (Average)", normalStyle),
                CreateNumberCell(uniqueCellRef, averageUniqueLines, normalStyle),
                CreateNumberCell(totalCellRef, averageTotalLines, normalStyle),
                CreateNumberCell(questCellRef, averageQuestLines, normalStyle),
                CreateNumberCell(nonQuestCellRef, averageNonQuestLines, normalStyle)
            );
            sheetData.AppendChild(typeAveragesRow);
            currentRowIndex++;

            foreach (var speakerGroup in bySpeaker) {
                allLines = speakerGroup.ToArray();
                (uniqueLines, totalLines, questLines, nonQuestLines) = GetLinesStats(allLines);

                var row = new Row { RowIndex = currentRowIndex };
                (speakerCellRef, uniqueCellRef, totalCellRef, questCellRef, nonQuestCellRef) = GetCellRefs(currentRowIndex);

                row.Append(
                    CreateTextCell(speakerCellRef, speakerGroup.Key, normalStyle),
                    CreateNumberCell(uniqueCellRef, uniqueLines, normalStyle),
                    CreateNumberCell(totalCellRef, totalLines, normalStyle),
                    CreateNumberCell(questCellRef, questLines, normalStyle),
                    CreateNumberCell(nonQuestCellRef, nonQuestLines, normalStyle)
                );

                sheetData.AppendChild(row);

                // Comments per cell
                var comments = commentsPart.Comments.CommentList!;
                if (uniqueLines > 0) {
                    var text = BuildCommentText(allLines.DistinctBy(UniqueLine));
                    comments.AppendChild(CreateComment(uniqueCellRef, text));
                }

                if (totalLines > 0) {
                    var text = BuildCommentText(allLines);
                    comments.AppendChild(CreateComment(totalCellRef, text));
                }

                if (questLines > 0) {
                    var text = BuildCommentText(allLines.Where(IsQuestLine));
                    comments.AppendChild(CreateComment(questCellRef, text));
                }

                if (nonQuestLines > 0) {
                    var text = BuildCommentText(allLines.Where(x => !IsQuestLine(x)));
                    comments.AppendChild(CreateComment(nonQuestCellRef, text));
                }

                currentRowIndex++;
            }
        }

        commentsPart.Comments.Save();
        worksheetPart.Worksheet.Save();
        workbookPart.Workbook.Save();

        logger.Here().Verbose("Finished writing overview voice sheet to {OutputFilePath}", outputFilePath);
    }

    private static (int uniqueLines, int totalLines, int questLines, int nonQuestLines) GetLinesStats(ExportLine[] allLines) {
        var totalLines = allLines.Length;
        var uniqueLines = allLines.DistinctBy(UniqueLine).Count();
        var questLines = allLines.Count(IsQuestLine);
        var nonQuestLines = totalLines - questLines;
        return (uniqueLines, totalLines, questLines, nonQuestLines);
    }

    private static Stylesheet CreateStylesheet() {
        return new Stylesheet {
            Fonts = new Fonts(
                new Font(
                    new FontName { Val = new StringValue("Arial") },
                    new FontSize { Val = 10 }),
                new Font(
                    new Bold(),
                    new FontName { Val = new StringValue("Arial") },
                    new FontSize { Val = 10 })
            ),
            Fills = new Fills(new Fill(new PatternFill { PatternType = PatternValues.None })),
            Borders = new Borders(new Border()),
            CellFormats = new CellFormats(
                new CellFormat { FontId = 0, ApplyFont = true },
                new CellFormat { FontId = 1, ApplyFont = true }
            ),
        };
    }

    private static Cell CreateTextCell(string cellReference, string text, uint styleIndex) {
        return new Cell {
            CellReference = cellReference,
            DataType = CellValues.String,
            CellValue = new CellValue(text),
            StyleIndex = styleIndex,
        };
    }

    private static Cell CreateNumberCell(string cellReference, double value, uint styleIndex) {
        return new Cell {
            CellReference = cellReference,
            DataType = CellValues.Number,
            CellValue = new CellValue(value.ToString(CultureInfo.InvariantCulture)),
            StyleIndex = styleIndex,
        };
    }

    private static Comment CreateComment(string cellReference, string text) {
        return new Comment {
            Reference = cellReference,
            AuthorId = 0,
            CommentText = new CommentText(new Run(new Text(text)))
        };
    }

    private static string BuildCommentText(IEnumerable<ExportLine> lines) {
        var transformedLines = lines
            .Select(x => x.Line)
            .Order()
            .ToArray();

        return string.Join("\n", transformedLines);
    }

    private static (string speakerCellRef, string uniqueCellRef, string totalCellRef, string questCellRef, string nonQuestCellRef) GetCellRefs(uint currentRowIndex) {
        var speakerCellRef = "A" + currentRowIndex;
        var uniqueCellRef = "B" + currentRowIndex;
        var totalCellRef = "C" + currentRowIndex;
        var questCellRef = "D" + currentRowIndex;
        var nonQuestCellRef = "E" + currentRowIndex;
        return (speakerCellRef, uniqueCellRef, totalCellRef, questCellRef, nonQuestCellRef);
    }

    private static string UniqueLine(ExportLine l) {
        return l.Line;
    }

    private static bool IsQuestLine(ExportLine x) => x.Quest.EditorID?.Contains("Dialogue", StringComparison.OrdinalIgnoreCase) is not true;
}
