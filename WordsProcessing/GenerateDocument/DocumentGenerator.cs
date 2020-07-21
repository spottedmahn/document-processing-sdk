﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if NETCOREAPP
using Telerik.Documents.Common.Model;
using Telerik.Documents.Primitives;
using Telerik.Documents.Media;
using Telerik.Documents.Core.Fonts;
#else
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Documents.Spreadsheet.Model;
#endif
using Telerik.Windows.Documents.Common.FormatProviders;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.FormatProviders.Rtf;
using Telerik.Windows.Documents.Flow.FormatProviders.Txt;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Media;
using System.Drawing.Imaging;

namespace GenerateDocument
{
    public class DocumentGenerator
    {
        private static readonly ThemableColor greenColor = new ThemableColor(Color.FromArgb(255, 92, 230, 0));
        private readonly string sampleDataFolder = "SampleData/";

        private string selectedExportFormat;

        public string SelectedExportFormat
        {
            get
            {
                return this.selectedExportFormat;
            }
            set
            {
                if (!object.Equals(this.selectedExportFormat, value))
                {
                    this.selectedExportFormat = value;
                }
            }
        }

        public DocumentGenerator()
            : this("docx")
        {
        }

        public DocumentGenerator(string documentFormat)
        {
            this.SelectedExportFormat = documentFormat;
        }

        public void Generate()
        {
            RadFlowDocument document = this.CreateDocument();

            this.SaveDocument(document, this.SelectedExportFormat);
        }

        private RadFlowDocument CreateDocument()
        {
            RadFlowDocument document = new RadFlowDocument();
            RadFlowDocumentEditor editor = new RadFlowDocumentEditor(document);
            editor.ParagraphFormatting.TextAlignment.LocalValue = Alignment.Justified;

            // Body
            editor.InsertLine("Dear Telerik User,");
            editor.InsertText("We’re happy to introduce the Telerik RadWordsProcessing component. High performance library that enables you to read, write and manipulate documents in DOCX, RTF and plain text format. The document model is independent from UI and ");
            Run run = editor.InsertText("does not require");
            run.Underline.Pattern = UnderlinePattern.Single;
            editor.InsertLine(" Microsoft Office.");

            var run2 = editor.InsertLine("My Table Header");
            //todo this isn't working, why not??
            //run.FontWeight = FontWeights.Bold;
            run2.FontSize += 28;
            run2.Paragraph.Spacing.SpacingAfter = 0;
            run2.Paragraph.Spacing.LineSpacing = 0;
            //editor.MoveToInlineEnd(run);

            var table = editor.InsertTable(3, 3);
            table.Borders = new TableBorders(new Border(1, BorderStyle.Single, new ThemableColor(Colors.Black)));
            var paragraph = table.Rows[0].Cells[0].Blocks.AddParagraph();
            editor.MoveToParagraphStart(paragraph);
            paragraph.Inlines.AddRun("cell text");
            //editor.InsertParagraph();
            editor.MoveToTableEnd(table);

            var bitmap = new System.Drawing.Bitmap(300, 300);
            var blackPen = new System.Drawing.Pen(System.Drawing.Color.Black, 3);

            int x1 = 0;
            int y1 = 100;
            int x2 = 300;
            int y2 = 100;
            var borderSize = 10;
            var pos = new System.Drawing.Point(borderSize, borderSize);
            // Draw line to screen.
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            using (var solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
            //using (var borderBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.Red, borderSize))
            {
                graphics.FillRectangle(solidBrush, 0, 0, 300, 300);
                //graphics.FillRectangle(borderBrush, pos.X - borderSize, pos.Y - borderSize,
                //    bitmap.Width + borderSize, bitmap.Height + borderSize);

                graphics.DrawLine(pen, new System.Drawing.Point(0, 0), new System.Drawing.Point(0, bitmap.Width));
                graphics.DrawLine(pen, new System.Drawing.Point(0, 0), new System.Drawing.Point(bitmap.Height, 0));
                graphics.DrawLine(pen, new System.Drawing.Point(0, bitmap.Width), new System.Drawing.Point(bitmap.Height, bitmap.Width));
                graphics.DrawLine(pen, new System.Drawing.Point(bitmap.Height, 0), new System.Drawing.Point(bitmap.Height, bitmap.Width));

                graphics.DrawLine(blackPen, x1, y1, x2, y2);
            }
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Jpeg);
                bitmap.Save("blah.jpg");
                editor.InsertImageInline(memoryStream, "jpeg");
            }
            editor.InsertParagraph();

            editor.InsertText("The current community preview version comes with full rich-text capabilities including ");
            editor.InsertText("bold, ").FontWeight = FontWeights.Bold;
            editor.InsertText("italic, ").FontStyle = FontStyles.Italic;
            editor.InsertText("underline,").Underline.Pattern = UnderlinePattern.Single;
            editor.InsertText(" font sizes and ").FontSize = 20;
            editor.InsertText("colors ").ForegroundColor = greenColor;

            editor.InsertLine("as well as text alignment and indentation. Other options include tables, hyperlinks, inline and floating images. Even more sweetness is added by the built-in styles and themes.");

            editor.InsertText("Here at Telerik we strive to provide the best services possible and fulfill all needs you as a customer may have. We would appreciate any feedback you send our way through the ");
            editor.InsertHyperlink("public forums", "http://www.telerik.com/forums", false, "Telerik Forums");
            editor.InsertLine(" or support ticketing system.");

            editor.InsertLine("We hope you’ll enjoy RadWordsProcessing as much as we do. Happy coding!");
            editor.InsertParagraph();
            editor.InsertText("Kind regards,");

            this.CreateSignature(editor);

            editor.InsertParagraph();
            editor.InsertLine("blah blah blah");

            this.CreateHeader(editor);

            this.CreateFooter(editor);

            return document;
        }

        private void CreateSignature(RadFlowDocumentEditor editor)
        {
            Table signatureTable = editor.InsertTable(1, 2);
            signatureTable.Rows[0].Cells[0].Borders = new TableCellBorders(
                new Border(BorderStyle.None),
                new Border(BorderStyle.None),
                new Border(4, BorderStyle.Single, greenColor),
                new Border(BorderStyle.None));

            // Create paragraph with image
            signatureTable.Rows[0].Cells[0].PreferredWidth = new TableWidthUnit(140);
            Paragraph paragraphWithImage = signatureTable.Rows[0].Cells[0].Blocks.AddParagraph();
            paragraphWithImage.Spacing.SpacingAfter = 0;
            editor.MoveToParagraphStart(paragraphWithImage);

#if NETCOREAPP
            using (Stream stream = File.OpenRead(this.sampleDataFolder + "Telerik_logo.jpg"))
            {
                editor.InsertImageInline(stream, "jpg", new Size(118, 28));
            }
#else
            using (Stream stream = File.OpenRead(sampleDataFolder + "Telerik_logo.png"))
            {
                editor.InsertImageInline(stream, "png", new Size(118, 28));
            }
#endif

            // Create cell with name and position
            signatureTable.Rows[0].Cells[1].Padding = new Telerik.Windows.Documents.Primitives.Padding(12, 0, 0, 0);
            Paragraph cellParagraph = signatureTable.Rows[0].Cells[1].Blocks.AddParagraph();
            cellParagraph.Spacing.SpacingAfter = 0;
            editor.MoveToParagraphStart(cellParagraph);
            editor.CharacterFormatting.FontSize.LocalValue = 12;

            editor.InsertText("Jane Doe").FontWeight = FontWeights.Bold;
            editor.InsertParagraph().Spacing.SpacingAfter = 0;
            editor.InsertText("Support Officer");
        }

        private void CreateFooter(RadFlowDocumentEditor editor)
        {
            Footer footer = editor.Document.Sections.First().Footers.Add();
            Paragraph paragraph = footer.Blocks.AddParagraph();
            paragraph.TextAlignment = Alignment.Right;

            editor.MoveToParagraphStart(paragraph);
        }

        private void CreateHeader(RadFlowDocumentEditor editor)
        {
            Header header = editor.Document.Sections.First().Headers.Add();
            editor.MoveToParagraphStart(header.Blocks.AddParagraph());

#if NETCOREAPP
            using (Stream stream = File.OpenRead(this.sampleDataFolder + "Telerik_develop_experiences.jpg"))
            {
                editor.InsertImageInline(stream, "jpg", new Size(660, 237));
            }
#else
            using (Stream stream = File.OpenRead(this.sampleDataFolder + "Telerik_develop_experiences.png"))
            {
                editor.InsertImageInline(stream, "png", new Size(660, 237));
            }
#endif
        }

        private void SaveDocument(RadFlowDocument document, string selectedFormat)
        {
            string selectedFormatLower = selectedFormat.ToLower();

            IFormatProvider<RadFlowDocument> formatProvider = null;
            switch (selectedFormatLower)
            {
                case "docx":
                    formatProvider = new DocxFormatProvider();
                    break;
                case "rtf":
                    formatProvider = new RtfFormatProvider();
                    break;
                case "txt":
                    formatProvider = new TxtFormatProvider();
                    break;
                case "html":
                    formatProvider = new HtmlFormatProvider();
                    break;
                case "pdf":
                    formatProvider = new PdfFormatProvider();
                    break;
            }

            if (formatProvider == null)
            {
                Console.WriteLine("Uknown or not supported format.");
                return;
            }

            string path = $"Sample document - {DateTime.Now:yyyy-MM-dd hh.mm.ss tt}." + selectedFormat;
            using (FileStream stream = File.OpenWrite(path))
            {
                formatProvider.Export(document, stream);
            }

            Console.Write("Document generated.");

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}